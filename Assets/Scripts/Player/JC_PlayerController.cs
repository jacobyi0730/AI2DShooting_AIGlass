using UnityEngine;

public class JC_PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private float _fixedZPosition;

    private void Awake()
    {
        _fixedZPosition = transform.position.z;
    }

    private void Update()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));

        Vector3 movement = Vector3.ClampMagnitude(new Vector3(input.x, input.y, 0f), 1f);
        Vector3 nextPosition = transform.position + (movement * moveSpeed * Time.deltaTime);
        nextPosition.z = _fixedZPosition;

        transform.position = nextPosition;
    }
}
