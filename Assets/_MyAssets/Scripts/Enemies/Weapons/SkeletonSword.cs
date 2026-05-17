using UnityEngine;

public class SkeletonSword : MonoBehaviour
{
    public int Damage { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<IDamageable>()?.TakeHit("EnemyAttack");
        }
    }
}