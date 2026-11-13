using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationParticleTriggerHelper : MonoBehaviour
{
    [SerializeField]ParticleSystem particles;

    public void Trigger()
    {
        particles.Play();
    }
}
