using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://medium.com/@arkantose.dc/creating-a-cursor-in-unity-f37ba3088618 - Cursor tutorial
public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorArrow;
    public Texture2D cursorArrow2;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;
    private bool wasLocked = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorArrow, hotSpot, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        bool isUnlocked = Cursor.lockState == CursorLockMode.None;

        // Whenever the game transitions from locked to unlocked it replaces the os cursor with the custom one. 
        if (isUnlocked && wasLocked)
        {
            ApplyCursor(cursorArrow);
        }
        wasLocked = !isUnlocked;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cursor.SetCursor(cursorArrow2, hotSpot, CursorMode.Auto);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Cursor.SetCursor(cursorArrow, hotSpot, CursorMode.Auto);
        }
    }

    private void ApplyCursor(Texture2D tex)
    {
        if (tex != null)
        {
            Cursor.SetCursor(tex, hotSpot, CursorMode.Auto);
        }
    }
}
