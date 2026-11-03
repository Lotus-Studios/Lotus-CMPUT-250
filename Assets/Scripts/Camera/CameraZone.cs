using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Camera References")]
    [SerializeField] private OrbitCamera orbitCamera;
    [SerializeField] private Transform cameraTarget;

    [Header("Speed Settings")]
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake()
    {
        if (orbitCamera == null && Camera.main != null)
        {
            //Finds the main camera in the scene and gets the OrbitCamera component from it
            orbitCamera = Camera.main.GetComponent<OrbitCamera>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return; //If object entring trigger zone is not the player

        Debug.Log("Player entered trigger");

        if (orbitCamera == null)
        {

            return;
        }

        if (cameraTarget == null)
        {

            return;
        }

        Debug.Log("Sending camera zone to OrbitCamera");
        //Start moving the camera towards the right place and rotation
        orbitCamera.SetCameraZone(
            cameraTarget,
            movementSpeed,
            rotationSpeed
        );
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Player exited camera zone");

        //Start transitioning out of the target
        orbitCamera.ClearCameraZone(cameraTarget);
    }
}
