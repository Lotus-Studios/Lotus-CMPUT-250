using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndingTriggerZone : MonoBehaviour
{
    bool triggered = false;
    bool finished = false;
    [SerializeField] Animation lotusBloomAnimation;

    public UnityEvent finishedAnimation;
    

    // #TODO: Hide the player sprite
    // #TODO: Turn camera zone into orbit cam


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!triggered){
                lotusBloomAnimation.Play("LotusBloomEnding");
                triggered = true;
            }

        }
    }

    void Update()
    {
        if (triggered && !finished && !lotusBloomAnimation.IsPlaying("LotusBloomEnding"))
        {
            finished = true;
            finishedAnimation?.Invoke();
        }       
    }
}
