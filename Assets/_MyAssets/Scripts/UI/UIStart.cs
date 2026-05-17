using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIStart : UI
{
    [SerializeField] private GameObject _resultsPanel;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private GameObject _gameButtons;
    [SerializeField] private Button _buttonStart;
    [SerializeField] private Button _buttonClose;

    private void Awake()
    {
        UIGame uiGame = FindAnyObjectByType<UIGame>();
        if (uiGame != null)
            Destroy(uiGame);
    }

    private void Start()
    {
        _resultsPanel.SetActive(false);
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(_buttonStart.gameObject);
    }

    public void OnResultsClick()
    {

        _resultsPanel.SetActive(true);
        _gameButtons.SetActive(false);
        _creditsButton.enabled = false;
        EventSystem.current.SetSelectedGameObject(_buttonClose.gameObject);
    }

    public void OnStartClick()
    {

        SceneManager.LoadScene(1);
    }

    public void OnCloseClick()
    {

        _resultsPanel.SetActive(false);
        _gameButtons.SetActive(true);
        _creditsButton.enabled = true;
        EventSystem.current.SetSelectedGameObject(_buttonStart.gameObject);
    }

    public void OnCreditsClick()
    {
        SceneManager.LoadScene("CreditsTest_JeremyI");
    }
}