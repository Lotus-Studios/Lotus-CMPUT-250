using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioLayering : MonoBehaviour
{
    public static AudioLayering Instance { get; private set; }

    public AudioMixer mainMixer; 

    [Header("Audio Sources")]
    public AudioSource chillSource;
    public AudioSource excitingSource;
    public AudioSource cutsceneSource;

    [Header("Audio Clips (Normal)")]
    public AudioClip chillNormal;
    public AudioClip excitingNormal;

    [Header("Audio Clips (Underwater)")]
    public AudioClip chillUnderwater;
    public AudioClip excitingUnderwater;

    [Header("Audio Clips (Cutscene)")]
    public AudioClip goodEndingMusic;
    public AudioClip badEndingMusic;

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

    public IEnumerator SetEnvironmentVolume(float targetVolume) 
    {
        float duration = 0.1f;
        float currentTime = 0f;
        float currentVol;
        mainMixer.GetFloat("EnvironmentVolume", out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            mainMixer.SetFloat("EnvironmentVolume", Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }

    public void SetPlayerVolume(float targetVolume)
    {
        mainMixer.SetFloat("PlayerVolume", Mathf.Log10(targetVolume) * 20);
    }

    // adapted from https://johnleonardfrench.music/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/
    public IEnumerator StartFade(string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        mainMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            mainMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }

    public void PlayGoodEndingMusic(float cutsceneLength) 
    {
        StartCoroutine(CutsceneRoutine(goodEndingMusic, 23.5f));
    }

    public void PlayBadEndingMusic(float cutsceneLength) 
    {
        StartCoroutine(CutsceneRoutine(badEndingMusic, 16.5f));
    }

    public IEnumerator CutsceneRoutine(AudioClip clip, float cutsceneLength) 
    {
        StartCoroutine(StartFade("NormalMusicVolume", 0.5f, 0));
        StartCoroutine(StartFade("SFX", 0.5f, 0));
        cutsceneSource.clip = clip;
        cutsceneSource.Play();
        StartCoroutine(StartFade("CutsceneMusicVolume", 0.1f, 1));
        yield return new WaitForSeconds(clip.length);
        StartCoroutine(StartFade("CutsceneMusicVolume", 0.5f, 0));
        StartCoroutine(StartFade("NormalMusicVolume", 0.5f, 1));
        yield return new WaitForSeconds(cutsceneLength - clip.length);
        StartCoroutine(StartFade("SFX", 0.5f, 1));
        yield break;
    }

}
