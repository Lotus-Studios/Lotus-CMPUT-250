using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    
[CreateAssetMenu(fileName = "New Sound Effect", menuName = "Sound Effect")]
public class SoundEffect : ScriptableObject
{
    public AudioClip[] audioClips;
    public float volume = 1f;
    public float pitch = 1;
}
