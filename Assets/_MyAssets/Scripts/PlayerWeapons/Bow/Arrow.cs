using UnityEngine;

public class Arrow : MonoBehaviour
{
    private bool _hasHit = false;

    private void Start()
    {
        Destroy(gameObject, 6f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            _hasHit = true;
            Destroy(gameObject);
        }
    }
}