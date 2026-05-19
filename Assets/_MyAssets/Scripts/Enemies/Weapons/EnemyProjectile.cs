using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int _damage = 1;
    public int Damage { get => _damage; set => _damage = value; }
}