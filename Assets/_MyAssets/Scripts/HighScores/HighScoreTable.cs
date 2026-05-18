using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTable : MonoBehaviour
{
    private class HighScores
    {
        public List<HighScoreEntry> highScoreEntryList;
    }

    [System.Serializable]
    public class HighScoreEntry
    {
        public int score;
        public string name;
    }

    [SerializeField] private int _maxHighScoreEntries = 10;

    private Transform _entryContainer;
    private Transform _entryTemplate;
    private List<Transform> _highScoreEntryTransformList;
    private HighScores highScores;

    private void Awake()
    {
        DisplayHighScoreTable();
    }

    public void DisplayHighScoreTable()
    {
        _entryContainer = transform.Find("HighScoreEntryContainer");
        _entryTemplate = _entryContainer.Find("HighScoreEntryTemplate");
        _entryTemplate.gameObject.SetActive(false);

        // Nettoie les anciennes entrées avant d'afficher
        foreach (Transform child in _entryContainer)
        {
            if (child.name != "HighScoreEntryTemplate")
                Destroy(child.gameObject);
        }

        string jsonString = PlayerPrefs.GetString("highScoreTable");
        highScores = JsonUtility.FromJson<HighScores>(jsonString);

        if (highScores == null)
        {
            return;
        }

        // Tri par ordre décroissant de score
        for (int i = 0; i < highScores.highScoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highScores.highScoreEntryList.Count; j++)
            {
                if (highScores.highScoreEntryList[j].score > highScores.highScoreEntryList[i].score)
                {
                    HighScoreEntry tmp = highScores.highScoreEntryList[i];
                    highScores.highScoreEntryList[i] = highScores.highScoreEntryList[j];
                    highScores.highScoreEntryList[j] = tmp;
                }
            }
        }

        _highScoreEntryTransformList = new List<Transform>();
        int highScoreEntryCount = Mathf.Min(highScores.highScoreEntryList.Count, _maxHighScoreEntries);
        int highScoreEntryIndex = 1;

        foreach (HighScoreEntry highScoreEntry in highScores.highScoreEntryList)
        {
            if (highScoreEntryIndex <= highScoreEntryCount)
                CreateHighScoreEntryTransform(highScoreEntry, _entryContainer, _highScoreEntryTransformList);

            highScoreEntryIndex++;
        }
    }

    private void CreateHighScoreEntryTransform(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 50f;
        Transform entryTransform = Instantiate(_entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0f, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
            default: rankString = rank + "TH"; break;
        }

        entryTransform.Find("TxtPos").GetComponent<TextMeshProUGUI>().text = rankString;
        entryTransform.Find("TxtScore").GetComponent<TextMeshProUGUI>().text = highScoreEntry.score.ToString();
        entryTransform.Find("TxtName").GetComponent<TextMeshProUGUI>().text = highScoreEntry.name;

        if (rank == 1)
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(255, 210, 3, 71);
        else if (rank == 2)
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(203, 201, 193, 71);
        else if (rank == 3)
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(176, 114, 26, 71);
        else
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(255, 255, 255, 0);

        transformList.Add(entryTransform);
    }

    public void AddHighScoreEntry(int p_score, string p_name)
    {
        HighScoreEntry highScoreEntry = new HighScoreEntry { score = p_score, name = p_name };

        string jsonString = PlayerPrefs.GetString("highScoreTable");
        highScores = JsonUtility.FromJson<HighScores>(jsonString);

        if (highScores == null)
        {
            highScores = new HighScores()
            {
                highScoreEntryList = new List<HighScoreEntry>()
            };
        }

        highScores.highScoreEntryList.Add(highScoreEntry);

        // Limite la liste au maximum d'entrées permises pour éviter une liste infinie
        if (highScores.highScoreEntryList.Count > _maxHighScoreEntries)
        {
            // Trie d'abord pour garder les meilleurs scores
            highScores.highScoreEntryList.Sort((a, b) => b.score.CompareTo(a.score));
            highScores.highScoreEntryList.RemoveAt(highScores.highScoreEntryList.Count - 1);
        }

        string json = JsonUtility.ToJson(highScores);
        PlayerPrefs.SetString("highScoreTable", json);
        PlayerPrefs.Save();
    }

    public List<HighScoreEntry> GetHighScoreEntries()
    {
        return highScores?.highScoreEntryList;
    }
}