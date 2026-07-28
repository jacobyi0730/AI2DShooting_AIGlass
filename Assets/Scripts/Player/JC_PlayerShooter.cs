using UnityEngine;

public class JC_PlayerShooter : MonoBehaviour
{
    private const string FireInputName = "Fire1";

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private JC_BulletSpawnPoint bulletSpawnPoint;
    [SerializeField, Min(0f)] private float fireInterval = 0.2f;
    [SerializeField, Min(0)] private int initialBulletPoolSize = 20;

    private float _nextFireTime;
    private JC_ObjectPool _bulletPool;

    private void Awake()
    {
        _bulletPool = GetComponent<JC_ObjectPool>();
        if (_bulletPool == null)
        {
            _bulletPool = gameObject.AddComponent<JC_ObjectPool>();
        }

        _bulletPool.Initialize(bulletPrefab, initialBulletPoolSize);
    }

    private void Update()
    {
        if (!Input.GetButton(FireInputName))
        {
            return;
        }

        if (Time.time < _nextFireTime)
        {
            return;
        }

        Fire();
    }

    private void Fire()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null)
        {
            return;
        }

        _bulletPool.Get(bulletSpawnPoint.Position, Quaternion.identity);
        _nextFireTime = Time.time + fireInterval;
    }
}
