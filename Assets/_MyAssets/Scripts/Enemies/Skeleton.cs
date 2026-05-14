using UnityEngine;

public class Skeleton : EnemyBase
{
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private GameObject _attackHitbox;

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

        float distance = Vector2.Distance(transform.position, _player.position);

        //
        float xDiff = _player.position.x - transform.position.x;
        _lookingDirection = xDiff != 0f ? xDiff : _lookingDirection;
        //

        bool inRange = distance <= _attackRange;
        if (inRange || _isAttacking)
            StopAndAttack();
        else
            MoveTowardPlayer();
        SetAnims(inRange);
    }


    private void StopAndAttack()
    {
        _isAttacking = Time.time < _nextAttackTime;
        if (Time.time >= _nextAttackTime)
        {
            Attack();
            _nextAttackTime = Time.time + _attackCooldown;
        }
    }

    private void Attack()
    {
        Vector2 direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        Vector3 spawnPos = transform.position + (Vector3)(direction * 2.5f);

        //Instantiate(_attackHitbox, spawnPos, Quaternion.identity);

        Debug.Log($"[Skeleton] Attaque le joueur � direction: {direction}");
    }



    private void SetAnims(bool inRange)
    {
        if (inRange)
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
        } else
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
}