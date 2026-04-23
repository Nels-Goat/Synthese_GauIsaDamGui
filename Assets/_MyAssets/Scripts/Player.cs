using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Propriétés Joueur")]
    [SerializeField] private float _playerSpeed = 5f;

    [Header("Limites de la map")]
    [SerializeField] private GameObject _background;
    private float _minX, _maxX, _minY, _maxY;

    private InputSystem_Actions _inputSystemActions;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        // Liaison avec les input actions
        _inputSystemActions = new InputSystem_Actions();
        _inputSystemActions.Player.Enable();

        _rigidbody2D = GetComponent<Rigidbody2D>();

        // Permet de calculer la largeur et hauteur de mon joueur
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Calcule les limites automatiquement depuis le Background
        SpriteRenderer backgroundRenderer = _background.GetComponent<SpriteRenderer>();
        _minX = backgroundRenderer.bounds.min.x;
        _maxX = backgroundRenderer.bounds.max.x;
        _minY = backgroundRenderer.bounds.min.y;
        _maxY = backgroundRenderer.bounds.max.y;
    }

    private void OnDestroy()
    {
        _inputSystemActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector2 direction2D = _inputSystemActions.Player.Move.ReadValue<Vector2>();
        direction2D.Normalize();

        Vector2 newPosition = _rigidbody2D.position + direction2D * Time.fixedDeltaTime * _playerSpeed;

        float halfPlayerWidth = _spriteRenderer.bounds.extents.x;
        float halfPlayerHeight = _spriteRenderer.bounds.extents.y;

        float clampedX = Mathf.Clamp(newPosition.x, _minX + halfPlayerWidth, _maxX - halfPlayerWidth);
        float clampedY = Mathf.Clamp(newPosition.y, _minY + halfPlayerHeight, _maxY - halfPlayerHeight);

        _rigidbody2D.MovePosition(new Vector2(clampedX, clampedY));
    }
}