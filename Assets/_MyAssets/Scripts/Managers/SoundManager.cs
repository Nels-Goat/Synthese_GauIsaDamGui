using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Musiques par scène")]
    public AudioClip startSceneMusic;
    public AudioClip gameSceneMusic;
    public AudioClip endSceneMusic;

    [Range(0f, 1f)] public float volume = 0.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = true;
            _audioSource.volume = volume;

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
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        if (sceneName == "Start") clipToPlay = startSceneMusic;
        else if (sceneName == "GameTest_NelsonG" || sceneName == "GameTest_JeremyI" || sceneName == "GameTest_XavierD" || sceneName == "GameTest_XavierG" || sceneName == "Game")
            clipToPlay = gameSceneMusic;
        else if (sceneName == "End") clipToPlay = endSceneMusic;

        if (clipToPlay == null)
        {
            _audioSource.Stop();
            return;
        }

        if (_audioSource.clip == clipToPlay && _audioSource.isPlaying) return;

        _audioSource.clip = clipToPlay;
        _audioSource.Play();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}