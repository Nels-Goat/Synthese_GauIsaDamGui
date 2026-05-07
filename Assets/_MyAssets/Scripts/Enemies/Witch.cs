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
    private GameObject _skeletonContainer;
    private float _halfSkeletonWidth;
    private float _halfSkeletonHeight;

    protected override void Start()
    {
        base.Start();

        _skeletonContainer = GameObject.FindGameObjectWithTag("EnemyContainer");

        SpriteRenderer skelRenderer = _skeletonPrefab.GetComponent<SpriteRenderer>();
        _halfSkeletonWidth = skelRenderer.bounds.extents.x;
        _halfSkeletonHeight = skelRenderer.bounds.extents.y;
    }

    private void Update()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);

        if (distance > _summonRange)
            MoveTowardPlayer();
        else
            SummonSkeletons();
    }

    private void SummonSkeletons()
    {
        if (Time.time < _nextSummonTime) return;

        float angleStep = (float)Mathf.PI * 2 / _numSkeleton;

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision);
    }
}