using System;
using UnityEngine;

public class Witch : EnemyBase
{
    [Header("Invocation Witch")]
    [SerializeField] private GameObject _skeletonPrefab;
    [SerializeField] private float _summonRange = 10f;
    [SerializeField] private float _summonCooldown = 4f;
    [SerializeField] private int _numSkeleton = 2;
    [SerializeField] private float _spawnRadius = 1.5f;

    private float _nextSummonTime;
    private bool _isAttacking = false;

    private GameObject _skeletonContainer;
    private float _halfSkeletonWidth;
    private float _halfSkeletonHeight;

    private float _lookingDirection;
    private Animator _anim;

    protected override void Start()
    {
        base.Start();

        _anim = GetComponent<Animator>();
        _lookingDirection = 1;

        _skeletonContainer = GameObject.FindGameObjectWithTag("EnemyContainer");
        SpriteRenderer skelRenderer = _skeletonPrefab.GetComponent<SpriteRenderer>();
        _halfSkeletonWidth = skelRenderer.bounds.extents.x;
        _halfSkeletonHeight = skelRenderer.bounds.extents.y;
    }

    private void Update()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);

        //
        float xDiff = _player.position.x - transform.position.x;
        _lookingDirection = xDiff != 0f ? xDiff : _lookingDirection;
        //

        bool inRange = distance <= _summonRange;
        if (inRange)
            SummonSkeletons();
        else
            MoveTowardPlayer();

        SetAnims(inRange);
    }

    private void SummonSkeletons()
    {
        if (Time.time < _nextSummonTime) return;

        SoundManager.Instance?.PlayWitchSpawnSkeleton(); // Witch invoque des squelettes
        float angleStep = Mathf.PI * 2 / _numSkeleton;

        for (int i = 0; i < _numSkeleton; i++)
        {
            if (GameManager.Instance.IsEnemyMaxed()) break;

            float rawX = (float)Math.Cos(i * angleStep) * _spawnRadius + transform.position.x;
            float rawY = (float)Math.Sin(i * angleStep) * _spawnRadius + transform.position.y;
            float clampedX = GameManager.Instance.ClampX(rawX, _halfSkeletonWidth);
            float clampedY = GameManager.Instance.ClampY(rawY, _halfSkeletonHeight);

            GameObject skel = Instantiate(_skeletonPrefab, new Vector3(clampedX, clampedY, 0f), Quaternion.identity);
            skel.transform.parent = _skeletonContainer.transform;
        }

        _nextSummonTime = Time.time + _summonCooldown;
        _isAttacking = true;
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
                _anim.SetBool("_turningRight", true);
                _anim.SetBool("_turningLeft", false);
            } else
            {
                _anim.SetBool("_idling", true);
                _anim.SetBool("_attacking", false);
                _anim.SetBool("_turningLeft", true);
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

    protected override void PlayDeathSound()
        => SoundManager.Instance?.PlayWitchDie();
}