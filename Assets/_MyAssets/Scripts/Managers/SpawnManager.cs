using System;
using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Rayon de spawn autour du joueur")]
    [SerializeField] private float _enemyMinRadius = 10f;
    [SerializeField] private float _enemyMaxRadius = 25f;

    [Header("Intervalles de spawn")]
    [SerializeField] private float _enemySpawnInterval = 1f;
    [SerializeField] private float _enemyInitialSpawnInterval = 2f;
    [SerializeField] private float _enemyIntervalReducer = 0.1f;

    [Header("Difficulté progressive")]
    [SerializeField] private int _difficultyThreshold = 100;
    [SerializeField] private int _difficultyMultiplier = 2;
    [SerializeField] private int _timeToSpawnEnemy2 = 1000;
    [SerializeField] private int _timeToSpawnEnemy3 = 2000;

    [Header("Références")]
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private GameObject _enemyContainer;

    private float[] _halfEnemyWidthTable;
    private float[] _halfEnemyHeightTable;
    private bool _stopSpawning = false;
    private Transform _player;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _halfEnemyWidthTable  = new float[_enemyPrefabs.Length];
        _halfEnemyHeightTable = new float[_enemyPrefabs.Length];

        foreach (var enemy in _enemyPrefabs)
        {
            int index = Array.IndexOf(_enemyPrefabs, enemy);
            _halfEnemyWidthTable[index]  = enemy.GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
            _halfEnemyHeightTable[index] = enemy.GetComponent<SpriteRenderer>().sprite.bounds.extents.y;
        }

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null)
            _player = target.transform;

        StartCoroutine(SpawnEnemyRoutine());
    }


    IEnumerator SpawnEnemyRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(_enemyInitialSpawnInterval);

            if (GameManager.Instance.IsEnemyMaxed()) continue;
            if (_player == null) continue;

            // === SÉLECTION ENNEMI === //
            int randomEnemyIndex;

            if (GameManager.Instance.PlayerScore >= _timeToSpawnEnemy3)
                randomEnemyIndex = UnityEngine.Random.Range(0, _enemyPrefabs.Length);
            else if (GameManager.Instance.PlayerScore >= _timeToSpawnEnemy2)
                randomEnemyIndex = UnityEngine.Random.Range(0, 2);
            else
                randomEnemyIndex = 0;
            // ======================== //


            // === GÉNÉRATION COORDONNÉES === //
            float spawnRadius = UnityEngine.Random.Range(_enemyMinRadius, _enemyMaxRadius);
            float angle       = UnityEngine.Random.value * (float)Math.PI * 2;

            float randomX = (float)Math.Cos(angle) * spawnRadius + _player.position.x;
            float randomY = (float)Math.Sin(angle) * spawnRadius + _player.position.y;

            float clampedX = GameManager.Instance.ClampX(randomX, _halfEnemyWidthTable[randomEnemyIndex]);
            float clampedY = GameManager.Instance.ClampY(randomY, _halfEnemyHeightTable[randomEnemyIndex]);
            // ============================== //


            // === INSTANCIATION === //
            GameObject newEnemy = Instantiate(_enemyPrefabs[randomEnemyIndex], new Vector3(clampedX, clampedY, 0f), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            // ===================== //


            // === DIFFICULTÉ PROGRESSIVE === //
            if (GameManager.Instance.PlayerScore >= _difficultyThreshold)
            {
                _difficultyThreshold *= _difficultyMultiplier;
                _enemySpawnInterval  -= _enemySpawnInterval * _enemyIntervalReducer;
            }
            // ============================== //

            yield return new WaitForSeconds(_enemySpawnInterval);
        }
    }


    public void StopSpawning()
    {
        _stopSpawning = true;
    }
}