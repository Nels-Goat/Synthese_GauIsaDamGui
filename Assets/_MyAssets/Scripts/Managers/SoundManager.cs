using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))] // Fix Bug 1 : garantit qu'un AudioSource existe
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Musiques par scène")]
    [SerializeField] private AudioClip _startSceneMusic;
    [SerializeField] private AudioClip _gameSceneMusic;
    [SerializeField] private AudioClip _endSceneMusic;

    [Header("Mute UI")]
    [SerializeField] private Image _muteImage;
    [SerializeField] private Sprite _spriteOn;
    [SerializeField] private Sprite _spriteOff;
    [SerializeField][Range(0f, 1f)] private float _volume = 0.5f;

    private InputSystem_Actions _inputSystem_Actions;
    private AudioSource _audioSource;
    private bool _isMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = true;
            _audioSource.volume = _volume;
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
        _muteImage.sprite = _spriteOn;
        _muteImage.gameObject.SetActive(false);
    }

    private void Mute_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnMuteClick();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        if (sceneName == "StartTest_NelsonG")
            clipToPlay = _startSceneMusic;
        else if (sceneName == "GameTest_NelsonG")
            clipToPlay = _gameSceneMusic;
        else if (sceneName == "End")
            clipToPlay = _endSceneMusic;

        if (clipToPlay == null)
        {
            _audioSource.Stop();
            return;
        }

        if (_audioSource.clip == clipToPlay && _audioSource.isPlaying) return;

        _audioSource.clip = clipToPlay;
        _audioSource.Play();
    }

    public void OnMuteClick()
    {
        _isMuted = !_isMuted;
        _audioSource.mute = _isMuted;
        _muteImage.sprite = _isMuted ? _spriteOff : _spriteOn;

        _muteImage.gameObject.SetActive(true);
        CancelInvoke(nameof(HideMuteImage));
        Invoke(nameof(HideMuteImage), 5f);
    }

    private void HideMuteImage()
    {
        _muteImage.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // Fix Bug 2 : vérification null avant d'utiliser _inputSystem_Actions
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (_inputSystem_Actions != null)
            {
                _inputSystem_Actions.Player.Mute.performed -= Mute_performed;
            }
        }
    }
}