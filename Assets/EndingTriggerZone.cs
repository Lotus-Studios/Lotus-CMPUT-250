using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTriggerZone : MonoBehaviour
{
    bool triggered = false;
    [SerializeField] Animation lotusBloomAnimation;
    

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
}
