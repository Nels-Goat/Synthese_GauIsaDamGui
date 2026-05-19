using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

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
    [SerializeField] private int _enemiesPerLevel = 5;

    [Header("Propriétés Upgrade Panel")]
    [SerializeField] private Sprite[] _cardSprites;
    [SerializeField] private Button[] _cardSlots;

    int level = 1;

    private Player _player;

    // === DEV VAR === //
    InputSystem_Actions _inputSystemActions;
    // =============== //

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            _player = player.GetComponent<Player>();
            UpdateLifeBar();
        }

        _xpBar.fillAmount = 0f;

        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyDestroyed += OnEnemyDestroyed;

        Time.timeScale = 0f;
        _instructionsPanel.SetActive(true);
        _gameBar.SetActive(false);
        _upgradePanel.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(_buttonCloseInstructions.gameObject);

        _txtLevel.text = $"Niveau {level}";
        _txtPoints.text = $"{GameManager.Instance.PlayerScore}";


        // DEV
        EnableGod();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyDestroyed -= OnEnemyDestroyed;

        _inputSystemActions.Player.God.performed -= GodLvlUp;
    }

    public void UpdateLifeBar() => _lifeBar.fillAmount = _player.PlayerLife / _player.PlayerMaxLife;
    private void UpdateScore() => _txtPoints.text = $"{GameManager.Instance.PlayerScore}";
    private void UpdateXpBar() => _xpBar.fillAmount = (GameManager.Instance.EnemyKillCount % _enemiesPerLevel) / (float)_enemiesPerLevel;

    private void OnEnemyDestroyed(object sender, GameManager.OnEnemyDestroyedEventArgs e)
    {
        if (e.DestroyedObjectTag == "PlayerAttack")
        {
            UpdateScore();
            UpdateXpBar();

            if (GameManager.Instance.EnemyKillCount % _enemiesPerLevel == 0)
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
        List<Powerup> powerups = PowerupManager.Instance.GetPowerups();
        if (powerups.Count <= 0)
        {
            LevelUp();
            PowerupManager.Instance.UpgradePowerup(EPowerupType.Rafraîchissement);
            return;
        }

        DisableCards();
        Time.timeScale = 0f;
        _upgradePanel.SetActive(true);
        _gameBar.SetActive(false);
        _instructionsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_buttonFirstUpgrade.gameObject);

        
        for (int i = 0; i < powerups.Count; i++)
        {
            Button card = _cardSlots[i];
            Powerup pu  = powerups[i];
            
            Image cSprite          = card.gameObject.GetComponent<Image>();
            TextMeshProUGUI cLevel = card.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            Image cIcon            = card.gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
            TextMeshProUGUI cTitle = card.gameObject.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI cDesc  = card.gameObject.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();

            cSprite.sprite = _cardSprites[pu.Level];
            cLevel.text    = (pu.Level+1).ToString();
            cIcon.sprite   = pu.Icon;
            cTitle.text    = pu.Name.ToString();
            cDesc.text     = pu.Description;

            card.gameObject.SetActive(true);
        }
    }

    public void UpgradeChosen(TextMeshProUGUI cTitle)
    {
        DisableCards();

        EPowerupType name;
        bool validSelection = Enum.TryParse<EPowerupType>(cTitle.text, out name);
        if (validSelection)
            PowerupManager.Instance.UpgradePowerup(name);

        Time.timeScale = 1.0f;
        _instructionsPanel.SetActive(false);
        _upgradePanel.SetActive(false);
        _gameBar.SetActive(true);
        UpdateLifeBar();
        LevelUp();
    }

    private void LevelUp()
    {
        level += 1;
        _txtLevel.text = $"Niveau {level}";

        _enemiesPerLevel += level % 5 == 0 ? 5 : 0;
        UpdateXpBar();
    }

    private void DisableCards()
    {
        foreach(Button card in _cardSlots) card.gameObject.SetActive(false);
    }




    // DEV
    private void EnableGod()
    {
        _inputSystemActions = new InputSystem_Actions();
        _inputSystemActions.Player.Enable();
        _inputSystemActions.Player.God.performed += GodLvlUp;
    }

    private void GodLvlUp(InputAction.CallbackContext context)
    {
        OpenUpgradePanel();
    }
}

