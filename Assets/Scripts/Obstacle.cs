using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

/// <summary>
/// Attach to : Obstacle prefab
/// Destroys random children
/// </summary>
public class Obstacle : MonoBehaviour
{
    [SerializeField] private float obstacleDensity;

    private void Start()
    {
        transform.GetChild(Random.Range(0, transform.childCount)).gameObject.SetActive(false); //At least on of the obstacle must be shut down to let a way trough
        
        foreach (Transform obstacle in transform) //Randomly removes the obstacles
            if(Random.Range(0f,1f) < obstacleDensity)
                obstacle.gameObject.SetActive(false);
    }
}
