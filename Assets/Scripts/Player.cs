

using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float releaseHardness = 1;
    [SerializeField] private float dragHardness = 1;
    [SerializeField] private Transform cameraPlaceHolder;
    private Movements _movements;

    private void Start()
    {
        _movements = GetComponent<Movements>();
        
        //Setting up the camera
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        cameraFollow.target = transform;
        cameraFollow.placeHolder = cameraPlaceHolder;
        cameraFollow.Initialize();

    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            _movements.deviation = Mathf.Clamp(_movements.deviation + Input.GetAxis("Mouse X") * dragHardness, -1f, 1f);
        }
        else
            _movements.deviation = Mathf.Lerp(_movements.deviation, 0, Time.deltaTime * releaseHardness);
    }

}