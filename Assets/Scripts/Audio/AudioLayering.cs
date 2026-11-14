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
    public AudioSource chillUnderwaterSource;
    public AudioSource excitingUnderwaterSource;

    [Header("Audio Clips (Normal)")]
    public AudioClip chillNormal;
    public AudioClip excitingNormal;

    [Header("Audio Clips (Underwater)")]
    public AudioClip chillUnderwater;
    public AudioClip excitingUnderwater;

    [Header("Settings")]
    public float fadeSpeed = 0.5f;
    public float fadeDuration = 0.5f;

    private bool isExciting = false;
    private bool isUnderwater = false;
    private AudioSource currentSource;
    private AudioSource targetSource;
    private IEnumerator lastCurrentCoroutine = null;
    private IEnumerator lastTargetCoroutine = null;

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
        chillUnderwaterSource.PlayScheduled(syncTime);
        excitingUnderwaterSource.PlayScheduled(syncTime);

        chillSource.volume = 0f;
        excitingSource.volume = 0f;

        currentSource = chillSource;
        targetSource = chillSource;
        if (AudioController.Instance != null) {
            StartCoroutine(AudioController.Instance.FadeAudioSource(currentSource, 3f, 1f));
        } else {
            chillSource.volume = 1f;
        }
    }



    void Update()
    {
        targetSourceCheck();

        if (targetSource != currentSource) 
        {
            if (AudioController.Instance != null)
            {
                lastCurrentCoroutine = AudioController.Instance.FadeAudioSource(currentSource, fadeDuration, 0f);
                lastTargetCoroutine = AudioController.Instance.FadeAudioSource(targetSource, fadeDuration, 1f);
                StartCoroutine(lastCurrentCoroutine);
                StartCoroutine(lastTargetCoroutine);
            }
            currentSource = targetSource;
        }
    }

    private void targetSourceCheck() {
        if (isExciting) 
        {
            if (isUnderwater) targetSource = excitingUnderwaterSource;
            else targetSource = excitingSource;
        } 
        else 
        {
            if (isUnderwater) targetSource = chillUnderwaterSource;
            else targetSource = chillSource;
        }
    }

    private void targetSourceCheck2() {
        if (isExciting && !isUnderwater) 
        {
            targetSource = excitingSource;
        } 
        else 
        {
            targetSource = excitingUnderwaterSource;
        }
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
        /*
        SwapClip(chillSource, underwater ? chillUnderwater : chillNormal);
        SwapClip(excitingSource, underwater ? excitingUnderwater : excitingNormal);
        */
    }

    private void SwapClip(AudioSource source, AudioClip newClip)
    {
        // saves the time then swaps the clips and plays it
        // h
        float currentTime = source.time;
        source.clip = newClip;
        source.time = currentTime;
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
}
