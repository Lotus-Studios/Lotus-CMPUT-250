using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterStartTrigger : MonoBehaviour
{
    [SerializeField] private RisingWater risingWater;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (risingWater != null)
            {
                risingWater.StartMe();
                if (AudioLayering.Instance != null)
                {
                    AudioLayering.Instance.FadeToExciting();
                }
            }
        }
    }
}
