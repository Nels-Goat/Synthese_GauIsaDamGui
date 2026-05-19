using UnityEngine;

public class GoblinSpear : MonoBehaviour
{
    public int Damage { get; set; }

    private void Start()
    {
        Destroy(gameObject, 5f);
    }
}