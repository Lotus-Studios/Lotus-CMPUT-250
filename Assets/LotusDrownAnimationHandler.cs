using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LotusDrownAnimationHandler : MonoBehaviour
{
    bool finished = false;
    bool triggered = false;
    [SerializeField] Animation seedlinDrownAnimation;
    [SerializeField] Camera cam;

    public UnityEvent finishedAnimation;


    void Update()
    {
        if (triggered && !finished && !seedlinDrownAnimation.IsPlaying("BadEndingDrowning"))
        {
            finished = true;
            finishedAnimation?.Invoke();
            Debug.Log("Finished droawning.");
        }       
    }

    public void activate()
    {
        seedlinDrownAnimation.Play("BadEndingDrowning");
        cam.enabled = true;
        triggered = true;
    }

    public void deactivate()
    {
        cam.enabled = false;
    }
}