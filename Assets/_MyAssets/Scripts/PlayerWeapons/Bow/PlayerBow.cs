using UnityEngine;

public class PlayerBow : MonoBehaviour
{
    [Header("Attaque")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float arrowSpeed = 20f;

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int weaponLevel = 1;

    private Vector2 lastLookDirection = Vector2.right;
    private Vector3 lastPlayerPosition;
    private Transform player;
    private float nextFireTime;

    private void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");

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

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + attackCooldown;
        }
    }

    private void Shoot()
    {
        Vector2 baseDirection = GetLookDirection();

        if (weaponLevel == 1)
        {
            ShootArrow(baseDirection);
        }
        else if (weaponLevel == 2)
        {
            ShootArrow(baseDirection);
            ShootArrow(RotateDirection(baseDirection, 20f));
            ShootArrow(RotateDirection(baseDirection, -20f));
        }
        else if (weaponLevel >= 3)
        {
            ShootArrow(baseDirection);
            ShootArrow(RotateDirection(baseDirection, 15f));
            ShootArrow(RotateDirection(baseDirection, -15f));
            ShootArrow(RotateDirection(baseDirection, 30f));
            ShootArrow(RotateDirection(baseDirection, -30f));
        }
    }

    private void ShootArrow(Vector2 direction)
    {
        Vector3 spawnPos = transform.position + (Vector3)(direction.normalized * 1f);

        GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = direction.normalized * arrowSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private Vector2 RotateDirection(Vector2 direction, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;

        float x = direction.x * Mathf.Cos(rad) - direction.y * Mathf.Sin(rad);
        float y = direction.x * Mathf.Sin(rad) + direction.y * Mathf.Cos(rad);

        return new Vector2(x, y).normalized;
    }

    private void FollowPlayer()
    {
        Vector2 direction = GetLookDirection();

        transform.localPosition = direction.normalized * 0.7f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
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
        weaponLevel = level;
    }
}
