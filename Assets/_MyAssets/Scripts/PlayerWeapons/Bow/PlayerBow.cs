using UnityEngine;

public class PlayerBow : WeaponBaseDamage
{
    [Header("Attaque")]
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private float _arrowSpeed = 20f;
    [SerializeField] private int _weaponLevel = 1;

    [Header("Sprites arc")]
    [SerializeField] private SpriteRenderer _bowRenderer;
    [SerializeField] private Sprite _bowSprite1;
    [SerializeField] private Sprite _bowSprite2;

    [Header("Sprites flèches")]
    [SerializeField] private Sprite _arrowLevel1Sprite;
    [SerializeField] private Sprite _arrowLevel2Sprite;
    [SerializeField] private Sprite _arrowLevel3Sprite;

    [SerializeField] private GameObject _arrowPrefab;

    private float _shootSpriteDuration = 0.1f;
    private Vector2 _lastLookDirection = Vector2.right;
    private Vector3 _lastPlayerPosition;
    private Transform _player;
    private float _nextFireTime;
    private Vector2 _currentDirection = Vector2.right;

    private void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (_bowRenderer != null)
            _bowRenderer.sprite = _bowSprite1;

        if (target != null)
        {
            _player = target.transform;
            transform.SetParent(_player);
            _lastPlayerPosition = _player.position;
        }
    }

    private void Update()
    {
        if (_player == null) return;
        _currentDirection = GetLookDirection();
        FollowPlayer();
        if (Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + _attackCooldown;
        }
    }

    private void Shoot()
    {
        SoundManager.Instance?.PlayBow();
        _bowRenderer.sprite = _bowSprite2;
        CancelInvoke(nameof(ResetBowSprite));
        Invoke(nameof(ResetBowSprite), _shootSpriteDuration);

        if (_weaponLevel == 1)
            SpawnArrow(_currentDirection);
        else if (_weaponLevel == 2)
        {
            SpawnArrow(_currentDirection);
            SpawnArrow(RotateDirection(_currentDirection, 20f));
            SpawnArrow(RotateDirection(_currentDirection, -20f));
        }
        else
        {
            SpawnArrow(_currentDirection);
            SpawnArrow(RotateDirection(_currentDirection, 15f));
            SpawnArrow(RotateDirection(_currentDirection, -15f));
            SpawnArrow(RotateDirection(_currentDirection, 30f));
            SpawnArrow(RotateDirection(_currentDirection, -30f));
        }
    }

    private void SpawnArrow(Vector2 direction)
    {
        Vector3 spawnPos = _player.position + (Vector3)(direction.normalized * 1.5f);
        GameObject arrowObj = Instantiate(_arrowPrefab, spawnPos, Quaternion.identity);

        SpriteRenderer arrowRenderer = arrowObj.GetComponentInChildren<SpriteRenderer>();
        if (arrowRenderer != null)
        {
            arrowRenderer.sprite = _weaponLevel switch
            {
                1 => _arrowLevel1Sprite,
                2 => _arrowLevel2Sprite,
                _ => _arrowLevel3Sprite
            };
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowObj.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = arrowObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = direction.normalized * _arrowSpeed;
        }

        // Flèche hérite des dégâts de l'arc
        if (arrowObj.TryGetComponent<Arrow>(out var arrow))
            arrow.SetDamage(Damage);
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
        transform.localPosition = _currentDirection.normalized * 0.7f;
        float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private Vector2 GetLookDirection()
    {
        Vector2 movementDirection = _player.position - _lastPlayerPosition;
        if (movementDirection.magnitude > 0.01f)
            _lastLookDirection = movementDirection.normalized;
        _lastPlayerPosition = _player.position;
        return _lastLookDirection;
    }

    public void SetWeaponLevel(int level)
    {
        _weaponLevel = Mathf.Clamp(level, 1, 3);
      
    }

    private void ResetBowSprite()
    {
        if (_bowRenderer != null)
            _bowRenderer.sprite = _bowSprite1;
    }
}