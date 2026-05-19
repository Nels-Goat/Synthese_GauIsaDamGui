using System.Collections;
using UnityEngine;

public class UI : MonoBehaviour
{
    
    public void OnQuitCLick()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quitte le programme ex�cutable
        //Application.Quit();
        StartCoroutine(QuitterBorne());
#endif

    }

    private IEnumerator QuitterBorne()
    {
        try
        {
            string cheminPortail = System.IO.Path.Combine(Application.dataPath, "../../Portail/Portail.exe");
            System.Diagnostics.Process.Start(cheminPortail);
        } catch (System.Exception e)
        {
            Debug.LogError("Impossible de lancer le portail : " + e.Message);
        }
        yield return new WaitForSeconds(.5f);
        Application.Quit();
    }

}
