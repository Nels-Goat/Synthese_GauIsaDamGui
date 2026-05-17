using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSound : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        SoundManager.Instance?.PlayMenuHover();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        SoundManager.Instance?.PlayMenuSubmit();
    }
}