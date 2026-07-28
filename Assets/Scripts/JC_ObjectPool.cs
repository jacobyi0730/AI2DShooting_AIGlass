using System.Collections.Generic;
using UnityEngine;

public class JC_ObjectPool : MonoBehaviour
{
    private readonly Stack<GameObject> _availableObjects = new Stack<GameObject>();
    private readonly HashSet<GameObject> _allObjects = new HashSet<GameObject>();

    private GameObject _prefab;
    private bool _isInitialized;

    public void Initialize(GameObject prefab, int initialSize)
    {
        if (_isInitialized || prefab == null)
        {
            return;
        }

        _prefab = prefab;
        _isInitialized = true;

        for (int index = 0; index < initialSize; index++)
        {
            CreateAndRelease();
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        if (!_isInitialized || _prefab == null)
        {
            return null;
        }

        GameObject pooledObject = _availableObjects.Count > 0
            ? _availableObjects.Pop()
            : CreateObject();

        pooledObject.transform.SetParent(null);
        pooledObject.transform.SetPositionAndRotation(position, rotation);
        pooledObject.SetActive(true);
        return pooledObject;
    }

    public void Release(GameObject pooledObject)
    {
        if (pooledObject == null || !_allObjects.Contains(pooledObject) || _availableObjects.Contains(pooledObject))
        {
            return;
        }

        pooledObject.SetActive(false);
        pooledObject.transform.SetParent(transform);
        _availableObjects.Push(pooledObject);
    }

    private void CreateAndRelease()
    {
        Release(CreateObject());
    }

    private GameObject CreateObject()
    {
        GameObject pooledObject = Instantiate(_prefab, transform);
        JC_PooledObject pooledMarker = pooledObject.GetComponent<JC_PooledObject>();
        if (pooledMarker == null)
        {
            pooledMarker = pooledObject.AddComponent<JC_PooledObject>();
        }

        pooledMarker.SetPool(this);
        _allObjects.Add(pooledObject);
        return pooledObject;
    }
}
