using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ===================== ÉVÉNEMENTS ===================== //
    public event EventHandler<OnEnemyDestroyedEventArgs> OnEnemyDestroyed;
    public class OnEnemyDestroyedEventArgs : EventArgs
    {
        public string DestroyedObjectTag;
    }
    // ====================================================== //


    [Header("Limites de la map")]
    [SerializeField] private GameObject _background;
    private float _minX, _maxX, _minY, _maxY;

    [Header("Gestion des ennemis")]
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private int _maxEnemy = 20;

    [Header("Score")]
    private int _playerScore = 0;
    public int PlayerScore => _playerScore;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SpriteRenderer backgroundRenderer = _background.GetComponent<SpriteRenderer>();
        _minX = backgroundRenderer.bounds.min.x;
        _maxX = backgroundRenderer.bounds.max.x;
        _minY = backgroundRenderer.bounds.min.y;
        _maxY = backgroundRenderer.bounds.max.y;
    }


    // ===================== GESTION ENNEMIS ===================== //

    public bool IsEnemyMaxed()
    {
        return _enemyContainer.transform.childCount >= _maxEnemy;
    }

    public void EnemyDestroyed(int p_enemyPoints, string p_gameObjectTag)
    {
        if (p_gameObjectTag == "PlayerAttack")
            _playerScore += p_enemyPoints;

        OnEnemyDestroyed?.Invoke(this, new OnEnemyDestroyedEventArgs
        {
            DestroyedObjectTag = p_gameObjectTag
        });
    }

    public void TriggerOnEnemyDestroyed(object sender)
    {
        OnEnemyDestroyed?.Invoke(this, new OnEnemyDestroyedEventArgs
        {
            DestroyedObjectTag = "Player"
        });
    }

    // =========================================================== //


    // ===================== LIMITES DE LA MAP ===================== //

    public float ClampX(float coo, float half)
    {
        return Mathf.Clamp(coo, _minX + half, _maxX - half);
    }

    public float ClampY(float coo, float half)
    {
        return Mathf.Clamp(coo, _minY + half, _maxY - half);
    }

    // ============================================================= //
}