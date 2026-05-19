using UnityEngine;

public class Arrow : WeaponBaseDamage
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
            if (other.TryGetComponent<IDamageable>(out var target))
                target.TakeHit("PlayerAttack", Damage);

            _hasHit = true;
            Destroy(gameObject);
        }
    }
}