using UnityEngine;

public class UICredits : UI
{
    [SerializeField] private string _sceneName = "Start";
    public float scrollSpeed = 50f;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {

        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;


        if (rectTransform.anchoredPosition.y > 1500f)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneName);
        }
    }
}
