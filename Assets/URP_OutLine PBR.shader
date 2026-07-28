Shader "Custom/URP_OutlineOnly"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineWidth("Outline Width", Range(0.0, 10.0)) = 5.0
    }
    SubShader
    {
        // 아웃라인은 기본 메쉬가 그려진 후 렌더링되어야 겹침 문제가 덜하므로 Queue를 미세하게 조정할 수 있습니다.
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        Pass
        {
            Name "Outline"
            // Renderer Feature 없이 자동으로 그려지도록 UniversalForward 태그를 사용합니다.
            Tags { "LightMode" = "UniversalForward" }

            Cull Front // 핵심: 메쉬의 앞면을 지우고 뒷면만 렌더링합니다.
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // 오브젝트 스페이스에서 노멀(법선) 방향으로 버텍스를 밀어냅니다.
                float3 extrudedPos = IN.positionOS.xyz + (IN.normalOS * _OutlineWidth * 0.01); 
                
                OUT.positionHCS = TransformObjectToHClip(extrudedPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor; // 단색 반환
            }
            ENDHLSL
        }
    }
}