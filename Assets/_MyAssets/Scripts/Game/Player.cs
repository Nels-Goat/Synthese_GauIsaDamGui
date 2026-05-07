using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Propriétés Joueur")]
    [SerializeField] private float _playerLife = 10f;
    [SerializeField] private float _playerSpeed = 10f;
    [SerializeField] private float _playerDashForce = 25f;
    [SerializeField] private float _playerDashRate = 0.5f;
    [SerializeField] private int _playerDashDuration = 10;

    [Header("Son")]
    [SerializeField] private AudioClip _walkingSound;
    [SerializeField, Range(0f, 1f)] private float _walkingVolume = 0.5f;
    [SerializeField] private float _stepInterval = 0.35f;
    [SerializeField] private AudioClip _dashSound;
    [SerializeField, Range(0f, 1f)] private float _dashVolume = 0.7f;

    //private int _exp = 0;
    //private int _level = 0;

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


    private void Awake()
    {
        GameManager.Instance.OnEnemyDestroyed += OnEnemyDestroyed;
    }

    private void Start()
    {
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
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnEnemyDestroyed -= OnEnemyDestroyed;

        _inputSystemActions.Player.Disable();
        _inputSystemActions.Player.Dash.started -= _ => _isDashing = true;
        _inputSystemActions.Player.Dash.canceled -= _ => _isDashing = false;
    }


    // ===================== DÉGÂTS ===================== //

    private void OnEnemyDestroyed(object sender, GameManager.OnEnemyDestroyedEventArgs e)
    {
        if (e.DestroyedObjectTag == "Player")
            TakeDamage();
    }

    // Appelé par OnEnemyDestroyed (lance du goblin, skeleton, witch)
    // et par OnTriggerEnter2D (troll en charge, attaques directes)
    private void TakeDamage()
    {
        _playerLife--;

        if (_playerLife <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
        // SpawnManager.Instance.StopSpawning();
        // GameManager.Instance.EndGame();
    }

    // Détecte le contact direct avec un ennemi en charge (EnemyAttack)
    // ou une attaque ennemie (lance du goblin)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            TakeDamage();
            Destroy(collision.gameObject); // Détruit la lance ou le troll en charge
        }
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
            Debug.LogError("GameManager.Instance est null !");
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