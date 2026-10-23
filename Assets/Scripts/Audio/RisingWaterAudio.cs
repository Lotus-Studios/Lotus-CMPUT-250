using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingWaterAudio : MonoBehaviour
{
    public static RisingWaterAudio Instance { get; private set; }    
    [SerializeField] private Transform topOfHeadTransform;
    [SerializeField] private Transform waterTransform;
    [SerializeField] private AudioSource risingWaterSource;
    [SerializeField] private float maxVolume = 0.2f;
    [SerializeField] private float maxHeight = 20f;
    [SerializeField] private float fadeOutSpeed = 0.5f;
    [SerializeField] public AudioSource waveSource;
    private float heightDifference = 0;
    private float targetVolume = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        heightDifference = topOfHeadTransform.position.y - waterTransform.position.y; 

        targetVolume = (maxHeight - heightDifference) * (maxVolume / maxHeight);
            
        
        if (heightDifference >= 0) {
            risingWaterSource.volume = Mathf.Lerp(risingWaterSource.volume, targetVolume, Time.deltaTime);
        } else {
            risingWaterSource.volume = Mathf.Lerp(risingWaterSource.volume, 0, fadeOutSpeed * Time.deltaTime);
        }
    }

    public void PlayWave()
    {
        waveSource.Play();
    }

}
