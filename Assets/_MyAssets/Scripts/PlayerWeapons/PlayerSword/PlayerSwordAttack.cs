using UnityEngine;

public class PlayerSwordAttack : WeaponBaseDamage
{
    private void Start()
    {
        Destroy(gameObject, 0.1f);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<IDamageable>(out var target))
                target.TakeHit("PlayerAttack", Damage);
        }
    }
}
