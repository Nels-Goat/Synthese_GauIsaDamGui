using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Musiques par scène")]
    [SerializeField] private AudioClip _startSceneMusic;
    [SerializeField] private AudioClip _gameSceneMusic;
    [SerializeField] private AudioClip _endSceneMusic;

    [Header("SFX - Ennemis - Goblin")]
    [SerializeField] private AudioClip _goblinDie;
    [SerializeField] private AudioClip _goblinSpearThrow;
    [SerializeField][Range(0f, 1f)] private float _goblinVolume = 1f;

    [Header("SFX - Ennemis - Skeleton")]
    [SerializeField] private AudioClip _skeletonDie;
    [SerializeField][Range(0f, 1f)] private float _skeletonVolume = 1f;

    [Header("SFX - Ennemis - Troll")]
    [SerializeField] private AudioClip _trollDie;
    [SerializeField] private AudioClip _trollDash;
    [SerializeField][Range(0f, 1f)] private float _trollVolume = 1f;

    [Header("SFX - Ennemis - Witch")]
    [SerializeField] private AudioClip _witchDie;
    [SerializeField] private AudioClip _witchSpawnSkeleton;
    [SerializeField][Range(0f, 1f)] private float _witchVolume = 1f;

    [Header("SFX - Armes - Bow")]
    [SerializeField] private AudioClip _bow;
    [SerializeField] private AudioClip _bowHit;
    [SerializeField][Range(0f, 1f)] private float _bowVolume = 1f;

    [Header("SFX - Armes - Staff")]
    [SerializeField] private AudioClip _staffShoot;
    [SerializeField] private AudioClip _staffOnGround;
    [SerializeField][Range(0f, 1f)] private float _staffVolume = 1f;

    [Header("SFX - Armes - Sword")]
    [SerializeField] private AudioClip _swordSwing;
    [SerializeField][Range(0f, 1f)] private float _swordVolume = 1f;

    [Header("SFX - Joueur")]
    [SerializeField] private AudioClip _dash;
    [SerializeField][Range(0f, 1f)] private float _dashVolume = 0.8f;
    [SerializeField] private AudioClip _footstep;
    [SerializeField][Range(0f, 1f)] private float _footstepVolume = 0.4f;
    [SerializeField] private AudioClip _playerGetHit;
    [SerializeField][Range(0f, 1f)] private float _getHitVolume = 0.9f;

    [Header("SFX - Menu")]
    [SerializeField] private AudioClip _menuHover;
    [SerializeField] private AudioClip _menuSubmit;
    [SerializeField][Range(0f, 1f)] private float _menuVolume = 1f;

    [Header("Mute UI")]
    [SerializeField] private Image _muteImage;
    [SerializeField] private Sprite _volumeOn;
    [SerializeField] private Sprite _volumeOff;

    [Header("Volume Master")]
    [SerializeField][Range(0f, 1f)] private float _musicVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _sfxVolume = 1f;

    private InputSystem_Actions _inputSystem_Actions;
    private AudioSource _audioSource;
    private AudioSource _sfxSource;
    private bool _isMusicMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = true;
            _audioSource.volume = _musicVolume;

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop = false;
            _sfxSource.volume = _sfxVolume;

            SceneManager.sceneLoaded += OnSceneLoaded;
            PlayMusicForScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        _inputSystem_Actions = new InputSystem_Actions();
        _inputSystem_Actions.Player.Enable();
        _inputSystem_Actions.Player.Mute.performed += Mute_performed;

        if (_muteImage != null)
            _muteImage.gameObject.SetActive(false);
    }


    private void Mute_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        => OnMuteClick();

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        => PlayMusicForScene(scene.name);

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = sceneName switch
        {
            "Start" => _startSceneMusic,
            "Game" => _gameSceneMusic,
            "End" => _endSceneMusic,
            _ => null
        };

        if (clipToPlay == null) { _audioSource.Stop(); return; }
        if (_audioSource.clip == clipToPlay && _audioSource.isPlaying) return;

        _audioSource.clip = clipToPlay;
        _audioSource.Play();
    }

    // --- Ennemis ---

    public void PlayGoblinDie() => PlaySFX(_goblinDie, _goblinVolume);
    public void PlayGoblinSpearThrow() => PlaySFX(_goblinSpearThrow, _goblinVolume);

    public void PlaySkeletonDie() => PlaySFX(_skeletonDie, _skeletonVolume);

    public void PlayTrollDie() => PlaySFX(_trollDie, _trollVolume);
    public void PlayTrollDash() => PlaySFX(_trollDash, _trollVolume);

    public void PlayWitchDie() => PlaySFX(_witchDie, _witchVolume);
    public void PlayWitchSpawnSkeleton() => PlaySFX(_witchSpawnSkeleton, _witchVolume);

    // --- Armes ---

    public void PlayBow() => PlaySFX(_bow, _bowVolume);
    public void PlayBowHit() => PlaySFX(_bowHit, _bowVolume);

    public void PlayStaffShoot() => PlaySFX(_staffShoot, _staffVolume);
    public void PlayStaffOnGround() => PlaySFX(_staffOnGround, _staffVolume);

    public void PlaySwordSwing() => PlaySFX(_swordSwing, _swordVolume);

    // --- Joueur ---

    public void PlayDash() => PlaySFX(_dash, _dashVolume);
    public void PlayFootstep() => PlaySFX(_footstep, _footstepVolume);
    public void PlayPlayerGetHit() => PlaySFX(_playerGetHit, _getHitVolume);

    // --- Menu ---

    public void PlayMenuHover() => PlaySFX(_menuHover, _menuVolume);
    public void PlayMenuSubmit() => PlaySFX(_menuSubmit, _menuVolume);

    // --- Core ---

    private void PlaySFX(AudioClip clip, float volume)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, volume * _sfxVolume);
    }

    public void OnMuteClick()
    {
        _isMusicMuted = !_isMusicMuted;
        _audioSource.mute = _isMusicMuted;

        if (_muteImage != null)
        {
            _muteImage.sprite = _isMusicMuted ? _volumeOff : _volumeOn;
            _muteImage.gameObject.SetActive(true);
            CancelInvoke(nameof(HideMuteUI));
            Invoke(nameof(HideMuteUI), 3f);
        }
    }

    private void HideMuteUI()
    {
        if (_muteImage != null)
            _muteImage.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_inputSystem_Actions != null)
                _inputSystem_Actions.Player.Mute.performed -= Mute_performed;
        }
    }
}