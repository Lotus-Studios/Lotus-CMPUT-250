using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotusBloomAnimationTrigger : MonoBehaviour
{
    [SerializeField] Animation anim;

    public void PlayMe()
    {
        anim.Play("LotusBloomEnding");
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            PlayMe();
        }
    }
}
