using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Witch : MonoBehaviour
{
    [SerializeField] private GameObject _skeletonPrefab;

    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _summonRange = 10f;
    [SerializeField] private float _summonCooldown = 4f;
    [SerializeField] private int _numSkeleton = 2;

    [SerializeField] private float _spawnRadius = 1.5f;

    private float _nextSummonTime;
    private Transform _player;

    private GameObject _skeletonContainer;

    private SpriteRenderer _spriteRenderer;
    private float _halfSkeletonWidth;
    private float _halfSkeletonHeight;

    private void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
            _player = target.transform;

        _skeletonContainer = GameObject.FindGameObjectWithTag("EnemyContainer");

        _spriteRenderer = _skeletonPrefab.GetComponent<SpriteRenderer>();
        _halfSkeletonWidth = _spriteRenderer.bounds.extents.x;
        _halfSkeletonHeight = _spriteRenderer.bounds.extents.y;
    }

    private void Update()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);

        if (distance > _summonRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            SummonSkeletons();
        }
    }

    private void MoveTowardPlayer()
    {
        Vector2 direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;

        transform.position += (Vector3)(direction * _moveSpeed * Time.deltaTime);

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void SummonSkeletons()
    {
        if (Time.time < _nextSummonTime) return;

        float angle = (float)Mathf.PI*2 / _numSkeleton;
        for (int i = 0; i < _numSkeleton; i++)
        {
            if (GameManager.Instance.IsEnemyMaxed()) break;

            float randomX = (float)Math.Cos(i * angle) * _spawnRadius + gameObject.transform.position.x;
            float randomY = (float)Math.Sin(i * angle) * _spawnRadius + gameObject.transform.position.y;

            float clampedX = GameManager.Instance.ClampX(randomX, _halfSkeletonWidth);
            float clampedY = GameManager.Instance.ClampY(randomY, _halfSkeletonHeight);

            Vector3 spawnPosition = new Vector3(clampedX, clampedY, 0f);
            GameObject skel = Instantiate(_skeletonPrefab, spawnPosition, Quaternion.identity);
            skel.transform.parent = _skeletonContainer.transform;
        } 

        _nextSummonTime = Time.time + _summonCooldown;
    }
}
