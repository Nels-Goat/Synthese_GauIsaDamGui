using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string _endSceneName = "EndTest_NelsonG";
    public static GameManager Instance;

    public event EventHandler<OnEnemyDestroyedEventArgs> OnEnemyDestroyed;
    public class OnEnemyDestroyedEventArgs : EventArgs
    {
        public string DestroyedObjectTag;
        public int Damage;
    }

    [Header("Limites de la map")]
    [SerializeField] private float _minX = -10f;
    [SerializeField] private float _maxX = 10f;
    [SerializeField] private float _minY = -5f;
    [SerializeField] private float _maxY = 5f;

    [Header("Gestion des ennemis")]
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private int _maxEnemy = 20;

    private int _playerScore = 0;
    public int PlayerScore => _playerScore;

    private int _enemyKillCount = 0;
    public int EnemyKillCount => _enemyKillCount;

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

        Debug.Log("[GameManager] Initialis� � Score: 0");
    }

    public bool IsEnemyMaxed()
    {
        return _enemyContainer.transform.childCount >= _maxEnemy;
    }

    public void EnemyDestroyed(int p_enemyPoints, string p_gameObjectTag, int p_damage = 1)
    {
        if (p_gameObjectTag == "PlayerAttack")
        {
            _enemyKillCount++;
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

        Debug.Log($"[GameManager] Game Over � Score: {_playerScore} | HighScore: {PlayerPrefs.GetInt("PlayerHighScore")}");

        SceneManager.LoadScene(_endSceneName);
    }
    // ====================================================== //

    // ===================== M�THODES DE D�LIMITATION ===================== //
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