using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputManager : MonoBehaviour
{
    public static float MouseX;

    private float _lastTouchX;

    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        if (Input.touchCount == 0) return;

        MouseX = Input.GetTouch(0).deltaPosition.x;
#else
        MouseX = Input.GetAxis("Mouse X");
#endif
    }
}