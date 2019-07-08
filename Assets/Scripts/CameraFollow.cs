using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to : Main camera
/// This component manages the camera that follows the player
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float moveHardness = 1;
    [SerializeField] private float lookHardness = 1;

    [HideInInspector] public Transform placeHolder;
    [HideInInspector] public Transform target;
    
    void Update()
    {
        if (target) //Before having a target, the camera is in "lobby mode", so it should not move
        {
            // Smoothly moving at the camera holder position
            transform.position = Vector3.Lerp(transform.position, placeHolder.position, moveHardness * Time.deltaTime);
            
            var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookHardness * Time.deltaTime);
        }
    }
}
