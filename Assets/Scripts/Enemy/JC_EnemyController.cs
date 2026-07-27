using UnityEngine;

public class JC_EnemyController : MonoBehaviour
{
    private enum JC_EnemyMovementMode
    {
        ChaseTarget,
        FixedSpawnDirection
    }

    [SerializeField] private JC_EnemyMovementMode movementMode = JC_EnemyMovementMode.ChaseTarget;
    [SerializeField] private JC_EnemyTargetTracker targetTracker;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField, Range(0f, 1f)] private float targetDirectionChance = 0.3f;

    private float _fixedZPosition;
    private Vector3 _lockedMoveDirection = Vector3.down;

    private void Awake()
    {
        _fixedZPosition = transform.position.z;

        if (targetTracker == null)
        {
            targetTracker = GetComponent<JC_EnemyTargetTracker>();
        }

        if (targetTracker != null)
        {
            targetTracker.EnsureTargetAssigned();
        }

        CacheMoveDirection();
    }

    private void Update()
    {
        Vector3 direction = GetMoveDirection();
        direction.z = 0f;

        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
        Vector3 nextPosition = transform.position + movement;
        nextPosition.z = _fixedZPosition;

        transform.position = nextPosition;
    }

    private Vector3 GetMoveDirection()
    {
        if (movementMode == JC_EnemyMovementMode.FixedSpawnDirection)
        {
            return _lockedMoveDirection;
        }

        if (targetTracker == null || targetTracker.Target == null)
        {
            return Vector3.zero;
        }

        return targetTracker.Target.position - transform.position;
    }

    private void CacheMoveDirection()
    {
        if (movementMode != JC_EnemyMovementMode.FixedSpawnDirection)
        {
            return;
        }

        _lockedMoveDirection = Vector3.down;

        if (targetTracker == null || targetTracker.Target == null)
        {
            return;
        }

        if (Random.value > targetDirectionChance)
        {
            return;
        }

        Vector3 targetDirection = targetTracker.Target.position - transform.position;
        targetDirection.z = 0f;

        if (targetDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        _lockedMoveDirection = targetDirection.normalized;
    }
}
