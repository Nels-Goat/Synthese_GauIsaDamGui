using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ===================== ÉVÉNEMENTS ===================== //
    public event EventHandler<OnEnemyDestroyedEventArgs> OnEnemyDestroyed;
    public class OnEnemyDestroyedEventArgs : EventArgs
    {
        public string DestroyedObjectTag;
        public int Damage;
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
        PlayerPrefs.SetInt("PlayerScore", 0);

        SpriteRenderer backgroundRenderer = _background.GetComponent<SpriteRenderer>();
        _minX = backgroundRenderer.bounds.min.x;
        _maxX = backgroundRenderer.bounds.max.x;
        _minY = backgroundRenderer.bounds.min.y;
        _maxY = backgroundRenderer.bounds.max.y;

        Debug.Log("[GameManager] Initialisé — Score: 0");
    }

    // ===================== GESTION ENNEMIS ===================== //
    public bool IsEnemyMaxed()
    {
        return _enemyContainer.transform.childCount >= _maxEnemy;
    }

    public void EnemyDestroyed(int p_enemyPoints, string p_gameObjectTag, int p_damage = 1)
    {
        if (p_gameObjectTag == "PlayerAttack")
        {
            _playerScore += p_enemyPoints;
            Debug.Log($"[GameManager] +{p_enemyPoints} pts | Score total : {_playerScore}");
        }

        OnEnemyDestroyed?.Invoke(this, new OnEnemyDestroyedEventArgs
        {
            DestroyedObjectTag = p_gameObjectTag,
            Damage = p_damage
        });
    }

    public void TriggerOnEnemyDestroyed(object sender)
    {
        OnEnemyDestroyed?.Invoke(this, new OnEnemyDestroyedEventArgs
        {
            DestroyedObjectTag = "Player",
            Damage = 1
        });
    }
    // =========================================================== //

    // ===================== FIN DE JEU ===================== //
    public void EndGame()
    {
        // Sauvegarde du score
        PlayerPrefs.SetInt("PlayerScore", _playerScore);

        // Meilleur score
        if (PlayerPrefs.HasKey("PlayerHighScore"))
        {
            int highScore = PlayerPrefs.GetInt("PlayerHighScore");
            if (_playerScore > highScore)
                PlayerPrefs.SetInt("PlayerHighScore", _playerScore);
        }
        else
        {
            PlayerPrefs.SetInt("PlayerHighScore", _playerScore);
        }

        PlayerPrefs.Save();

        Debug.Log($"[GameManager] Game Over — Score: {_playerScore} | HighScore: {PlayerPrefs.GetInt("PlayerHighScore")}");

        SceneManager.LoadScene("End");
    }
    // ====================================================== //

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