using UnityEngine;

public class Movement2d : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.1f;
    private Vector3 moveDirection;

    public float MoveSpeed => moveSpeed;

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
