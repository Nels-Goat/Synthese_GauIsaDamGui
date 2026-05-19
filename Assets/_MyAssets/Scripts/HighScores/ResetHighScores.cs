using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetHighScores : MonoBehaviour
{
    private const string PASSWORD = "GAUISADAMGUI";

    [SerializeField] private GameObject _resetHighScoresPanel;
    [SerializeField] private int _maxPasswordLength = 10;
    [SerializeField] private TextMeshProUGUI _txtPass;
    [SerializeField] private Button _btResetPass;
    [SerializeField] private UIEnd _uiEnd;

    private string _tempText = "";
    private string _tempTextHidden = "";
    private bool _showingError = false;

    private void Start()
    {
        _btResetPass.onClick.AddListener(ValidatePassword);
        StartCoroutine(CloseResetHighScorePanelDelay());
    }

    IEnumerator CloseResetHighScorePanelDelay()
    {
        yield return new WaitForSeconds(60f);
        OnCancelClick();
    }

    public void AddLettterPass(string lettre)
    {
        if (_resetHighScoresPanel.activeSelf)
        {
            if (_showingError)
            {
                _showingError = false;
                _tempText = "";
                _tempTextHidden = "";
            }

            if (lettre == "Space")
            {
                _tempText += "_";
                _tempTextHidden += "*";
            }
            else
            {
                if (_tempText.Length < _maxPasswordLength)
                {
                    _tempText += lettre;
                    _tempTextHidden += "*";
                }
            }
            _txtPass.text = _tempTextHidden;
        }
    }

    public void ValidatePassword()
    {
        if (_tempText == PASSWORD)
        {
            _txtPass.text = "Mot de passe correct!";
            _showingError = true;
            PlayerPrefs.DeleteKey("highScoreTable");
            StartCoroutine(LoadMenuSceneDelay());
        }
        else
        {
            _txtPass.text = "Mot de passe incorrect!";
            _showingError = true;
            _tempText = "";
            _tempTextHidden = "";
            StartCoroutine(CloseAfterErrorDelay());
        }
    }

    IEnumerator CloseAfterErrorDelay()
    {
        yield return new WaitForSeconds(2f);
        OnCancelClick();
    }

    IEnumerator LoadMenuSceneDelay()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }

    public void OnCancelClick()
    {
        _txtPass.text = "";
        _showingError = false;
        _tempText = "";
        _tempTextHidden = "";

        if (_uiEnd != null)
            _uiEnd.OnClosePanelResetHighScoreClick();
    }
}