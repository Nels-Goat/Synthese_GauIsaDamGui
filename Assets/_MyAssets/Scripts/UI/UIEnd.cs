using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIEnd : UI
{
    [SerializeField] private TextMeshProUGUI _txtScore;
    //[SerializeField] private TextMeshProUGUI _txtHighScore;
    [SerializeField] private Button _buttonRestart;

    private void Start()
    {
        int score = PlayerPrefs.GetInt("PlayerScore", 0);
        //int highScore = PlayerPrefs.GetInt("PlayerHighScore", 0);

        _txtScore.text = $"Score : {score}";
        //_txtHighScore.text = $"Meilleur score : {highScore}";

        //Debug.Log($"[UIEnd] Score: {score} | HighScore: {highScore}");

        EventSystem.current.SetSelectedGameObject(_buttonRestart.gameObject);
    }

    public void OnRestartClick()
    {

        SceneManager.LoadScene("Game");

    }

    public void OnMenuClick()
    {

        SceneManager.LoadScene("StartTest_JeremyI");

    }
}