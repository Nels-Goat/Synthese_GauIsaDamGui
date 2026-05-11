using UnityEngine;

public class CreditsScroll : MonoBehaviour
{

    public float scrollSpeed = 50f;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Déplace vers le haut
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Optionnel : retourner au menu quand c'est terminé
        if (rectTransform.anchoredPosition.y > 2000f)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
        }
    }

}
