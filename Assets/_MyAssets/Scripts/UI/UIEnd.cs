using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIEnd : UI
{
    [SerializeField] private string _startSceneName = "Start";

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI _txtScore;
    [SerializeField] private TextMeshProUGUI _txtHighScore;

    [Header("Panels")]
    [SerializeField] private GameObject _panelButtons;
    [SerializeField] private GameObject _panelNewHighScore;
    [SerializeField] private GameObject _panelResetHighScore;

    [Header("Buttons")]
    [SerializeField] private Button _buttonRestart;

    private void Start()
    {
        int score = PlayerPrefs.GetInt("PlayerScore", 0);

        HighScoreTable highScoreTable = FindAnyObjectByType<HighScoreTable>();
        List<HighScoreTable.HighScoreEntry> entries = highScoreTable.GetHighScoreEntries();

        int highScore = (entries != null && entries.Count > 0) ? entries[0].score : 0;

        _txtScore.text = $"Score : {score}";

        if (score > highScore)
        {
            _txtHighScore.text = $"Nouveau record : {score} !";
            _panelButtons.SetActive(false);
            _panelNewHighScore.SetActive(true);
        }
        else
        {
            _txtHighScore.text = $"Meilleur score : {highScore}";
            _panelButtons.SetActive(true);
            _panelNewHighScore.SetActive(false);

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(_buttonRestart.gameObject);
        }
    }

    public void OnRestartScoresClick()
    {
        _panelResetHighScore.SetActive(true);
        _panelButtons.SetActive(false);
        _panelNewHighScore.SetActive(false);
    }

    public void OnClosePanelResetHighScoreClick()
    {
        _panelResetHighScore.SetActive(false);
        _panelButtons.SetActive(true);
        _panelNewHighScore.SetActive(false);
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(_buttonRestart.gameObject);
    }

    public void OnMenuClick()
    {
        SceneManager.LoadScene(_startSceneName);
    }
}