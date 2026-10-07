using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class UnderwaterAudio : MonoBehaviour
{
    public static UnderwaterAudio _instance;
    public static UnderwaterAudio Instance { get { return _instance; } }

    public AudioLowPassFilter lowPassFilter;

    [SerializeField] private float normalFrequency = 22000f;
    [SerializeField] private float underwaterFrequency = 2000f;
    [SerializeField] private float transitionSpeed = 8f;

    private float targetFrequency;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        lowPassFilter = GetComponent<AudioLowPassFilter>();
        targetFrequency = normalFrequency;
        lowPassFilter.cutoffFrequency = normalFrequency;
    }
    private void Update()
    {
        // Smoothly slide the frequency up or down 
        lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, targetFrequency, transitionSpeed * Time.deltaTime);
    }

    // Call this to plunge the audio underwater
    public void EnableUnderwaterAudio()
    {
        targetFrequency = underwaterFrequency;
    }

    // Call this when you surface to clear the water from your ears
    public void DisableUnderwaterAudio()
    {
        targetFrequency = normalFrequency;
    }
}
