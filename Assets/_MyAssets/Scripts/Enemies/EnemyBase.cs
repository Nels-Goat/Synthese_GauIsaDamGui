using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Propriétés communes")]
    [SerializeField] protected float _moveSpeed = 2f;
    [SerializeField] protected int _points = 10;
    [SerializeField] protected int _maxLife = 1;
    [SerializeField] protected int _damage = 1;

    protected int _currentLife;
    protected Transform _player;
    protected SpriteRenderer _spriteRenderer;
    protected float _halfWidth;
    protected float _halfHeight;

    protected virtual void Start()
    {
        _currentLife = _maxLife;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _halfWidth = _spriteRenderer.bounds.extents.x;
        _halfHeight = _spriteRenderer.bounds.extents.y;

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null)
            _player = target.transform;
        else
            Debug.LogWarning($"[{gameObject.name}] Aucun joueur trouvé avec le tag 'Player'.");
    }

    protected void MoveTowardPlayer()
    {
        if (_player == null) return;

        Vector2 direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * _moveSpeed * Time.deltaTime);

        transform.localScale = new Vector3(
            direction.x > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    protected void HandleCollision(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyAttack") || collision.CompareTag("PowerUp"))
            return;

        if (collision.CompareTag("PlayerAttack"))
        {
            if (collision.gameObject.GetComponent<StaffAOE>() == null)
            {
                Destroy(collision.gameObject);
            }

            Die("PlayerAttack");
        }
        else if (collision.CompareTag("Player"))
        {
            TakeHit("Player");
        }
    }

    // Interface IDamageable
    public void TakeHit(string attackerTag)
    {
        _currentLife--;
        Debug.Log($"[{gameObject.name}] TakeHit() — Vie : {_currentLife}/{_maxLife} — tag: {attackerTag}");

        if (_currentLife <= 0)
            Die(attackerTag);
    }

    protected void Die(string collidedTag)
    {
        Debug.Log($"[{gameObject.name}] Die() — tag: {collidedTag} — +{_points} pts | Dégâts: {_damage}");
        GameManager.Instance.EnemyDestroyed(_points, collidedTag, _damage); // _damage passé ici
        Destroy(gameObject);
    }
}