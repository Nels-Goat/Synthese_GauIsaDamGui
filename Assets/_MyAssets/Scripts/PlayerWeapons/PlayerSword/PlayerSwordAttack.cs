using UnityEngine;

public class PlayerSwordAttack : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 0.1f);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
