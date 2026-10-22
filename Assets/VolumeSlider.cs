using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{

    //https://stackoverflow.com/questions/46529147/how-to-set-a-mixers-volume-to-a-sliders-volume-in-unity

    [SerializeField] AudioMixer mixerAsset;
    [SerializeField] float minDecibels = -40;
    float fullDecibels = 0;
    
    [SerializeField] Slider slider;

    [SerializeField] string targetAudioChannel = "Music";

    [SerializeField] GameObject mutedXMark;

    void Awake()
    {
        
        mixerAsset.GetFloat(targetAudioChannel, out float defaultDecibels);
        fullDecibels = defaultDecibels;
    }

    public void updateSlider()
    {
        float sliderRawValue = slider.value;
        ChangeVolume(sliderRawValue);
    }

    // https://discussions.unity.com/t/audio-slider-in-unity-done-right-unity-tutorial/873505

    void ChangeVolume(float sliderRawValue)
    {
        // Debug.Log(Mathf.Lerp(minDecibels, fullDecibels, sliderRawValue));
        // mixerAsset.SetFloat(targetAudioChannel, Mathf.Lerp(minDecibels, fullDecibels, sliderRawValue));
        if (sliderRawValue <= 0.05)
        {
            mixerAsset.SetFloat(targetAudioChannel, -40);
            mutedXMark.SetActive(true);
            return;
        }
        mutedXMark.SetActive(false);
        
        mixerAsset.SetFloat(targetAudioChannel, Mathf.Log10(sliderRawValue)*20);
        
    }

}
