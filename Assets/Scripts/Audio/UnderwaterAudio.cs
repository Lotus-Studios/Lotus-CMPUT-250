using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class UnderwaterAudio : MonoBehaviour
{
    public static UnderwaterAudio _instance;
    public static UnderwaterAudio Instance { get { return _instance; } }
    [SerializeField] private AudioSource underwaterSource;
    [SerializeField] private AudioClip submergeSound;
    [SerializeField] private AudioClip emergeSound;
    private bool isUnderwater = false;


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

    // don't need to apply a low pass filter here anymore since marcus is going to apply it outside as a separate track. 
    public void EnableUnderwaterAudio()
    {
        if (AudioLayering.Instance != null)
        {
            AudioLayering.Instance.SetUnderwater(true);
        }

        if (AudioController.Instance != null)
        {
            AudioController.Instance.SetUnderwater(true);
        }
        PlaySubmerge();
    }

    public void DisableUnderwaterAudio()
    {
        if (AudioLayering.Instance != null)
        {
            AudioLayering.Instance.SetUnderwater(false);
        }

        if (AudioController.Instance != null)
        {
            AudioController.Instance.SetUnderwater(false);
        }
        PlayEmerge();
    }

    public void PlaySubmerge()
    {
        if (!isUnderwater) {    // don't play if already underwater
            underwaterSource.PlayOneShot(submergeSound, 0.3f);
            isUnderwater = true;
            //underwaterSource.clip = submergeSound;
            //underwaterSource.Play();
        }
    }

    public void PlayEmerge()
    {
        if (isUnderwater) {
            underwaterSource.PlayOneShot(emergeSound, 0.3f);
            isUnderwater = false;
        }
    }
}
