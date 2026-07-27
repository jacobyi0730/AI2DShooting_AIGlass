using UnityEngine;

public class JC_PlayerShooter : MonoBehaviour
{
    private const string FireInputName = "Fire1";

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private JC_BulletSpawnPoint bulletSpawnPoint;
    [SerializeField, Min(0f)] private float fireInterval = 0.2f;

    private float _nextFireTime;

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

        Instantiate(bulletPrefab, bulletSpawnPoint.Position, Quaternion.identity);
        _nextFireTime = Time.time + fireInterval;
    }
}
