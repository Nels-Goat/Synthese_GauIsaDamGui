using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        _inputSystem_Actions = new InputSystem_Actions(); // Initialisation de la classe d'actions d'entrée
        _inputSystem_Actions.Player.Enable();             // Activation des actions d'entrée pour le joueur
        _inputSystem_Actions.Player.Mute.performed += Mute_performed;

        _muteImage.sprite = _spriteOn; // Sprite par défaut : musique active
        _muteImage.gameObject.SetActive(false); // Caché par défaut
        PlayMusicForScene(SceneManager.GetActiveScene().name);
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

        if (sceneName == "Start")
            clipToPlay = _startSceneMusic;
        else if (sceneName == "GameTest_NelsonG" || sceneName == "GameTest_JeremyI" ||
                 sceneName == "GameTest_XavierD" || sceneName == "GameTest_XavierG" ||
                 sceneName == "Game")
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
        if (_isMuted)
        {
            _audioSource.mute = false;
            _muteImage.sprite = _spriteOn;
            _isMuted = false;
        }
        else
        {
            _audioSource.mute = true;
            _muteImage.sprite = _spriteOff;
            _isMuted = true;
        }

        // Affiche le sprite et annule le hide précédent si on rappuie avant 5 sec
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
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _inputSystem_Actions.Player.Mute.performed -= Mute_performed;
        }
    }
}