using UnityEngine;

public class PlayerBow : MonoBehaviour
{
    [Header("Attaque")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float arrowSpeed = 20f;


    [Header("Sprites arc")]
    [SerializeField] private SpriteRenderer bowRenderer;
    [SerializeField] private Sprite bowSprite1;
    [SerializeField] private Sprite bowSprite2;

    [Header("Sprites flèches")]
    [SerializeField] private Sprite arrowLevel1Sprite;
    [SerializeField] private Sprite arrowLevel2Sprite;
    [SerializeField] private Sprite arrowLevel3Sprite;

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int weaponLevel = 1;

    private float shootSpriteDuration = 0.1f;
    private Vector2 lastLookDirection = Vector2.right;
    private Vector3 lastPlayerPosition;
    private Transform player;
    private float nextFireTime;
    private Vector2 currentDirection = Vector2.right;


    private void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (bowRenderer != null)
            bowRenderer.sprite = bowSprite1;

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

        currentDirection = GetLookDirection();

        FollowPlayer();

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + attackCooldown;
        }
    }

    private void Shoot()
    {
        Vector2 baseDirection = currentDirection;

        bowRenderer.sprite = bowSprite2;
        CancelInvoke(nameof(ResetBowSprite));
        Invoke(nameof(ResetBowSprite), shootSpriteDuration);


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
        Vector3 spawnPos = player.position + (Vector3)(direction.normalized * 1.5f);

        GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        SpriteRenderer arrowRenderer = arrow.GetComponentInChildren<SpriteRenderer>();

        if (arrowRenderer != null)
        {
            if (weaponLevel == 1)
                arrowRenderer.sprite = arrowLevel1Sprite;
            else if (weaponLevel == 2)
                arrowRenderer.sprite = arrowLevel2Sprite;
            else
                arrowRenderer.sprite = arrowLevel3Sprite;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = direction.normalized * arrowSpeed;
        }
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
        Vector2 direction = currentDirection;

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
        if (level > 3) { level = 3; }
        weaponLevel = level;
    }

    private void ResetBowSprite()
    {
        if (bowRenderer != null)
            bowRenderer.sprite = bowSprite1;
    }
}
