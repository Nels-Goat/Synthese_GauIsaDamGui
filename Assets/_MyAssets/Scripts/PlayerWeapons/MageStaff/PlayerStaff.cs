using UnityEngine;
using UnityEngine.Audio;

public class PlayerStaff : MonoBehaviour
{
    [Header("Attaque")]
    [SerializeField] private float attackCooldown = 3.5f;
    [SerializeField] private GameObject staffAOEPrefab;
    [SerializeField] private int weaponLevel = 1;

    [Header("Audio")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField][Range(0f, 1f)] private float fireVolume = 1f;
    private AudioSource _audioSource;

    private Vector2 lastLookDirection = Vector2.right;
    private Vector3 lastPlayerPosition;
    private Transform player;
    private float nextFireTime; 

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.playOnAwake = false;

        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
        {
            player = target.transform;
            transform.SetParent(player);
            lastPlayerPosition = player.position;
        }

    }

    private void Update()
    {
        if (player == null) return;

        FollowPlayer();

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + attackCooldown;
        }
    }

    private void Shoot()
    {
        Vector2 direction = GetLookDirection();

        SpawnAOE(direction);

        PlayFireSound();
    }

    private void PlayFireSound()
    {
        if (fireSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(fireSound, fireVolume);
        }
    }

    private void SpawnAOE(Vector2 direction)
    {
        Vector3 spawnPos = transform.position + (Vector3)(direction.normalized * 3f);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject aoe = Instantiate(
            staffAOEPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, angle)
        );

        float multiplier = 1f;

        if (weaponLevel == 2) multiplier = 1.5f;
        if (weaponLevel >= 3) multiplier = 2f;

        aoe.transform.localScale *= multiplier;
    }
    private Vector2 RotateDirection(Vector2 direction, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;

        float x = direction.x * Mathf.Cos(rad) - direction.y * Mathf.Sin(rad);
        float y = direction.x * Mathf.Sin(rad) + direction.y * Mathf.Cos(rad);

        return new Vector2(x, y).normalized;
    }

    private void FollowPlayer()
    {
        Vector2 direction = GetLookDirection();

        transform.localPosition = direction.normalized * 0.7f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private Vector2 GetLookDirection()
    {
        Vector2 movementDirection = player.position - lastPlayerPosition;

        if (movementDirection.magnitude > 0.01f)
        {
            lastLookDirection = movementDirection.normalized;
        }

        lastPlayerPosition = player.position;

        return lastLookDirection;
    }

    public void SetWeaponLevel(int level)
    {
        weaponLevel = level;
    }
}
