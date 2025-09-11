using UnityEngine;

public class CharacterSoundManager : MonoBehaviour
{
    [Header("Audio Setup")]
    [SerializeField] AudioSource audioSource;
    
    [Header("Footstep Sounds")]
    [SerializeField] AudioClip leftFootClip;   // step2.mp3
    [SerializeField] AudioClip rightFootClip;  // step1.mp3
    
    [Header("Hit Sounds")]
    [SerializeField] AudioClip[] hitClips;     // Array of 36 hit sounds
    
    [Header("Sound Settings")]
    [SerializeField] float volume = 0.8f;
    [SerializeField] Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    [SerializeField] float minInterval = 0.05f; // Small cooldown to prevent double-triggers
    
    [Header("Debug")]
    [SerializeField] bool enableDebugLog = false;

    float nextAllowedTime = 0f;

    void Awake()
    {
        // Auto-find AudioSource if not assigned
        if (audioSource == null) 
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning($"CharacterSoundManager on {gameObject.name}: No AudioSource found! Please assign one.");
            }
        }
    }

    /// <summary>
    /// Called by Animation Events - plays left foot sound (step2.mp3)
    /// </summary>
    public void PlayLeftFoot()
    {
        PlayFootstepSound(leftFootClip, "Left");
    }

    /// <summary>
    /// Called by Animation Events - plays right foot sound (step1.mp3)
    /// </summary>
    public void PlayRightFoot()
    {
        PlayFootstepSound(rightFootClip, "Right");
    }

    /// <summary>
    /// Legacy method - now calls PlayRightFoot for backward compatibility
    /// </summary>
    public void PlayFootstep()
    {
        PlayRightFoot();
    }

    /// <summary>
    /// Internal method to play footstep sound with validation
    /// </summary>
    void PlayFootstepSound(AudioClip clip, string footType)
    {
        // Check cooldown to prevent double-triggers
        if (Time.time < nextAllowedTime) 
        {
            if (enableDebugLog) Debug.Log($"CharacterSoundManager: Skipping {footType} foot (cooldown) on {gameObject.name}");
            return;
        }

        // Validate components
        if (audioSource == null) 
        {
            if (enableDebugLog) Debug.LogWarning($"CharacterSoundManager: No AudioSource on {gameObject.name}");
            return;
        }

        if (clip == null) 
        {
            if (enableDebugLog) Debug.LogWarning($"CharacterSoundManager: {footType} foot clip is null on {gameObject.name}");
            return;
        }

        // Apply random pitch variation
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        
        // Play the sound
        audioSource.PlayOneShot(clip, volume);
        
        // Set cooldown
        nextAllowedTime = Time.time + minInterval;

        if (enableDebugLog) 
        {
            Debug.Log($"CharacterSoundManager: Played {clip.name} ({footType} foot) on {gameObject.name} (pitch: {audioSource.pitch:F2})");
        }
    }


    /// <summary>
    /// Called by Animation Events - plays a random hit sound
    /// </summary>
    public void PlayHitSound()
    {
        PlayRandomHitSound();
    }

    /// <summary>
    /// Internal method to play random hit sound with validation
    /// </summary>
    void PlayRandomHitSound()
    {
        // Check cooldown to prevent double-triggers
        if (Time.time < nextAllowedTime) 
        {
            if (enableDebugLog) Debug.Log($"CharacterSoundManager: Skipping hit sound (cooldown) on {gameObject.name}");
            return;
        }

        // Validate components
        if (audioSource == null) 
        {
            if (enableDebugLog) Debug.LogWarning($"CharacterSoundManager: No AudioSource on {gameObject.name}");
            return;
        }

        if (hitClips == null || hitClips.Length == 0) 
        {
            if (enableDebugLog) Debug.LogWarning($"CharacterSoundManager: No hit clips assigned on {gameObject.name}");
            return;
        }

        // Select random hit clip
        AudioClip clipToPlay = hitClips[Random.Range(0, hitClips.Length)];
        
        // Apply random pitch variation
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        
        // Play the sound
        audioSource.PlayOneShot(clipToPlay, volume);
        
        // Set cooldown
        nextAllowedTime = Time.time + minInterval;

        if (enableDebugLog) 
        {
            Debug.Log($"CharacterSoundManager: Played hit sound {clipToPlay.name} on {gameObject.name} (pitch: {audioSource.pitch:F2})");
        }
    }

    /// <summary>
    /// Manually set footstep clips at runtime
    /// </summary>
    public void SetFootstepClips(AudioClip leftClip, AudioClip rightClip)
    {
        leftFootClip = leftClip;
        rightFootClip = rightClip;
    }

    /// <summary>
    /// Manually set hit clips at runtime
    /// </summary>
    public void SetHitClips(AudioClip[] clips)
    {
        hitClips = clips;
    }

    /// <summary>
    /// Adjust volume at runtime
    /// </summary>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
    }

    /// <summary>
    /// Check if any sound can be played (not on cooldown)
    /// </summary>
    public bool CanPlaySound()
    {
        return Time.time >= nextAllowedTime;
    }

    /// <summary>
    /// Force reset cooldown (useful for testing)
    /// </summary>
    public void ResetCooldown()
    {
        nextAllowedTime = 0f;
        if (enableDebugLog) Debug.Log($"CharacterSoundManager: Reset cooldown on {gameObject.name}");
    }


    void OnValidate()
    {
        // Ensure pitch range is valid
        if (pitchRange.x > pitchRange.y)
        {
            float temp = pitchRange.x;
            pitchRange.x = pitchRange.y;
            pitchRange.y = temp;
        }
        
        // Clamp pitch range to reasonable values
        pitchRange.x = Mathf.Clamp(pitchRange.x, 0.1f, 3f);
        pitchRange.y = Mathf.Clamp(pitchRange.y, 0.1f, 3f);
        
        // Clamp volume
        volume = Mathf.Clamp01(volume);
        
        // Ensure positive interval
        minInterval = Mathf.Max(0.01f, minInterval);
    }
}
