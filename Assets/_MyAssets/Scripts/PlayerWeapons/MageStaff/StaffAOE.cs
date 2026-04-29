using UnityEngine;

public class StaffAOE : MonoBehaviour
{
    [SerializeField] private float lifeTime = 7f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
