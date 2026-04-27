using UnityEngine;

public class Skeleton : MonoBehaviour
{
    private Transform player;

    [SerializeField] private float moveSpeed = 2.5f;

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    private void Update()
    {
        if (player == null) return;

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;

        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
