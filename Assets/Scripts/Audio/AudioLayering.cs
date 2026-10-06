using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLayering : MonoBehaviour
{
    public static AudioLayering _instance;
    public static AudioLayering Instance { get { return _instance; } }

    public AudioSource chillSource;
    public AudioSource excitingSource;

    public float fadeSpeed = 0.5f;
    private float targetChill = 0.8f;
    private float targetExciting = 0f;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }
    void Start()
    {
        chillSource.volume = 0.8f;
        excitingSource.volume = 0f;

        chillSource.Play();
        excitingSource.Play();
    }

    void Update()
    {
        // basically the lerp which will never fully reach 1 or 0 will be intercepted by the Movetowards functions so it can smoothly fade out or in.
        // I tried just setting it to 0f and 1f instantly but it was too abrupt for my taste 
        if (targetChill == 0f && chillSource.volume <= 0.05f)
        {
            chillSource.volume = Mathf.MoveTowards(chillSource.volume, 0f, fadeSpeed * Time.deltaTime);
        }
        else if (targetChill == 1f && chillSource.volume >= 0.95f)
        {
            chillSource.volume = Mathf.MoveTowards(chillSource.volume, 0.8f, fadeSpeed * Time.deltaTime);
        }
        else
        {
            chillSource.volume = Mathf.Lerp(chillSource.volume, targetChill, fadeSpeed * Time.deltaTime);
        }

        if (targetExciting == 0f && excitingSource.volume <= 0.05f)
        {
            excitingSource.volume = Mathf.MoveTowards(excitingSource.volume, 0f, fadeSpeed * Time.deltaTime);
        }
        else if (targetExciting == 1f && excitingSource.volume >= 0.95f)
        {
            excitingSource.volume = Mathf.MoveTowards(excitingSource.volume, 0.8f, fadeSpeed * Time.deltaTime);
        }
        else
        {
            excitingSource.volume = Mathf.Lerp(excitingSource.volume, targetExciting, fadeSpeed * Time.deltaTime);
        }
    }

    public void FadeToExciting()
    {
        targetChill = 0f;
        targetExciting = 0.8f;
    }

    public void FadeToChill()
    {
        targetChill = 0.8f;
        targetExciting = 0f;
    }
}
