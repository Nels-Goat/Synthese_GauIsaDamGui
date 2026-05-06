using UnityEngine;

public class UI : MonoBehaviour
{
    
    public void OnQuitCLick()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quitte le programme exécutable
        Application.Quit();
#endif

    }

}
