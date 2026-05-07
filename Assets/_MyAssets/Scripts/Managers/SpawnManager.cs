using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;


    [SerializeField] private float _enemyMinRadius = 10f;
    [SerializeField] private float _enemyMaxRadius = 25f;

    [SerializeField] private float _enemySpawnInterval = 1f;
    [SerializeField] private float _enemyInitialSpawnInterval = 2f;
    [SerializeField] private float _enemyIntervalReducer = 0.1f;

    [SerializeField] private int _difficultyThreshold = 100;
    [SerializeField] private int _difficultyMultiplier = 2;

    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private GameObject _enemyContainer;

    [SerializeField] private int _timeToSpawnEnemy2 = 1000;
    [SerializeField] private int _timeToSpawnEnemy3 = 2000;



    private float[] _halfEnemyWidthTable;
    private float[] _halfEnemyHeightTable;

    private bool _stopSpawning = false;

    private Transform _player;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _halfEnemyWidthTable = new float[_enemyPrefabs.Length];
        _halfEnemyHeightTable = new float[_enemyPrefabs.Length];
        foreach (var enemy in _enemyPrefabs)
        {
            float halfEnemyWidth = enemy.GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
            _halfEnemyWidthTable[Array.IndexOf(_enemyPrefabs, enemy)] = halfEnemyWidth;

            float halfEnemyHeight = enemy.GetComponent<SpriteRenderer>().sprite.bounds.extents.y;
            _halfEnemyHeightTable[Array.IndexOf(_enemyPrefabs, enemy)] = halfEnemyHeight;
        }


        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
            _player = target.transform;

        StartCoroutine(SpawnEnemyRoutine());
    }

    void Update()
    {
        
    }


    IEnumerator SpawnEnemyRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(_enemyInitialSpawnInterval); // Attente de l'intervalle initial avant de faire apparaître le premier ennemi

            if (GameManager.Instance.IsEnemyMaxed()) continue;

            // === SÉLECTION ENNEMI === //
            int randomEnemyIndex;
            if (GameManager.Instance.PlayerScore >= _timeToSpawnEnemy3)
            {
                randomEnemyIndex = UnityEngine.Random.Range(0, _enemyPrefabs.Length);
            }
            else if (GameManager.Instance.PlayerScore >= _timeToSpawnEnemy2)
            {
                randomEnemyIndex = UnityEngine.Random.Range(0, 1);
            }
            else
            {
                randomEnemyIndex = 0;
            }

            // ==== TEST ==== //
            randomEnemyIndex = UnityEngine.Random.Range(0, _enemyPrefabs.Length);

            // === GÉNÉRATION COORDONNÉES === //
            float playerX = _player.transform.position.x;
            float playerY = _player.transform.position.y;
            float spawnRadius = UnityEngine.Random.Range(_enemyMinRadius, _enemyMaxRadius);
            float angle = UnityEngine.Random.value * (float)Math.PI * 2;
            float randomX = (float)Math.Cos(angle) * spawnRadius + playerX;
            float randomY = (float)Math.Sin(angle) * spawnRadius + playerY;

            // Génération d'une position X et Y aléatoire pour faire apparaître l'ennemi
            /*float randomX = UnityEngine.Random.Range(-Camera.main.orthographicSize * Camera.main.aspect + _halfEnemyWidthTable[randomEnemyIndex],
            Camera.main.orthographicSize * Camera.main.aspect - _halfEnemyWidthTable[randomEnemyIndex]); 

            float randomY = UnityEngine.Random.Range(-Camera.main.orthographicSize * Camera.main.aspect + _halfEnemyHeightTable[randomEnemyIndex],
            Camera.main.orthographicSize * Camera.main.aspect - _halfEnemyHeightTable[randomEnemyIndex]);
            */

            float clampedX = GameManager.Instance.ClampX(randomX, _halfEnemyWidthTable[randomEnemyIndex]);
            float clampedY = GameManager.Instance.ClampY(randomY, _halfEnemyHeightTable[randomEnemyIndex]);

            Vector3 spawnPosition = new Vector3(clampedX, clampedY, 0f);
            GameObject newEnemy = Instantiate(_enemyPrefabs[randomEnemyIndex], spawnPosition, Quaternion.identity); // Instanciation de l'ennemi à la position de spawn
            newEnemy.transform.parent = _enemyContainer.transform; // Organisation de l'ennemi dans le conteneur

            if (GameManager.Instance.PlayerScore >= _difficultyThreshold) // Vérifie si le score du joueur atteint le seuil pour augmenter le taux de spawn des ennemis
            {
                _difficultyThreshold *= _difficultyMultiplier;

                float intervalFraction = _enemySpawnInterval * _enemyIntervalReducer;
                _enemySpawnInterval -= intervalFraction;
            }
            yield return new WaitForSeconds(_enemySpawnInterval); // Attente de l'intervalle de temps entre chaque spawn d'ennemi
        }
    }


}
