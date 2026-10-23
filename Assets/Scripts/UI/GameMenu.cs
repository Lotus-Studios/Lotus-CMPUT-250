using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameMenu : MonoBehaviour
{
    [SerializeField] private bool isEnabled = false;
    [SerializeField] private bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && isEnabled)
        {
            //playerController.ResetToCurrentCheckpoint();
        }
    }

    void setVisible()
    {
        
    }
}
