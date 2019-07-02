using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to : Toboggan prefab
/// This component stores all key objects of the toboggan prefab
/// </summary>
public class TobogganModule : MonoBehaviour
{
    [SerializeField] private Transform path; //The gameObject storing all the path points
    public Transform endLink; //The point at the end of the module
    public float angle;
}