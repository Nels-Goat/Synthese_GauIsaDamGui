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
    [SerializeField] private float _playerDashRate = 0.5f;
    [SerializeField] private int _playerDashDuration = 10;
    [SerializeField] private float _bumpingForce = 1f;

    [Header("Invincibilité")]
    [SerializeField] private float _iFramesDuration = 1.5f;
    [SerializeField] private int _iFramesFlashCount = 6;

    [Header("Son")]
    [SerializeField] private AudioClip _walkingSound;
    [SerializeField, Range(0f, 1f)] private float _walkingVolume = 0.5f;
    [SerializeField] private float _stepInterval = 0.35f;
    [SerializeField] private AudioClip _dashSound;
    [SerializeField, Range(0f, 1f)] private float _dashVolume = 0.7f;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField, Range(0f, 1f)] private float _hitVolume = 0.8f;

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

    private AudioSource _audioSource;
    private float _stepTimer;

    private bool _isInvincible = false;


    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyDestroyed += OnEnemyDestroyed;
        }

        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;

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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyDestroyed -= OnEnemyDestroyed;
        }

        _inputSystemActions.Player.Disable();
        _inputSystemActions.Player.Dash.started -= _ => _isDashing = true;
        _inputSystemActions.Player.Dash.canceled -= _ => _isDashing = false;
    }


    // ===================== DÉGÂTS ===================== //

    private void OnEnemyDestroyed(object sender, GameManager.OnEnemyDestroyedEventArgs e)
    {
        if (e.DestroyedObjectTag == "Player")
            TakeDamage(e.Damage);
    }

    private void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        _playerLife -= damage;
        Debug.Log($"[Player] -{damage} vie | Vie restante : {_playerLife}/{_playerMaxLife}");

        UIGame.Instance?.UpdateLifeBar(_playerLife);

        if (_playerLife <= 0)
        {
            Die();
            return;
        }

        if (_hitSound != null)
            _audioSource.PlayOneShot(_hitSound, _hitVolume);

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

            Debug.Log("[Player] Touché par EnemyAttack ! Dmg: " + enemy.Damage);
            TakeDamage(enemy.Damage); // Dégât fixe pour les projectiles

            TriggerBumping(collision.transform);

            try {
                collision.tag = "Enemy";
            } catch (Exception) {}

            //Destroy(collision.gameObject);
        }

        if (collision.CompareTag("EnemyProjectile"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            Debug.Log("[Player] Touché par EnemyProjectile ! Dmg: " + enemy.Damage);
            TakeDamage(enemy.Damage); // Dégât fixe pour les projectiles
            
            TriggerBumping(collision.transform);

            Destroy(collision.gameObject);
        }
    }

    private void TriggerBumping(Transform bumper)
    {
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

            if (_dashSound != null)
            {
                _audioSource.pitch = 1f;
                _audioSource.PlayOneShot(_dashSound, _dashVolume);
            }
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
        if (isMoving && _walkingSound != null)
        {
            _stepTimer -= Time.fixedDeltaTime;
            if (_stepTimer <= 0f)
            {
                _audioSource.PlayOneShot(_walkingSound, _walkingVolume);
                _stepTimer = _stepInterval;
            }
        }
        else
        {
            _stepTimer = 0f;
        }
    }
}