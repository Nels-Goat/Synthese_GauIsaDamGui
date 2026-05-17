using UnityEngine;

public class Skeleton : EnemyBase
{
    [Header("Attaque Skeleton")]
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _attackDuration = 1f;
    [SerializeField] private GameObject _enemyHitBox;
    [SerializeField] private float _enemyHitBoxOffset = 1f;

    private float _lookingDirection;
    private Animator _anim;

    private float _nextAttackTime;
    private bool _isAttacking = false;

    protected override void Start()
    {
        base.Start();

        _anim = GetComponent<Animator>();
        _lookingDirection = 1;
    }

    private void Update()
    {
        if (_player == null) return;
        MoveTowardPlayer();

        float distance = Vector2.Distance(transform.position, _player.position);

        //
        float xDiff = _player.position.x - transform.position.x;
        _lookingDirection = xDiff != 0f ? xDiff : _lookingDirection;
        //

        bool inRange = distance <= _attackRange;
        if (inRange)
            StopAndAttack();
        else
            MoveTowardPlayer();

        SetAnims(inRange);
    }


    private void StopAndAttack()
    {
        if (Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + _attackCooldown;
            _isAttacking = true;
            
            Vector2 direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;
            Vector3 spawnPos = transform.position + (Vector3)(direction * _enemyHitBoxOffset);

            GameObject hitBox = Instantiate(_enemyHitBox, spawnPos, Quaternion.identity);
            hitBox.GetComponent<SkeletonSword>().Damage = this._damage;
            Destroy(hitBox, _attackDuration);

            Debug.Log($"[Skeleton] Attaque vers le joueur � direction: {direction}");

        }
    }



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
        else if (inRange)
        {
            if (_lookingDirection > 0f)
            {
                _anim.SetBool("_idling", true);
                _anim.SetBool("_attacking", false);
                _anim.SetBool("_turningRight", false);
                _anim.SetBool("_turningLeft", false);
            } else
            {
                _anim.SetBool("_idling", true);
                _anim.SetBool("_attacking", false);
                _anim.SetBool("_turningLeft", false);
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
        HandleCollision(collision);
    }

    protected override void PlayDeathSound()
        => SoundManager.Instance?.PlaySkeletonDie();
}