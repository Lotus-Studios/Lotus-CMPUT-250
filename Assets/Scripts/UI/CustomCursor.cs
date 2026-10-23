using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://medium.com/@arkantose.dc/creating-a-cursor-in-unity-f37ba3088618 - Cursor tutorial
public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorArrow;
    public Texture2D cursorArrow2;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cursor.SetCursor(cursorArrow2, Vector2.zero, CursorMode.ForceSoftware);
            var screenPos = Input.mousePosition;
            screenPos.z = 10;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
        }
    }
}
