using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGroundLayer : MonoBehaviour
{
    public Transform playerTransform;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position, playerTransform.TransformDirection(Vector3.down), out hit, Mathf.Infinity)) 
        {
            if (AudioController.Instance != null) {
                AudioController.Instance.SetFootsteps(hit.transform.gameObject.layer);
            }
        } 
        
    }
}
