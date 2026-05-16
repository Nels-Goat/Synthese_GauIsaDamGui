using UnityEngine;

public class UICredits : UI
{
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
        }
    }
}
