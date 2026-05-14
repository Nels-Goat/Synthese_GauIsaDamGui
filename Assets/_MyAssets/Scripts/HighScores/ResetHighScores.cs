using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetHighScores : MonoBehaviour
{
    private const string PASSWORD = "TOTO";

    [SerializeField] private GameObject _resetHighScoresPanel;
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private int _maxPasswordLength = 10;
    [SerializeField] private TextMeshProUGUI _txtPass;
    [SerializeField] private Button _btResetPass;
    [SerializeField] private Button _menuButton;

    private string _tempText = "";
    private string _tempTextHidden = "";
    private bool _showingError = false; // Indique si un message d'erreur est affiché

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
            // Si un message d'erreur est affiché, on repart à zéro proprement
            if (_showingError)
            {
                _showingError = false;
                _tempText = "";
                _tempTextHidden = "";
            }

            if (lettre == "Space")
            {
                _tempText += " ";
                _tempTextHidden += "*";
            }
            else if (lettre == "←" && _tempText.Length > 0)
            {
                _tempText = _tempText.Remove(_tempText.Length - 1);
                _tempTextHidden = _tempTextHidden.Remove(_tempTextHidden.Length - 1);
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
        }

        _tempText = "";
        _tempTextHidden = "";

    }

    IEnumerator LoadMenuSceneDelay()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }

    public void OnCancelClick()
    {
        _resetHighScoresPanel.SetActive(false);
        _endPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_menuButton.gameObject);
        _txtPass.text = "";
        _showingError = false;
    }
}