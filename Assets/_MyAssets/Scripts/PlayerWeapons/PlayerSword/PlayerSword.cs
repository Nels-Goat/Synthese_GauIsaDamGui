using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    [Header("Attaque")]
    [SerializeField] private float attackCooldown = 0.25f;

    [SerializeField] private GameObject swordAOEPrefab;
    [SerializeField] private int weaponLevel = 1;

    private Vector2 lastLookDirection = Vector2.right;
    private Vector3 lastPlayerPosition;
    private Transform player;
    private float nextAttackTime;
    private float baseAttackCD;

    private void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");

        baseAttackCD = attackCooldown;

        if (target != null)
        {
            player = target.transform;
            transform.SetParent(player);
            lastPlayerPosition = player.position;
        }
    }

    private void Update()
    {
        if (player == null) return;

        FollowPlayer();

        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Attack()
    {
        Vector2 direction = GetLookDirection();

        SpawnAOE(direction);
    }

    private void SpawnAOE(Vector2 direction)
    {
        Vector3 spawnPos = player.position + (Vector3)(direction.normalized * 1.5f);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject aoe = Instantiate(
            swordAOEPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, angle)
        );

        float multiplier = 1f;

        if (weaponLevel == 2) { multiplier = 1.5f; };
        if (weaponLevel >= 3) { multiplier = 2f; };

        aoe.transform.localScale *= multiplier;
        attackCooldown = baseAttackCD / multiplier;
        
    }

    private void FollowPlayer()
    {
        Vector2 direction = GetLookDirection();

        transform.localPosition = direction.normalized * 0.7f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private Vector2 GetLookDirection()
    {
        Vector2 movementDirection = player.position - lastPlayerPosition;

        if (movementDirection.magnitude > 0.01f)
        {
            lastLookDirection = movementDirection.normalized;
        }

        lastPlayerPosition = player.position;

        return lastLookDirection;
    }

    public void SetWeaponLevel(int level)
    {
        if(level > 3) {level= 3;}
        weaponLevel = level;
    }
}
