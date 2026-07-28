using UnityEngine;

public class JC_PooledObject : MonoBehaviour
{
    private JC_ObjectPool _pool;

    public void SetPool(JC_ObjectPool pool)
    {
        _pool = pool;
    }

    public void ReturnToPool()
    {
        if (_pool != null)
        {
            _pool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
