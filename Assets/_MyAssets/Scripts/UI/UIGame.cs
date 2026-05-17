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
    [SerializeField] private Button _buttonFirstUpgrade;

    [Header("Barres UI")]
    [SerializeField] private Image _lifeBar;
    [SerializeField] private Image _xpBar;
    [SerializeField] private float _maxLife = 10f;
    [SerializeField] private int _enemiesPerLevel = 5;

    private float _currentLife;

    int level = 1;
    int points = 0;
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

        _lifeBar.fillAmount = 1f;
        _xpBar.fillAmount = 0f;

        _currentLife = _maxLife;

        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyDestroyed += OnEnemyDestroyed;

        Time.timeScale = 0f;

        _instructionsPanel.SetActive(true);
        _gameBar.SetActive(false);
        _upgradePanel.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(_buttonCloseInstructions.gameObject);

        _txtLevel.text = $"Niveau {level}";
        _txtPoints.text = $"{points}";

    }

    private void OnDestroy()
    {

        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyDestroyed -= OnEnemyDestroyed;

    }

    public void UpdateLifeBar(float currentLife)
    {

        _currentLife = currentLife;
        _lifeBar.fillAmount = _currentLife / _maxLife;

    }

    private void OnEnemyDestroyed(object sender, GameManager.OnEnemyDestroyedEventArgs e)
    {

        if (e.DestroyedObjectTag == "PlayerAttack")
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

        _lifeBar.fillAmount = _currentLife / _maxLife;

    }

    public void OpenUpgradePanel()
    {

        Time.timeScale = 0f;

        _upgradePanel.SetActive(true);
        _gameBar.SetActive(false);
        _instructionsPanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_buttonFirstUpgrade.gameObject);

    }

    public void UpgradeChosen()
    {

        Time.timeScale = 1.0f;

        _instructionsPanel.SetActive(false);
        _upgradePanel.SetActive(false);
        _gameBar.SetActive(true);

        _lifeBar.fillAmount = _currentLife / _maxLife;

        level += 1;
        _txtLevel.text = $"Niveau {level}";

    }

}