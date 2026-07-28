Shader "Custom/URP_Unlit_RimLight_PBRTextures"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [Normal] _NormalMap("Normal Map", 2D) = "bump" {}
        _MetallicMap("Metallic Map", 2D) = "white" {}
        _RoughnessMap("Roughness Map", 2D) = "white" {}
        
        [HDR] _RimColor("Rim Color", Color) = (0, 1, 1, 1)
        _RimPower("Rim Power", Range(0.1, 10.0)) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        Pass
        {
            Name "Unlit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // URP 기본 라이브러리 포함
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // 텍스처 및 샘플러 선언
            TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);      SAMPLER(sampler_NormalMap);
            TEXTURE2D(_MetallicMap);    SAMPLER(sampler_MetallicMap);
            TEXTURE2D(_RoughnessMap);   SAMPLER(sampler_RoughnessMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _RimColor;
                float _RimPower;
                float4 _BaseMap_ST; // BaseMap의 Tiling / Offset
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT; // 노멀 맵 계산을 위한 탄젠트
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 viewDirWS    : TEXCOORD1;
                
                // 탄젠트 스페이스 노멀을 월드 스페이스로 변환하기 위한 TBN
                float3 normalWS     : TEXCOORD3;
                float3 tangentWS    : TEXCOORD4;
                float3 bitangentWS  : TEXCOORD5;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // 위치 및 UV 변환
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                // 월드 공간의 뷰 디렉션 (카메라 방향)
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(worldPos);

                // TBN 행렬 구성 (GetVertexNormalInputs 내장 함수 활용)
                VertexNormalInputs normalInput = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
                OUT.normalWS = normalInput.normalWS;
                OUT.tangentWS = normalInput.tangentWS;
                OUT.bitangentWS = normalInput.bitangentWS;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 1. 텍스처 샘플링
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half4 normalSample = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv);
                
                // Metallic과 Roughness는 R채널을 사용 (텍스처 패킹 방식에 따라 변경 가능)
                half metallic = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, IN.uv).r;
                half roughness = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, IN.uv).r;

                // 2. 노멀 맵 디코딩 및 월드 스페이스 변환
                half3 tangentNormal = UnpackNormal(normalSample);
                float3 normalWS = normalize(tangentNormal.x * IN.tangentWS + 
                                            tangentNormal.y * IN.bitangentWS + 
                                            tangentNormal.z * IN.normalWS);

                // 3. 뷰 디렉션 정규화
                float3 viewDirWS = normalize(IN.viewDirWS);

                // 4. 프레넬(Fresnel) 연산 - 노멀 맵이 적용된 normalWS 사용
                // 림 라이트가 모델의 단순한 외곽이 아닌 텍스처의 굴곡을 따라 맺히게 됩니다.
                float fresnel = 1.0 - saturate(dot(normalWS, viewDirWS));
                fresnel = pow(fresnel, _RimPower);

                // 5. 림 라이트 컬러 적용
                half3 rimLight = _RimColor.rgb * fresnel;

                // 6. 최종 컬러 합성 (Unlit 이므로 조명 연산 없이 Base Color에 발광 추가)
                half3 finalColor = baseColor.rgb + rimLight;

                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }
    }
}