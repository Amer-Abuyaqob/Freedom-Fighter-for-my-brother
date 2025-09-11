using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MusicTrack
{
    [Header("Track Info")]
    public string trackName;
    public AudioClip audioClip;
    
    [Header("Settings")]
    public bool loop = true;
    public float volume = 1f;
    public float fadeInTime = 2f;
    public float fadeOutTime = 2f;
}

public class MusicManager : MonoBehaviour
{
    [Header("Music Tracks")]
    [SerializeField] MusicTrack backgroundMusic; // For MainMenu, Intro_Lv1, Level1
    [SerializeField] MusicTrack sadMusic; // For GameOver, Outro_Lv1
    
    [Header("Audio Settings")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] float masterVolume = 0.7f;
    [SerializeField] bool playOnStart = true;
    
    [Header("Scene Music Mapping")]
    [SerializeField] string[] backgroundMusicScenes = { "MainMenu", "Intro_Lv1", "Level1" };
    [SerializeField] string[] sadMusicScenes = { "GameOver", "Outro_Lv1" };
    
    [Header("Debug")]
    [SerializeField] bool enableDebugLog = false;
    
    public static MusicManager Instance { get; private set; }
    
    MusicTrack currentTrack;
    bool isFading = false;
    
    void Awake()
    {
        // Singleton pattern - persist across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Setup audio source if not assigned
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
            
            // Configure audio source
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.volume = masterVolume;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        if (playOnStart)
        {
            PlayMusicForCurrentScene();
        }
    }
    
    void OnEnable()
    {
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (enableDebugLog)
        {
            Debug.Log($"MusicManager: Scene loaded: {scene.name}");
        }
        
        // Play appropriate music for the new scene
        PlayMusicForCurrentScene();
    }
    
    void PlayMusicForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        if (enableDebugLog)
        {
            Debug.Log($"MusicManager: Playing music for scene: {currentSceneName}");
        }
        
