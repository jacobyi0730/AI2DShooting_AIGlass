using UnityEngine;

public class JC_DestroyOnHit : MonoBehaviour
{
    private const int EnemyLayer = 9;
    private const int BulletLayer = 10;

    private readonly Collider[] _overlapResults = new Collider[8];

    [SerializeField] private LayerMask destroySelfOnLayers;
    [SerializeField] private LayerMask destroyOtherOnLayers;

    private Collider _cachedCollider;
    private bool _isPendingDestroy;

    private void Awake()
    {
        _cachedCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (_isPendingDestroy || _cachedCollider == null)
        {
            return;
        }

        Bounds bounds = _cachedCollider.bounds;
        int overlapCount = Physics.OverlapBoxNonAlloc(
            bounds.center,
            bounds.extents,
            _overlapResults,
            transform.rotation);

        for (int index = 0; index < overlapCount; index++)
        {
            Collider overlappedCollider = _overlapResults[index];
            if (overlappedCollider == null || overlappedCollider == _cachedCollider)
            {
                continue;
            }

            int otherLayerMask = 1 << overlappedCollider.gameObject.layer;
            bool shouldDestroySelf = (destroySelfOnLayers.value & otherLayerMask) != 0;
            bool shouldDestroyOther = (destroyOtherOnLayers.value & otherLayerMask) != 0;

            if (!shouldDestroySelf && !shouldDestroyOther)
            {
                continue;
            }

            if (shouldDestroyOther)
            {
                DestroyResolveTarget(gameObject, overlappedCollider);
            }

            if (shouldDestroySelf)
            {
                _isPendingDestroy = true;
                Destroy(gameObject);
                return;
            }
        }
    }

    private static void DestroyResolveTarget(GameObject sourceObject, Collider overlappedCollider)
    {
        JC_DestroyOnHit destroyTarget = overlappedCollider.GetComponentInParent<JC_DestroyOnHit>();
        GameObject targetObject = destroyTarget != null
            ? destroyTarget.gameObject
            : overlappedCollider.gameObject;

        if (sourceObject == null || targetObject == null)
        {
            return;
        }

        bool bulletHitEnemy =
            (sourceObject.layer == BulletLayer && targetObject.layer == EnemyLayer) ||
            (sourceObject.layer == EnemyLayer && targetObject.layer == BulletLayer);

        if (bulletHitEnemy)
        {
            GameObject enemyObject = sourceObject.layer == EnemyLayer ? sourceObject : targetObject;
            JC_ScoreManager.Instance?.TryAddEnemyKill(enemyObject);
        }

        Destroy(targetObject);
    }
}
