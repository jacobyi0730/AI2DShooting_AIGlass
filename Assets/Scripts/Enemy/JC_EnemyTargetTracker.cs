using UnityEngine;

public class JC_EnemyTargetTracker : MonoBehaviour
{
    private const int PlayerLayer = 8;

    [SerializeField] private Transform playerTarget;

    public Transform Target
    {
        get
        {
            if (playerTarget == null)
            {
                TryAssignPlayerTarget();
            }

            return playerTarget;
        }
    }

    private void Awake()
    {
        TryAssignPlayerTarget();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            TryAssignPlayerTarget();
        }
    }

    public void SetTarget(Transform target)
    {
        playerTarget = target;
    }

    public void EnsureTargetAssigned()
    {
        TryAssignPlayerTarget();
    }

    private void TryAssignPlayerTarget()
    {
        if (playerTarget != null)
        {
            return;
        }

        GameObject playerObject = FindPlayerObject();
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
    }

    private static GameObject FindPlayerObject()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            return playerObject;
        }

        GameObject[] sceneObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int index = 0; index < sceneObjects.Length; index++)
        {
            GameObject sceneObject = sceneObjects[index];
            if (sceneObject.layer == PlayerLayer)
            {
                return sceneObject;
            }
        }

        return GameObject.Find("Player");
    }
}
