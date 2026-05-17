using UnityEngine;

public class Skeleton : EnemyBase
{
    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (_player == null) return;
        MoveTowardPlayer();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision);
    }

    protected override void PlayDeathSound()
        => SoundManager.Instance?.PlaySkeletonDie();
}