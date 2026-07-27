using UnityEngine;

public class JC_PlayerBullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float despawnYPosition = 7f;

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (transform.position.y > despawnYPosition)
        {
            Destroy(gameObject);
        }
    }
}
