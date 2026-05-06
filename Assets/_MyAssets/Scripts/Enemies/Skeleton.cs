using UnityEngine;

public class Skeleton : MonoBehaviour
{
    private Transform player;

    [SerializeField] private float moveSpeed = 1.5f;

    private void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
            player = target.transform;
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
