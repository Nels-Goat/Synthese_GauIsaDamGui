using System;
using UnityEngine;

public class Troll : EnemyBase
{
    [Header("Attaque Troll")]
    [SerializeField] private float _chargeSpeed = 12f;
    [SerializeField] private float _chargeForce = 5f;
    [SerializeField] private float _chargeCooldown = 5f;
    [SerializeField] private float _chargeMaxDistance = 15f;
    [SerializeField] private float _detectionRange = 6f;


    private bool _isCharging = false;
    private Vector2 _preChargePos;
    private float _nextChargeTime;
    private Vector2 _chargeDirection;

    private float _lookingDirection;
    private bool _isAttacking = false;
    private Animator _anim;

    protected override void Start()
    {
        base.Start();

        _anim = GetComponent<Animator>();
        _lookingDirection = 1;
    }

    private void Update()
    {
        TrollMovement();
    }

    private void TrollMovement()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);

        //
        float xDiff = _player.position.x - transform.position.x;
        _lookingDirection = xDiff != 0f ? xDiff : _lookingDirection;
        //

        bool inRange = distance <= _detectionRange;

        if (!_isCharging)
        {
            if (inRange)
            {
                Charge();
            }
            else
                MoveTowardPlayer();
        }
        else
        {
            float distanceFromPreCharge = Vector2.Distance(transform.position, _preChargePos);
            if (distanceFromPreCharge >= _chargeMaxDistance)
            {
                _isCharging = false;
                gameObject.tag = "Enemy"; // ← remet le tag après la charge
            }
            else
                transform.Translate(_chargeDirection * _chargeSpeed * Time.deltaTime);
        }

        SetAnims(inRange);
        //HandleScreenWrap();
    }

    private void Charge()
    {
        if (Time.time < _nextChargeTime) return;

        SoundManager.Instance?.PlayTrollDash();

        _isCharging = true;
        _isAttacking = true;
        _preChargePos = transform.position;
        _chargeDirection = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        _nextChargeTime = Time.time + _chargeCooldown;

        gameObject.tag = "EnemyAttack";
        Debug.Log("[Troll] Charge d�clench�e !");
    }


    /*
    private void HandleScreenWrap()
    {
        if (transform.position.y < -Camera.main.orthographicSize - 1f)
        {
            float randomX = UnityEngine.Random.Range(
                -Camera.main.orthographicSize * Camera.main.aspect + _halfWidth,
                 Camera.main.orthographicSize * Camera.main.aspect - _halfWidth
            );
            transform.position = new Vector3(randomX, Camera.main.orthographicSize + 2f, 0f);
            _isCharging = false;
            gameObject.tag = "Enemy";
            Debug.Log("[Troll] Reset apr�s sortie d'�cran.");
        }
    }*/

    private void SetAnims(bool inRange)
    {
        if (_isAttacking)
        {
            if (_lookingDirection > 0f)
            {
                _anim.SetBool("_idling", false);
                _anim.SetBool("_attacking", true);
                _anim.SetBool("_turningLeft", false);
                _anim.SetBool("_turningRight", true);
            } else
            {
                _anim.SetBool("_idling", false);
                _anim.SetBool("_attacking", true);
                _anim.SetBool("_turningLeft", true);
                _anim.SetBool("_turningRight", false);
            }
            _isAttacking = false;
        }
        else if (_isCharging)
        {
            if (_chargeDirection.x > 0f)
            {
                _anim.SetBool("_idling", true);
                _anim.SetBool("_attacking", false);
                _anim.SetBool("_turningLeft", false);
                _anim.SetBool("_turningRight", true);
            } else
            {
                _anim.SetBool("_idling", true);
                _anim.SetBool("_attacking", false);
                _anim.SetBool("_turningLeft", true);
                _anim.SetBool("_turningRight", false);
            }
        }
        else if (inRange)
        {
            if (_lookingDirection > 0f)
            {
                _anim.SetBool("_idling", true);
                _anim.SetBool("_attacking", false);
                _anim.SetBool("_turningLeft", false);
                _anim.SetBool("_turningRight", true);
            } else
            {
                _anim.SetBool("_idling", true);
                _anim.SetBool("_attacking", false);
                _anim.SetBool("_turningLeft", true);
                _anim.SetBool("_turningRight", false);
            }
        }
        else
        {
            if (_lookingDirection > 0f)
            {
                _anim.SetBool("_idling", false);
                _anim.SetBool("_attacking", false);
                _anim.SetBool("_turningRight", true);
                _anim.SetBool("_turningLeft", false);
            } else
            {
                _anim.SetBool("_idling", false);
                _anim.SetBool("_attacking", false);
                _anim.SetBool("_turningLeft", true);
                _anim.SetBool("_turningRight", false);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            Destroy(collision.gameObject);
            TakeHit("PlayerAttack"); // fonctionne charge ou pas
        }
        else if (collision.CompareTag("Player") && _isCharging)
        {
            _isCharging = false;
            gameObject.tag = "Enemy";
            collision.transform.Translate(_chargeDirection * _chargeForce);
        }
    }

    protected override void PlayDeathSound()
        => SoundManager.Instance?.PlayTrollDie();
}