using UnityEngine;

public class StaffAOE : WeaponBaseDamage
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
            if (other.TryGetComponent<IDamageable>(out var target))
                target.TakeHit("PlayerAttack", Damage);
        }
    }
}
