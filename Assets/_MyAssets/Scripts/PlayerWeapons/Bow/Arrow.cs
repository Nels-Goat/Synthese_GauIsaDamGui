using UnityEngine;

public class Arrow : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 6f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}