        // Check if we're entering a background music scene
        if (IsSceneInArray(currentSceneName, backgroundMusicScenes))
        {
            // If we're not already playing background music, start it from the beginning
            if (currentTrack != backgroundMusic || !audioSource.isPlaying)
            {
                PlayTrackFromBeginning(backgroundMusic);
            }
            // If we're already playing background music, keep it playing (don't restart)
        }
        // Check if we're entering a sad music scene
        else if (IsSceneInArray(currentSceneName, sadMusicScenes))
        {
            // Always start sad music from the beginning
            PlayTrackFromBeginning(sadMusic);
        }
        // If we're entering any other scene, stop music completely
        else
        {
            StopMusic();
        }
    }
    
    bool IsSceneInArray(string sceneName, string[] sceneArray)
    {
        foreach (string scene in sceneArray)
        {
            if (sceneName == scene)
            {
                return true;
            }
        }
        return false;
    }
    
    public void PlayTrack(MusicTrack track)
    {
        if (track == null || track.audioClip == null)
        {
            if (enableDebugLog)
            {
                Debug.LogWarning("MusicManager: Cannot play track - track or audioClip is null");
            }
            return;
        }
        
        // If it's the same track and already playing, don't restart
        if (currentTrack == track && audioSource.isPlaying && audioSource.clip == track.audioClip)
        {
            if (enableDebugLog)
            {
                Debug.Log($"MusicManager: Track '{track.trackName}' is already playing");
            }
            return;
        }
        
        // If we're switching tracks, fade out current track first
        if (currentTrack != null && audioSource.isPlaying)
        {
            StartCoroutine(FadeOutAndPlayNewTrack(track));
        }
        else
        {
            // Play immediately
            PlayTrackImmediate(track);
        }
    }
    
    void PlayTrackFromBeginning(MusicTrack track)
    {
        if (track == null || track.audioClip == null)
        {
            if (enableDebugLog)
            {
                Debug.LogWarning("MusicManager: Cannot play track from beginning - track or audioClip is null");
            }
            return;
        }
        
        // Stop current music immediately
        audioSource.Stop();
        
        // Play new track from beginning
        currentTrack = track;
        audioSource.clip = track.audioClip;
        audioSource.loop = track.loop;
        audioSource.volume = track.volume * masterVolume;
        audioSource.time = 0f; // Start from beginning
        audioSource.Play();
        
        if (enableDebugLog)
        {
            Debug.Log($"MusicManager: Playing track '{track.trackName}' from beginning");
        }
    }
    
    void PlayTrackImmediate(MusicTrack track)
    {
        currentTrack = track;
        audioSource.clip = track.audioClip;
        audioSource.loop = track.loop;
        audioSource.volume = track.volume * masterVolume;
        audioSource.Play();
        
        if (enableDebugLog)
        {
            Debug.Log($"MusicManager: Playing track '{track.trackName}'");
        }
    }
    
    System.Collections.IEnumerator FadeOutAndPlayNewTrack(MusicTrack newTrack)
    {
        isFading = true;
        
        // Fade out current track
        float startVolume = audioSource.volume;
        float fadeOutTime = currentTrack != null ? currentTrack.fadeOutTime : 1f;
        
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeOutTime;
            yield return null;
        }
        
        // Stop current track
        audioSource.Stop();
        
        // Play new track
        PlayTrackImmediate(newTrack);
        
        // Fade in new track
        float targetVolume = newTrack.volume * masterVolume;
        float fadeInTime = newTrack.fadeInTime;
        
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += targetVolume * Time.deltaTime / fadeInTime;
            yield return null;
        }
        
        audioSource.volume = targetVolume;
        isFading = false;
        
        if (enableDebugLog)
        {
            Debug.Log($"MusicManager: Fade complete. Now playing '{newTrack.trackName}'");
        }
    }
    
    // Public methods for external control
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            PlayTrack(backgroundMusic);
        }
    }
    
    public void PlaySadMusic()
    {
        if (sadMusic != null)
        {
            PlayTrack(sadMusic);
        }
    }
    
    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            currentTrack = null;
            
            if (enableDebugLog)
            {
                Debug.Log("MusicManager: Music stopped");
            }
        }
    }
    
    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            
            if (enableDebugLog)
            {
                Debug.Log("MusicManager: Music paused");
            }
        }
    }
    
    public void ResumeMusic()
    {
        if (!audioSource.isPlaying && currentTrack != null)
        {
            audioSource.UnPause();
            
            if (enableDebugLog)
            {
                Debug.Log("MusicManager: Music resumed");
            }
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        
        if (!isFading && audioSource.isPlaying)
        {
            audioSource.volume = currentTrack.volume * masterVolume;
        }
        
        if (enableDebugLog)
        {
            Debug.Log($"MusicManager: Master volume set to {masterVolume}");
        }
    }
    
    public void SetTrackVolume(string trackName, float volume)
    {
        volume = Mathf.Clamp01(volume);
        
        if (backgroundMusic.trackName == trackName)
        {
            backgroundMusic.volume = volume;
        }
        else if (sadMusic.trackName == trackName)
        {
            sadMusic.volume = volume;
        }
        
        // Update current track volume if it's playing
        if (currentTrack != null && currentTrack.trackName == trackName && audioSource.isPlaying)
        {
            audioSource.volume = volume * masterVolume;
        }
        
        if (enableDebugLog)
        {
            Debug.Log($"MusicManager: Volume for '{trackName}' set to {volume}");
        }
    }
    
    // Getters for external systems
    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
    
    public string GetCurrentTrackName()
    {
        return currentTrack != null ? currentTrack.trackName : "None";
    }
    
    public float GetMasterVolume()
    {
        return masterVolume;
    }
    
    // Method to manually trigger music for current scene (for testing)
    [ContextMenu("Play Music for Current Scene")]
    public void PlayMusicForCurrentSceneManual()
    {
        PlayMusicForCurrentScene();
    }
    
    // Method to test specific tracks
    [ContextMenu("Test Background Music")]
    public void TestBackgroundMusic()
    {
        PlayBackgroundMusic();
    }
    
    [ContextMenu("Test Sad Music")]
    public void TestSadMusic()
    {
        PlaySadMusic();
    }
}
