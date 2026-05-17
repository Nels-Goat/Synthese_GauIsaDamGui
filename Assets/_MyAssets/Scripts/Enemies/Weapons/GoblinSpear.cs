using UnityEngine;

public class GoblinSpear : EnemyBase
{
    new private void Start()
    {
        Destroy(gameObject, 5f);
    }

    override protected void PlayDeathSound(){}
}