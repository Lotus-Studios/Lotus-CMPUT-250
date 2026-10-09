using UnityEngine;

public class AudioLayering : MonoBehaviour
{
    public static AudioLayering Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource chillSource;
    public AudioSource excitingSource;

    [Header("Audio Clips (Normal)")]
    public AudioClip chillNormal;
    public AudioClip excitingNormal;

    [Header("Audio Clips (Underwater)")]
    public AudioClip chillUnderwater;
    public AudioClip excitingUnderwater;

    [Header("Settings")]
    public float fadeSpeed = 0.5f;

    private bool isExciting = false;
    private bool isUnderwater = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // since swapping tracks the first time causes a slight stutter, didn't know unity had this lmao
        chillUnderwater.LoadAudioData();
        excitingUnderwater.LoadAudioData();
        chillSource.clip = chillNormal;
        excitingSource.clip = excitingNormal;

        // they start at the same time, this is here incase the startup is messed up a bit
        double syncTime = AudioSettings.dspTime + 0.1;
        chillSource.PlayScheduled(syncTime);
        excitingSource.PlayScheduled(syncTime);

        chillSource.volume = 1f;
        excitingSource.volume = 0f;
    }

    void Update()
    {
        float targetChill = isExciting ? 0f : 1f;
        float targetExciting = isExciting ? 1f : 0f;

        chillSource.volume = Mathf.MoveTowards(chillSource.volume, targetChill, fadeSpeed * Time.deltaTime);
        excitingSource.volume = Mathf.MoveTowards(excitingSource.volume, targetExciting, fadeSpeed * Time.deltaTime);
    }

    public void FadeToExciting()
    {
        isExciting = true;
    }
    public void FadeToChill()
    {
        isExciting = false;
    }

    public void SetUnderwater(bool underwater)
    {
        if (isUnderwater == underwater) return;

        isUnderwater = underwater;

        // Swap the audio file inside both sources
        SwapClip(chillSource, underwater ? chillUnderwater : chillNormal);
        SwapClip(excitingSource, underwater ? excitingUnderwater : excitingNormal);
    }

    private void SwapClip(AudioSource source, AudioClip newClip)
    {
        // saves the time then swaps the clips and plays it
        int currentSample = source.timeSamples;
        source.clip = newClip;
        source.timeSamples = currentSample;
        source.Play();
    }
}