using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float moveHardness = 1;
    [SerializeField] private float lookHardness = 1;

    [HideInInspector] public Transform placeHolder;
    [HideInInspector] public Transform target;
    
    void Update()
    {
        if (target)
        {
            transform.position = Vector3.Lerp(transform.position, placeHolder.position, moveHardness * Time.deltaTime);
            
            var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookHardness * Time.deltaTime);
        }
    }
}
