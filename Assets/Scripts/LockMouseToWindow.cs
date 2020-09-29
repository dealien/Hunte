using UnityEngine;
using System.Collections;

public class LockMouseToWindow : MonoBehaviour
{
    void Start() { LockCursor(); }


    // Lock mouse cursor to window and hide it
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = true;
    }


    void OnGUI()
    {
        GUILayout.BeginVertical();

        // Release cursor on escape keypress
        if (Input.GetKeyDown(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; }

        // Recapture cursor on mouse click
        if (Input.GetMouseButton(0) && Cursor.lockState != CursorLockMode.Locked) { LockCursor(); }
    }
}