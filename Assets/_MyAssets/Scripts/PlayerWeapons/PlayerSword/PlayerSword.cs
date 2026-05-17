using UnityEngine;
using System.Collections;

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

    [Header("Transform Level 1")]
    [SerializeField] private Vector3 level1Scale = new Vector3(0.2f, 0.2f, 1f);
    [SerializeField] private Vector3 level1Rotation = new Vector3(0f, 0f, -15f);

    [Header("Transform Level 2")]
    [SerializeField] private Vector3 level2Scale = new Vector3(0.2f, 0.2f, 1f);
    [SerializeField] private Vector3 level2Rotation = new Vector3(0f, 0f, -54f);

    [Header("Transform Level 3")]
    [SerializeField] private Vector3 level3Scale = new Vector3(0.2f, 0.2f, 1f);
    [SerializeField] private Vector3 level3Rotation = new Vector3(0f, 0f, -40f);

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

        if (weaponLevel == 1)
            visualAnimator.Play("SwordAttackLvl1", 0, 0f);
        else if (weaponLevel == 2)
            visualAnimator.Play("SwordAttackLvl2", 0, 0f);
        else if (weaponLevel == 3)
            visualAnimator.Play("SwordAttackLvl3", 0, 0f);
        StopCoroutine(nameof(ResetSwordAfterAnimation));
        StartCoroutine(nameof(ResetSwordAfterAnimation));

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

        SpriteRenderer sr = aoe.GetComponentInChildren<SpriteRenderer>();
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
        {
            lastLookDirection = movementDirection.normalized;
        }

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
            visual.localScale = level1Scale;
            visual.localRotation = Quaternion.Euler(level1Rotation);
        }
        else if (weaponLevel == 2)
        {
            visualRenderer.sprite = level2Sprite;
            visual.localScale = level2Scale;
            visual.localRotation = Quaternion.Euler(level2Rotation);
        }
        else if (weaponLevel == 3)
        {
            visualRenderer.sprite = level3Sprite;
            visual.localScale = level3Scale;
            visual.localRotation = Quaternion.Euler(level3Rotation);
        }
    }
    private IEnumerator ResetSwordAfterAnimation()
    {
        yield return new WaitForSeconds(0.12f); 
        UpdateSprite();
    }
}
