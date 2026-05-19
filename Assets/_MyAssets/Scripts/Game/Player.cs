using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Propriétés Joueur")]
    [SerializeField] private float _playerLife = 10f;
    [SerializeField] private float _playerMaxLife = 10f;
    [SerializeField] private float _playerSpeed = 10f;
    [SerializeField] private float _playerDashForce = 25f;
    [SerializeField] private float _playerDashRate = 2.5f;
    [SerializeField] private int _playerDashDuration = 10;
    [SerializeField] private float _bumpingForce = 4f;

    public float PlayerLife {get => _playerLife; set => _playerLife = value;}
    public float PlayerMaxLife {get => _playerMaxLife; set => _playerMaxLife = value;}
    public float PlayerDashRate {get => _playerDashRate; set => _playerDashRate = value;}

    [Header("Invincibilité")]
    [SerializeField] private float _iFramesDuration = 1f;
    [SerializeField] private int _iFramesFlashCount = 6;

    public float IFramesDuration {get => _iFramesDuration; set => _iFramesDuration = value;}

    [Header("Son - Marche")]
    [SerializeField] private float _stepInterval = 0.35f;

    private InputSystem_Actions _inputSystemActions;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _anim;

    private float _halfPlayerWidth;
    private float _halfPlayerHeight;

    private bool _isDashing;
    private float _deltaDash;
    private float _deltaDashDuration;

    private float _lookingDirection;
    private float _stepTimer;
    private bool _isInvincible = false;

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyDestroyed += OnEnemyDestroyed;

        _inputSystemActions = new InputSystem_Actions();
        _inputSystemActions.Player.Enable();

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _halfPlayerWidth = _spriteRenderer.bounds.extents.x;
        _halfPlayerHeight = _spriteRenderer.bounds.extents.y;

        _inputSystemActions.Player.Dash.started += _ => _isDashing = true;
        _inputSystemActions.Player.Dash.canceled += _ => _isDashing = false;
        _isDashing = false;

        _lookingDirection = 1;

        Debug.Log($"[Player] Initialisé — Vie : {_playerLife}/{_playerMaxLife}");
    }

    private void OnDestroy()
    {
 
        _inputSystemActions.Player.Dash.started -= _ => _isDashing = true;
        _inputSystemActions.Player.Dash.canceled -= _ => _isDashing = false;

        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyDestroyed -= OnEnemyDestroyed;

        _inputSystemActions.Player.Disable();
        _inputSystemActions.Dispose();
    }

    // ===================== DÉGÂTS ===================== //

    private void OnEnemyDestroyed(object sender, GameManager.OnEnemyDestroyedEventArgs e)
    {
        if (e.DestroyedObjectTag == "Player")
            TakeDamage(e.Damage);
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        _playerLife -= damage;
        Debug.Log($"[Player] -{damage} vie | Vie restante : {_playerLife}/{_playerMaxLife}");

        UIGame.Instance?.UpdateLifeBar();

        if (_playerLife <= 0)
        {
            Die();
            return;
        }

        SoundManager.Instance?.PlayPlayerGetHit();

        StartCoroutine(IFramesRoutine());
    }

    private IEnumerator IFramesRoutine()
    {
        _isInvincible = true;
        Debug.Log("[Player] Invincibilité activée.");

        float flashInterval = _iFramesDuration / (_iFramesFlashCount * 2);

        for (int i = 0; i < _iFramesFlashCount; i++)
        {
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(flashInterval);
        }

        _isInvincible = false;
        Debug.Log("[Player] Invincibilité terminée.");
    }

    private void Die()
    {
        Debug.Log("[Player] Le joueur est mort — Game Over.");
        SpawnManager.Instance?.StopSpawning();
        GameManager.Instance?.EndGame();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Got Touched by: " + collision.tag);
        if (collision.CompareTag("EnemyAttack"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            int damage = enemy != null ? enemy.Damage : 1;

            Debug.Log("[Player] Touché par EnemyAttack ! Dmg: " + damage);
            TakeDamage(damage);
            TriggerBumping(collision.transform);

            try { collision.tag = "Enemy"; } catch (Exception) { }
        }

        if (collision.CompareTag("EnemyProjectile"))
        {
            EnemyProjectile projectile = collision.GetComponent<EnemyProjectile>();
            int damage = projectile != null ? projectile.Damage : 1;
            Debug.Log("[Player] Touché par EnemyProjectile !");
            TakeDamage(damage);
            
            TriggerBumping(collision.transform);

            Destroy(collision.gameObject);
        }
    }

    private void TriggerBumping(Transform bumper)
    {
        if (_isInvincible) return;

        Vector3 diffPos = (gameObject.transform.position - bumper.position).normalized;
        gameObject.transform.Translate(diffPos * _bumpingForce);
    }

    // ================================================== //

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[Player] GameManager.Instance est null !");
            return;
        }

        Vector2 direction2D = _inputSystemActions.Player.Move.ReadValue<Vector2>();
        _lookingDirection = direction2D.x != 0 ? direction2D.x : _lookingDirection;
        direction2D.Normalize();

        // === VITESSE DE DÉPLACEMENT === //
        float speedMultiplier;

        if (_isDashing && _deltaDash < Time.time && _deltaDashDuration == 0f)
        {
            speedMultiplier = _playerDashForce;
            _deltaDashDuration += _playerDashDuration;
            _deltaDash = Time.time + _playerDashRate + _playerDashDuration / 60;

            SoundManager.Instance?.PlayDash();
        }
        else if (_deltaDashDuration > 0)
        {
            _deltaDashDuration--;
            speedMultiplier = _playerDashForce;
        }
        else
            speedMultiplier = _playerSpeed;
        // ============================== //

        // === ANIMATION === //
        if (_isDashing && _deltaDashDuration > 0)
        {
            _anim.SetBool("_dashing", true);
            _anim.SetBool("_turningRight", false);
            _anim.SetBool("_turningLeft", false);
        }
        else if (_deltaDashDuration > 0)
        {
            if (_lookingDirection > 0)
            {
                _anim.SetBool("_turningRight", true);
                _anim.SetBool("_turningLeft", false);
            }
            else
            {
                _anim.SetBool("_turningLeft", true);
                _anim.SetBool("_turningRight", false);
            }
        }
        else if (direction2D.x == 0f && direction2D.y == 0f)
        {
            _anim.SetBool("_turningRight", false);
            _anim.SetBool("_turningLeft", false);
            _anim.SetBool("_dashing", false);
        }
        else if (_lookingDirection > 0f)
        {
            _anim.SetBool("_turningRight", true);
            _anim.SetBool("_turningLeft", false);
            _anim.SetBool("_dashing", false);
        }
        else if (_lookingDirection < 0f)
        {
            _anim.SetBool("_turningLeft", true);
            _anim.SetBool("_turningRight", false);
            _anim.SetBool("_dashing", false);
        }
        // ================= //

        // === SON DE MARCHE === //
        bool isMoving = (direction2D.x != 0f || direction2D.y != 0f) && _deltaDashDuration == 0;
        HandleWalkingSound(isMoving);
        // ===================== //

        Vector2 newPosition = _rigidbody2D.position + direction2D * Time.fixedDeltaTime * speedMultiplier;

        float clampedX = GameManager.Instance.ClampX(newPosition.x, _halfPlayerWidth);
        float clampedY = GameManager.Instance.ClampY(newPosition.y, _halfPlayerHeight);

        _rigidbody2D.MovePosition(new Vector2(clampedX, clampedY));
    }

    private void HandleWalkingSound(bool isMoving)
    {
        if (!isMoving) { _stepTimer = 0f; return; }

        _stepTimer -= Time.fixedDeltaTime;
        if (_stepTimer <= 0f)
        {
            SoundManager.Instance?.PlayFootstep();
            _stepTimer = _stepInterval;
        }
    }
}