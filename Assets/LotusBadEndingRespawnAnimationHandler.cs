using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LotusBadEndingRespawnAnimationHandler : MonoBehaviour
{
    bool finished = false;
    bool triggered = false;
    [SerializeField] Animation seedlinDrownAnimation;
    [SerializeField] Camera cam;

    public UnityEvent finishedAnimation;


    void Update()
    {
        if (triggered && !finished && !seedlinDrownAnimation.IsPlaying("BadEndingRespawn"))
        {
            finished = true;
            finishedAnimation?.Invoke();
            Debug.Log("Finished getting thrown.");
        }       
    }

    public void activate()
    {
        seedlinDrownAnimation.Play("BadEndingRespawn");
        cam.enabled = true;
        triggered = true;
    }

    public void deactivate()
    {
        cam.enabled = false;
    }
}
