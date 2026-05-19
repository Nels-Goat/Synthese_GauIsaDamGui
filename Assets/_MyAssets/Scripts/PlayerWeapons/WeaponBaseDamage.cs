using UnityEngine;

public abstract class WeaponBaseDamage : MonoBehaviour
{
    [Header("Dégâts")]
    [SerializeField] private float _damage = 1f;

    public float Damage => _damage;

    public virtual void SetDamage(float value)
    {
        _damage = value;
    }
}