using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GetNewHighScore : MonoBehaviour
{
    [SerializeField] private GameObject _getNewHighScporePanel;
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private int _maxNameLength = 3;
    [SerializeField] private TextMeshProUGUI _txtName;
    [SerializeField] private Button _btSaveName;
    [SerializeField] private Button _menuButton;

    private string _tempText = "";
    private bool _showingError = false;

    private void Start()
    {
        _btSaveName.onClick.AddListener(EnregistrerNom);
        StartCoroutine(CloseGetHighScorePanelDelay());
    }

    IEnumerator CloseGetHighScorePanelDelay()
    {
        yield return new WaitForSeconds(60f);
        if (_getNewHighScporePanel.activeSelf)
            OnCancelClick();
    }

    public void AddLetter(string p_letter)
    {
        if (_getNewHighScporePanel.activeSelf)
        {
            // Repart à zéro proprement si erreur affichée
            if (_showingError)
            {
                _showingError = false;
                _tempText = "";
            }

            if (p_letter == "Space")
            {
                _tempText += " ";
            }
            else if (p_letter == "←" && _tempText.Length > 0)
            {
                _tempText = _tempText.Remove(_tempText.Length - 1);
            }
            else
            {
                if (_tempText.Length < _maxNameLength)
                    _tempText += p_letter;
            }
            _txtName.text = _tempText;
        }
    }

    public void EnregistrerNom()
    {
        bool validName = false;
        string nameInput = _txtName.text;

        foreach (char c in nameInput)
        {
            if (c != ' ')
                validName = true;
        }

        if (!string.IsNullOrEmpty(nameInput) && validName)
        {
            HighScoreTable highScoreTable = FindAnyObjectByType<HighScoreTable>();
            highScoreTable.AddHighScoreEntry(PlayerPrefs.GetInt("PlayerScore", 0), nameInput);
            highScoreTable.DisplayHighScoreTable();

            _getNewHighScporePanel.SetActive(false);
            _endPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_menuButton.gameObject);
            _tempText = "";
            _showingError = false;
        }
        else
        {
            // Affiche le message d'erreur dans le champ texte comme pour le mot de passe
            _txtName.text = "Nom invalide!";
            _showingError = true;
        }
    }

    public void OnCancelClick()
    {
        _getNewHighScporePanel.SetActive(false);
        _endPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_menuButton.gameObject);
        _tempText = "";
        _showingError = false;
    }
}