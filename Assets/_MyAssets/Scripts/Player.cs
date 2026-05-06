using System;
using Unity.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Propriétés Joueur")]
    [SerializeField] private float _playerSpeed = 10f;
    [SerializeField] private float _playerDashForce = 25f;
    [SerializeField] private float _playerDashRate = 0.5f;
    [SerializeField] private int _playerDashDuration = 10;

    [Header("Limites de la map")]
    [SerializeField] private GameObject _background;

    [Header("Son")]
    [SerializeField] private AudioClip _walkingSound;
    [SerializeField, Range(0f, 1f)] private float _walkingVolume = 0.5f;
    [SerializeField] private float _stepInterval = 0.35f;
    [SerializeField] private AudioClip _dashSound;
    [SerializeField, Range(0f, 1f)] private float _dashVolume = 0.7f;

    private float _minX, _maxX, _minY, _maxY;

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

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;

        // Liaison avec les input actions
        _inputSystemActions = new InputSystem_Actions();
        _inputSystemActions.Player.Enable();

        _rigidbody2D = GetComponent<Rigidbody2D>();

        _anim = GetComponent<Animator>();

        // Permet de calculer la largeur et hauteur de mon joueur
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _halfPlayerWidth = _spriteRenderer.bounds.extents.x;
        _halfPlayerHeight = _spriteRenderer.bounds.extents.y;


        // Calcule les limites automatiquement depuis le Background
        SpriteRenderer backgroundRenderer = _background.GetComponent<SpriteRenderer>();
        _minX = backgroundRenderer.bounds.min.x;
        _maxX = backgroundRenderer.bounds.max.x;
        _minY = backgroundRenderer.bounds.min.y;
        _maxY = backgroundRenderer.bounds.max.y;

        // Évènement du dash
        _inputSystemActions.Player.Dash.started += _ => _isDashing = true;
        _inputSystemActions.Player.Dash.canceled += _ => _isDashing = false;
        _isDashing = false;

        _lookingDirection = 1;
    }

    private void OnDestroy()
    {
        _inputSystemActions.Player.Disable();

        _inputSystemActions.Player.Dash.started -= _ => _isDashing = true;
        _inputSystemActions.Player.Dash.canceled -= _ => _isDashing = false;
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector2 direction2D = _inputSystemActions.Player.Move.ReadValue<Vector2>();
        _lookingDirection = direction2D.x != 0 ? direction2D.x : _lookingDirection;
        direction2D.Normalize();



        // === VITESSE DE DÉPLACEMENT === //
        float speedMultiplier;
        // Quand le joueur dash, initialise le dash et augmente la vitesse
        if (_isDashing && _deltaDash < Time.time && _deltaDashDuration == 0f)
        {
            speedMultiplier = _playerDashForce;
            _deltaDashDuration += _playerDashDuration;
            _deltaDash = Time.time + _playerDashRate + _playerDashDuration / 60;

            // Joue le son de dash
            if (_dashSound != null)
            {
                _audioSource.pitch = 1f; // Reset le pitch (au cas où il a été modifié par les pas)
                _audioSource.PlayOneShot(_dashSound, _dashVolume);
            }
        }
        // Si un dash est en cours, réduit son temps restant et augmente la vitesse
        else if (_deltaDashDuration > 0)
        {
            _deltaDashDuration--;
            speedMultiplier = _playerDashForce;
        }
        // Sinon, garde la vitesse normale
        else speedMultiplier = _playerSpeed;
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

        float clampedX = Mathf.Clamp(newPosition.x, _minX + _halfPlayerWidth, _maxX - _halfPlayerWidth);
        float clampedY = Mathf.Clamp(newPosition.y, _minY + _halfPlayerHeight, _maxY - _halfPlayerHeight);

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
            // Reset pour que le premier pas joue immédiatement quand on recommence à bouger
            _stepTimer = 0f;
        }
    }
}