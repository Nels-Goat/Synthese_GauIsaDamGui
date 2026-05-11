using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIGame : UI
{
    public static UIGame Instance;

    [SerializeField] private TextMeshProUGUI _txtLevel;
    [SerializeField] private TextMeshProUGUI _txtPoints;
    [SerializeField] private GameObject _upgradePanel;
    [SerializeField] private GameObject _instructionsPanel;
    [SerializeField] private GameObject _gameBar;
    [SerializeField] private Button _buttonCloseInstructions;

    [Header("Barres UI")]
    [SerializeField] private Image _lifeBar;
    [SerializeField] private Image _xpBar;
    [SerializeField] private float _maxLife = 10f;
    [SerializeField] private int _enemiesPerLevel = 5;

    int level = 1;
    int points = 0;
    private float _currentLife;
    private int _enemiesKilled = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _currentLife = _maxLife;
        _lifeBar.fillAmount = 1f;  // plein
        _xpBar.fillAmount = 0f;    // vide

        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyDestroyed += OnEnemyDestroyed;

        Time.timeScale = 0f;
        _instructionsPanel.SetActive(true);
        _gameBar.SetActive(false);
        _upgradePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_buttonCloseInstructions.gameObject);
        _txtLevel.text = $"Niveau {level}";
        _txtPoints.text = $"{points}";
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyDestroyed -= OnEnemyDestroyed;
    }

    private void OnEnemyDestroyed(object sender, GameManager.OnEnemyDestroyedEventArgs e)
    {
        if (e.DestroyedObjectTag == "Player")
        {
            _currentLife = Mathf.Max(0, _currentLife - 1f);
            _lifeBar.fillAmount = 1f - (_currentLife / _maxLife);
        }
        else if (e.DestroyedObjectTag == "PlayerAttack")
        {
            _enemiesKilled++;
            points++;
            _txtPoints.text = $"{points}";
            _xpBar.fillAmount = (_enemiesKilled % _enemiesPerLevel) / (float)_enemiesPerLevel;

            if (_enemiesKilled % _enemiesPerLevel == 0)
                OpenUpgradePanel();
        }
    }

    public void OnCloseClick()
    {
        Time.timeScale = 1.0f;
        _instructionsPanel.SetActive(false);
        _upgradePanel.SetActive(false);
        _gameBar.SetActive(true);
    }

    public void OpenUpgradePanel()
    {
        Time.timeScale = 0f;
        _upgradePanel.SetActive(true);
        _gameBar.SetActive(false);
        _instructionsPanel.SetActive(false);
    }

    public void UpgradeChosen()
    {
        Time.timeScale = 1.0f;
        _instructionsPanel.SetActive(false);
        _upgradePanel.SetActive(false);
        _gameBar.SetActive(true);
        level += 1;
        _txtLevel.text = $"Niveau {level}";
    }
}