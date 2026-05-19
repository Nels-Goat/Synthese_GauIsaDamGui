using UnityEngine;

public class PlayerStaff : MonoBehaviour
{
    [Header("Attaque")]
    [SerializeField] private float attackCooldown = 3.5f;
    [SerializeField] private GameObject staffAOEPrefab;
    [SerializeField] private int weaponLevel = 1;

    [Header("Animation")]
    [SerializeField] private Animator staffAnimator;

    [Header("Sprites Staff")]
    [SerializeField] private SpriteRenderer staffRenderer;

    [SerializeField] private Sprite level1Sprite;
    [SerializeField] private Sprite level2Sprite;
    [SerializeField] private Sprite level3Sprite;

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
        Vector2 direction = GetLookDirection();

        SoundManager.Instance?.PlayStaffShoot();

        if (staffAnimator != null)
        {
            staffAnimator.ResetTrigger("Shoot");
            staffAnimator.SetTrigger("Shoot");
        }

        SpawnAOE(direction);
    }

    private void SpawnAOE(Vector2 direction)
    {
        Vector3 spawnPos = player.position + (Vector3)(direction.normalized * 3f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject aoe = Instantiate(staffAOEPrefab, spawnPos, Quaternion.Euler(0, 0, angle));

        bool facingLeft = direction.x < 0;

        Vector3 aoeScale = new Vector3(10f, 10f, 1f);

        if (weaponLevel == 2) aoeScale *= 1.5f;
        if (weaponLevel >= 3) aoeScale *= 2f;

        if (facingLeft)
            aoeScale.y = -Mathf.Abs(aoeScale.y);
        else
            aoeScale.y = Mathf.Abs(aoeScale.y);

        aoe.transform.localScale = aoeScale;
    }

    private void FollowPlayer()
    {
        Vector2 direction = GetLookDirection();

        transform.localPosition = direction.normalized * 0.7f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);

        bool facingLeft = direction.x < 0;

        Vector3 visualScale = staffRenderer.transform.localScale;

        if (facingLeft)
            visualScale.y = -Mathf.Abs(visualScale.y);
        else
            visualScale.y = Mathf.Abs(visualScale.y);

        staffRenderer.transform.localScale = visualScale;
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
        if (level > 3) level = 3;
        if (level < 1) level = 1;

        weaponLevel = level;

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (staffRenderer == null) return;

        if (weaponLevel == 1)
        {
            staffRenderer.sprite = level1Sprite;
        }
        else if (weaponLevel == 2)
        {
            staffRenderer.sprite = level2Sprite;
        }
        else
        {
            staffRenderer.sprite = level3Sprite;
        }
    }
}