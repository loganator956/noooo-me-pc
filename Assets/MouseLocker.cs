using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLocker : MonoBehaviour
{
    private CharacterRotateyFP _rotateyGuy;

    private void Awake()
    {
        _rotateyGuy = FindAnyObjectByType<CharacterRotateyFP>();
    }
    // Update is called once per frame
    void Update()
    {
        if (_rotateyGuy.enabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
