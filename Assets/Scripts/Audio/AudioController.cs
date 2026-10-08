using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    public AudioSource audioSource;
    public AudioSource loopSource;

    [Header("Player SFX")]
    public AudioClip jumpSound;
    public AudioClip flutterSound;
    public AudioClip[] footstepSounds;

    private bool isUnderwater = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void SetUnderwater(bool underwater)
    {
        if (isUnderwater == underwater) return;
        isUnderwater = underwater;

        // drop or raise the pitch and volume of the flutter loop if it is currently playing
        if (loopSource.isPlaying && loopSource.clip == flutterSound)
        {
            loopSource.pitch = isUnderwater ? 0.85f : 1f;
            loopSource.volume = isUnderwater ? 0.85f : 1f;
        }
    }

    public void PlayJump(float pitch = 1f)
    {
        float finalPitch = isUnderwater ? pitch * 0.85f : pitch;
        float finalVolume = isUnderwater ? 0.85f : 1f;

        audioSource.pitch = finalPitch;
        audioSource.PlayOneShot(jumpSound, finalVolume);
    }

    public void StartFlutterLoop()
    {
        if (loopSource.clip == flutterSound && loopSource.isPlaying) return;

        loopSource.clip = flutterSound;
        loopSource.loop = true;

        // Apply the pitch and volume before starting the loop
        loopSource.pitch = isUnderwater ? 0.85f : 1f;
        loopSource.volume = isUnderwater ? 0.85f : 1f;
        loopSource.Play();
    }

    public void StopFlutterLoop()
    {
        if (loopSource.clip == flutterSound && loopSource.isPlaying)
        {
            loopSource.Stop();
            loopSource.clip = null;
        }
    }

    public void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;

        int randomIndex = Random.Range(0, footstepSounds.Length);

        // Base pitch is 1.7 to 1.9, multiplied by 0.85 if underwater
        float basePitch = Random.Range(1.7f, 1.9f);
        audioSource.pitch = isUnderwater ? basePitch * 0.85f : basePitch;

        float finalVolume = isUnderwater ? 0.65f : 1f;
        audioSource.PlayOneShot(footstepSounds[randomIndex], finalVolume);
    }
}