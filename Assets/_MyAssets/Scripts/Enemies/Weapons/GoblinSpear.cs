using UnityEngine;
public class GoblinSpear : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    override protected void PlayDeathSound(){}
}