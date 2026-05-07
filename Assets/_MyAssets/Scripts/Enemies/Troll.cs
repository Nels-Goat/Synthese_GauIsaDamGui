using UnityEngine;

public class Troll : EnemyBase
{
    [Header("Attaque Troll")]
    [SerializeField] private float _chargeSpeed = 12f;
    [SerializeField] private float _detectionRange = 6f;

    private bool _isCharging = false;
    private Vector2 _chargeDirection;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        TrollMovement();
    }

    private void TrollMovement()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);

        if (!_isCharging)
        {
            MoveTowardPlayer();

            if (distance <= _detectionRange)
            {
                _isCharging = true;
                _chargeDirection = ((Vector2)_player.position - (Vector2)transform.position).normalized;
                gameObject.tag = "EnemyAttack"; // Devient dangereux au contact
            }
        }
        else
        {
            transform.Translate(_chargeDirection * _chargeSpeed * Time.deltaTime);
        }

        HandleScreenWrap();
    }

    private void HandleScreenWrap()
    {
        if (transform.position.y < -Camera.main.orthographicSize - 1f)
        {
            float randomX = UnityEngine.Random.Range(
                -Camera.main.orthographicSize * Camera.main.aspect + _halfWidth,
                 Camera.main.orthographicSize * Camera.main.aspect - _halfWidth
            );
            transform.position = new Vector3(randomX, Camera.main.orthographicSize + 2f, 0f);
            _isCharging = false;
            gameObject.tag = "Enemy"; // Redevient normal
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Quand le Troll charge et touche le joueur, le joueur gère les dégâts via son propre OnTriggerEnter2D
        // Ici on détruit seulement le Troll s'il est touché par une attaque du joueur
        if (collision.CompareTag("PlayerAttack"))
        {
            Destroy(collision.gameObject);
            Die("PlayerAttack");
        }
    }
}