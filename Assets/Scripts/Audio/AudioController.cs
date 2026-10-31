using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    public AudioSource footstepSource;
    public AudioSource jumpSource;
    public AudioSource loopSource;
    public AudioSource dialogueSource;
    public AudioSource UISource;
    public AudioSource platformAudioSourceObject;

    [Header("Player SFX")]
    public AudioClip jumpSound;
    public AudioClip flutterSound;
    public AudioClip[] footstepSounds;

    [Header("UI SFX")]
    public AudioClip clickSound;
    public AudioClip playButtonSound;
    public AudioClip openWoosh;
    public AudioClip closeWoosh;


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
        float finalVolume = isUnderwater ? 0.25f : 0.50f;

        jumpSource.pitch = finalPitch;
        jumpSource.PlayOneShot(jumpSound, finalVolume);
    }

    public void StartFlutterLoop()
    {
        if (loopSource.clip == flutterSound && loopSource.isPlaying) return;

        loopSource.clip = flutterSound;
        loopSource.loop = true;

        // Apply the pitch and volume before starting the loop
        loopSource.pitch = isUnderwater ? 0.85f : 1f;
        loopSource.volume = isUnderwater ? 0.25f : 0.5f;
        loopSource.Play();
    }

    public void StopFlutterLoop()
    {
        if (loopSource.clip == flutterSound && loopSource.isPlaying)
        {
            StartCoroutine(StopAudioSource(loopSource));
        }
    }

    public void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;

        int randomIndex = Random.Range(0, footstepSounds.Length);

        // Base pitch is 1.7 to 1.9, multiplied by 0.85 if underwater
        float basePitch = Random.Range(1.4f, 2.6f);
        footstepSource.pitch = isUnderwater ? basePitch * 0.45f : basePitch;

        float finalVolume = isUnderwater ? 0.1f : 0.2f;
        //audioSource.PlayOneShot(footstepSounds[randomIndex], finalVolume);
        footstepSource.clip = footstepSounds[randomIndex];
        footstepSource.volume = finalVolume;
        footstepSource.Play();
    }



    public void PlayDialogueAudio(AudioClip clip)
    {
        dialogueSource.PlayOneShot(clip, 0.4f);
    }
    public void StopAllAudio()
    {
        if (loopSource != null) loopSource.Stop();
        if (footstepSource != null) footstepSource.Stop();
    }

    public void PlayPlatformSound(AudioClip clip, Vector3 position, float volume, float time) {
        // spawn audio source at position (Quaternion.identity is base rotation)
        AudioSource platformAudioSource = Instantiate(platformAudioSourceObject, position, Quaternion.identity);
        platformAudioSource.clip = clip;
        platformAudioSource.pitch = 1f;
        platformAudioSource.volume = volume;
        // TODO: make output audio mixer group as sfx
        platformAudioSource.Play();
        Destroy(platformAudioSource.gameObject, time);  // destroy audio source after time is passed
    }


    // use for platforms who call this function within an update loop
    public void PlayFromAudioSource(AudioClip clip, AudioSource source) 
    {
        if (!source.isPlaying) {
            source.clip = clip;
            source.Play();
        }
    }

    public void SetAudioSourceProperties(AudioSource source, float volume, float pitch) 
    {
        source.volume = volume;
        source.pitch = pitch;
    }


    // https://johnleonardfrench.music/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/
    // this one's not the best but it works
    public IEnumerator FadeAudioSource(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public IEnumerator StopAudioSource(AudioSource audioSource) 
    {
        float volume = audioSource.volume;
        float duration = 0.05f;
        StartCoroutine(FadeAudioSource(audioSource, duration, 0));
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
        audioSource.volume = volume;
        audioSource.clip = null;
        yield break;
    }

    public void PlayOpenWoosh() {
        UISource.PlayOneShot(openWoosh, 0.3f);
    }
    public void PlayCloseWoosh() {
        UISource.PlayOneShot(closeWoosh, 0.3f);
    }


}
