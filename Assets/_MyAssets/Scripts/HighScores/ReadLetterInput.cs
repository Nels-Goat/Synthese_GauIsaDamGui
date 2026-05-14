using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadLetterInput : MonoBehaviour
{
    [SerializeField] private GameObject _resetHighScoresPanel;
    [SerializeField] private GameObject _getNewHighScorePanel;

    private Button _letterButton;
    private GetNewHighScore _getNewHighScore;
    private ResetHighScores _resetHighScores;

    private void Start()
    {
        _getNewHighScore = FindAnyObjectByType<GetNewHighScore>();
        _resetHighScores = FindAnyObjectByType<ResetHighScores>();
        _letterButton = this.GetComponent<Button>();
        _letterButton.onClick.AddListener(LireTexte);
    }

    public void LireTexte()
    {
        string letterName = gameObject.name;
        Debug.Log($"Bouton cliqué: {letterName} | RecordPanel actif: {_getNewHighScorePanel.activeSelf} | ResetPanel actif: {_resetHighScoresPanel.activeSelf}");

        if (_getNewHighScorePanel.activeSelf)
        {
            if (letterName == "Enter")
                _getNewHighScore.EnregistrerNom();
            else if (_getNewHighScore != null)
                _getNewHighScore.AddLetter(letterName);
        }
        else if (_resetHighScoresPanel.activeSelf)
        {
            if (letterName == "Enter")
                _resetHighScores.ValidatePassword();
            else if (_resetHighScores != null)
                _resetHighScores.AddLettterPass(letterName);
        }
    }
}