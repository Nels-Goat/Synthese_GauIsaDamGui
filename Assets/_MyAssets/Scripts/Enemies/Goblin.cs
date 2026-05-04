using UnityEngine;

public class Goblin : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject goblinSpear; 

    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackRange = 10f;

    [Header("Attaque")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float spearSpeed = 20f;

    private float nextAttackTime;
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

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            StopAndAttack();
        }
    }

    private void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void StopAndAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Attack()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        Vector3 spawnPos = transform.position + (Vector3)(direction * 2.5f);

        GameObject spear = Instantiate(goblinSpear, spawnPos, Quaternion.identity);

        Rigidbody2D rb = spear.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * spearSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        spear.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

