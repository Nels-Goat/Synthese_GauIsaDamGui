using UnityEngine;

public class Goblin : EnemyBase
{
    [Header("Attaque Goblin")]
    [SerializeField] private GameObject _goblinSpear;
    [SerializeField] private float _attackRange = 10f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _spearSpeed = 20f;

    private float _nextAttackTime;

    protected override void Start()
    {
        base.Start();

        // Vérification que la lance a bien le bon tag
        if (_goblinSpear != null && !_goblinSpear.CompareTag("EnemyAttack"))
            Debug.LogWarning("[Goblin] Le prefab _goblinSpear n'a pas le tag 'EnemyAttack' ! Le joueur ne prendra pas de dégâts.");
    }

    private void Update()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);

        if (distance > _attackRange)
            MoveTowardPlayer();
        else
            StopAndAttack();
    }

    private void StopAndAttack()
    {
        if (Time.time >= _nextAttackTime)
        {
            Attack();
            _nextAttackTime = Time.time + _attackCooldown;
        }
    }

    private void Attack()
    {
        Vector2 direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        Vector3 spawnPos = transform.position + (Vector3)(direction * 2.5f);

        GameObject spear = Instantiate(_goblinSpear, spawnPos, Quaternion.identity);

        Rigidbody2D rb = spear.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * _spearSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        spear.transform.rotation = Quaternion.Euler(0, 0, angle);

        Debug.Log($"[Goblin] Lance tirée vers le joueur — direction: {direction}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision);
    }
}