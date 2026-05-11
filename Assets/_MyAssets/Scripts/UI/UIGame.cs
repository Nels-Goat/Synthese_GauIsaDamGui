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

    int level = 1;
    int points = 0;

    private void Awake()
    {
        
        if(Instance == null)
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

        Time.timeScale = 0f;
        _instructionsPanel.SetActive(true);

        _gameBar.SetActive(false);
        _upgradePanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_buttonCloseInstructions.gameObject);

        _txtLevel.text = $"Niveau {level}";
        _txtPoints.text = $"{points}";

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

        OnCloseClick();

    }

}
