using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/*
 * https://youtu.be/DU7cgVsU2rM  - Sasquatch B Studios - How To Add Sound Effects the RIGHT Way | Unity Tutorial 
 * https://www.reddit.com/r/Unity3D/comments/1grzlep/how_would_you_handle_audio_in_your_game/ - SupraOrbitalStudios - (general implementation)
 */
public class AudioController : MonoBehaviour
{
    public static AudioController _instance;
    public static AudioController Instance { get { return _instance; } }

    public AudioSource audioSource;
    public AudioSource musicSource;
    public AudioSource loopSource;

    public AudioSource soundFXObject;

    [Header("Player SFX")]
    public AudioClip jumpSound;
    public AudioClip flutterSound;
    public AudioClip[] footstepSounds; 
    
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] public float underwaterLowPassSpeed = 10;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

    public void PlayJump(float pitch = 1f)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(jumpSound);
    }

    public void StartFlutterLoop()
    {

        if (loopSource.clip == flutterSound && loopSource.isPlaying) return;

        loopSource.clip = flutterSound;
        loopSource.loop = true;
        loopSource.Play();
        
    }

    public void StopFlutterLoop()
    {
        // Only stop the audio if it is currently playing the flutter sound
        if (loopSource.clip == flutterSound && loopSource.isPlaying)
        {
            loopSource.Stop();
            loopSource.clip = null;
        }
    }

    public void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;

        // Choose random footstep sound so its somewhat unique and modify pitch a bit each time
        int randomIndex = Random.Range(0, footstepSounds.Length);
        audioSource.pitch = Random.Range(1.7f, 1.9f);

        audioSource.PlayOneShot(footstepSounds[randomIndex]);
    }

    public void PlaySoundEffectIn3D(SoundEffect sound, Transform spawnTransform) 
    {
        // set up audio source
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity); 
        audioSource.clip = sound.audioClips[0];
        audioSource.volume = sound.volume;
        audioSource.pitch = sound.pitch;

        // play and destroy audio source
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlaySoundEffect(SoundEffect sound) 
    {
        //AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity); 

        audioSource.clip = sound.audioClips[0];

        audioSource.volume = sound.volume;
        audioSource.pitch = sound.pitch;

        audioSource.Play();

        /*j
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
        */
    }

    public void AddUnderwaterEffect() {
        float lowPassFrequency;
        audioMixer.GetFloat("underwaterLowPass", out lowPassFrequency);
        lowPassFrequency = Mathf.Lerp(lowPassFrequency, 750f, Time.deltaTime * underwaterLowPassSpeed);
        audioMixer.SetFloat("underwaterLowPass", lowPassFrequency);
    }

    public void RemoveUnderwaterEffect() {
        float lowPassFrequency;
        audioMixer.GetFloat("underwaterLowPass", out lowPassFrequency);
        lowPassFrequency = Mathf.Lerp(lowPassFrequency, 22000f, Time.deltaTime * 2f);   // remove at a slower speed
        audioMixer.SetFloat("underwaterLowPass", lowPassFrequency);
    }


}
