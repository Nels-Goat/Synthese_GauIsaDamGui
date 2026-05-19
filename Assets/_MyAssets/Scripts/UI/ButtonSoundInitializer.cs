using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundInitializer : MonoBehaviour
{
    private void Start()
    {
        foreach (Button btn in GetComponentsInChildren<Button>())
        {
            btn.gameObject.AddComponent<ButtonHoverSound>();
        }
    }
}