using UnityEngine;

public class JC_DamageOnContact : MonoBehaviour
{
    private readonly Collider[] _overlapResults = new Collider[8];

    [SerializeField] private LayerMask playerLayerMask = 1 << 8;
                                                        
    [SerializeField, Min(1)] private int damageAmount = 1;
    [SerializeField, Min(0.05f)] private float damageInterval = 0.5f;

    private Collider _cachedCollider;
    private float _nextDamageTime;

    private void Awake()
    {
        _cachedCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (_cachedCollider == null)
        {
            return;
        }

        bool dealtDamageThisFrame = TryDamageOverlappingPlayer();
        if (!dealtDamageThisFrame && Time.time > _nextDamageTime)
        {
            _nextDamageTime = Time.time;
        }
    }

    private bool TryDamageOverlappingPlayer()
    {
        Bounds bounds = _cachedCollider.bounds;
        int overlapCount = Physics.OverlapBoxNonAlloc(
            bounds.center,
            bounds.extents,
            _overlapResults,
            transform.rotation,
            playerLayerMask);

        if (overlapCount == 0 || Time.time < _nextDamageTime)
        {
            return false;
        }

        for (int index = 0; index < overlapCount; index++)
        {
            Collider overlappedCollider = _overlapResults[index];
            if (overlappedCollider == null || overlappedCollider == _cachedCollider)
            {
                continue;
            }

            JC_Health health = overlappedCollider.GetComponentInParent<JC_Health>();
            if (health == null)
            {
                continue;
            }

            if (health.TakeDamage(damageAmount))
            {
                _nextDamageTime = Time.time + damageInterval;
                return true;
            }
        }

        return false;
    }
}
