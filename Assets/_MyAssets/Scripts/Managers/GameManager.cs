using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject _background;
    private float _minX, _maxX, _minY, _maxY;

    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private int _maxEnemy = 20;

    private int _playerScore = 0;
    public int PlayerScore {get; set;}




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpriteRenderer backgroundRenderer = _background.GetComponent<SpriteRenderer>();
        _minX = backgroundRenderer.bounds.min.x;
        _maxX = backgroundRenderer.bounds.max.x;
        _minY = backgroundRenderer.bounds.min.y;
        _maxY = backgroundRenderer.bounds.max.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsEnemyMaxed()
    {
        int numEnemy = _enemyContainer.transform.childCount;
        return numEnemy >= _maxEnemy;
    }

    public float ClampX(float coo, float half)
    {
        return Mathf.Clamp(coo, _minX + half, _maxX - half);
    }

    public float ClampY(float coo, float half)
    {
        return Mathf.Clamp(coo, _minY + half, _maxY - half);
    }
}
