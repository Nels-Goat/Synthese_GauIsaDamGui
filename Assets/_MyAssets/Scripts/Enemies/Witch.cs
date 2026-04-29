using UnityEngine;

public class Witch : MonoBehaviour
{
    [SerializeField] private GameObject skeletonPrefab;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float summonRange = 10f;
    [SerializeField] private float summonCooldown = 4f;

    [SerializeField] private float spawnDistance = 1.5f;

    private float nextSummonTime;
    private Transform player;

    private void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
            player = target.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > summonRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            SummonSkeletons();
        }
    }

    private void MoveTowardPlayer()
    {
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;

        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void SummonSkeletons()
    {
        if (Time.time < nextSummonTime) return;

        Vector3 spawnPos1 = transform.position + Vector3.left * spawnDistance;
        Vector3 spawnPos2 = transform.position + Vector3.right * spawnDistance;

        GameObject skel1 = Instantiate(skeletonPrefab, spawnPos1, Quaternion.identity);
        GameObject skel2 = Instantiate(skeletonPrefab, spawnPos2, Quaternion.identity);

        skel1.GetComponent<Skeleton>().SetPlayer(player);
        skel2.GetComponent<Skeleton>().SetPlayer(player);

        nextSummonTime = Time.time + summonCooldown;
    }
}
