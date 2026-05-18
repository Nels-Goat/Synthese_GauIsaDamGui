using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    [Header("Attaque")]
    [SerializeField] private float attackCooldown = 0.25f;
    [SerializeField] private GameObject swordAOEPrefab;
    [SerializeField] private int weaponLevel = 1;

    [Header("Sprites par niveau")]
    [SerializeField] private Sprite level1Sprite;
    [SerializeField] private Sprite level2Sprite;
    [SerializeField] private Sprite level3Sprite;

    [SerializeField] private Transform visual;
    [SerializeField] private Animator visualAnimator;
    [SerializeField] private SpriteRenderer visualRenderer;

    private SpriteRenderer sr;
    private Vector2 lastLookDirection = Vector2.right;
    private Vector3 lastPlayerPosition;
    private Transform player;
    private float nextAttackTime;
    private float baseAttackCD;

    private void Start()
    {
        UpdateSprite();
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

        UpdateSprite();


        visualAnimator.SetTrigger("Attack");
        SpawnAOE(direction);
    }

    private void SpawnAOE(Vector2 direction)
    {
        Vector3 spawnPos = player.position + (Vector3)(direction.normalized * 1.5f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject aoe = Instantiate(swordAOEPrefab, spawnPos, Quaternion.Euler(0, 0, angle));

        SpriteRenderer sr = aoe.GetComponentInChildren<SpriteRenderer>();
        float multiplier = 1f;
        if (weaponLevel == 2) multiplier = 1.5f;
        if (weaponLevel >= 3) multiplier = 2f;

        aoe.transform.localScale *= multiplier;
        attackCooldown = baseAttackCD / multiplier;
    }

    private void FollowPlayer()
    {
        Vector2 direction = GetLookDirection();
        transform.localPosition = direction.normalized * 0.7f;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.Euler(0, 0, angle);

        bool facingLeft = direction.x < 0;

        Vector3 visualScale = visual.localScale;

        if (facingLeft)
            visualScale.y = -Mathf.Abs(visualScale.y);
        else
            visualScale.y = Mathf.Abs(visualScale.y);

        visual.localScale = visualScale;
    }

    private Vector2 GetLookDirection()
    {
        Vector2 movementDirection = player.position - lastPlayerPosition;
        if (movementDirection.magnitude > 0.01f)
            lastLookDirection = movementDirection.normalized;

        lastPlayerPosition = player.position;
        return lastLookDirection;
    }

    public void SetWeaponLevel(int level)
    {
        if(level > 3) {level= 3;}
        if (level < 1) level = 1;
        weaponLevel = level;
        UpdateSprite(); 
    }

    private void UpdateSprite()
    {
        if (visualRenderer == null) return;

        if (weaponLevel == 1)
        {
            visualRenderer.sprite = level1Sprite;
        }
        else if (weaponLevel == 2)
        {
            visualRenderer.sprite = level2Sprite;
        }
        else if (weaponLevel == 3)
        {
            visualRenderer.sprite = level3Sprite;
        }
    }
}