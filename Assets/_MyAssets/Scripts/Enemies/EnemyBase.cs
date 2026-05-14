using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Propri�t�s communes")]
    [SerializeField] protected float _moveSpeed = 2f;
    [SerializeField] protected int _points = 10;

    protected Transform _player;
    protected SpriteRenderer _spriteRenderer;
    protected float _halfWidth;
    protected float _halfHeight;

    protected virtual void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _halfWidth = _spriteRenderer.bounds.extents.x;
        _halfHeight = _spriteRenderer.bounds.extents.y;

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null)
            _player = target.transform;
    }

    protected void MoveTowardPlayer()
    {
        if (_player == null) return;

        Vector2 direction = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * _moveSpeed * Time.deltaTime);

        /*
        transform.localScale = new Vector3(
            direction.x > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );*/
    }

    protected void HandleCollision(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyAttack") || collision.CompareTag("PowerUp"))
            return;

        if (collision.CompareTag("PlayerAttack"))
        {
            Destroy(collision.gameObject);
            Die("PlayerAttack");
        }
        else if (collision.CompareTag("Player"))
        {
            Die("Player");
        }
    }

    protected void Die(string collidedTag)
    {
        GameManager.Instance.EnemyDestroyed(_points, collidedTag);
        Destroy(gameObject);
    }
}