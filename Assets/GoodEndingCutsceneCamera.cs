using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodEndingCutsceneCamera : MonoBehaviour
{
    private bool isRotating = false;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] public Camera cam;
    [SerializeField] private Transform target;
    

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
            cam.gameObject.transform.LookAt(target);
        }
    }

    public void activate()
    {
        cam.enabled = true;
        isRotating = true;
    }

    public void deactivate()
    {
        cam.enabled = false;
        isRotating = false;
    }
}
