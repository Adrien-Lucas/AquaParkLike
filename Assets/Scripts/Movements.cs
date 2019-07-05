using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Movements : MonoBehaviour
{
    //Path follow vars
    List<Vector3> totalPath;
    private int actualWaypoint = 0, nextWaypoint = 1;
    private Vector3 posOnPath;
    
    //Movements vars
    [SerializeField] private float speed = 1;
    [SerializeField] private float moveHardness = 1;
    [SerializeField] private float rotationHardness = 1;
    [HideInInspector] public float deviation; //Clamped value between -1 and 1, 0 is the center of the toboggan


    // Start is called before the first frame update
    void Start()
    {
        totalPath = TobogganGenerator.TotalPath;
        posOnPath = totalPath[0];
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(posOnPath, 0.5f);
    }

    // FixedUpdate is used for physics
    void Update()
    {
        RaycastHit hit;

        Vector3 segment = totalPath[nextWaypoint] - totalPath[actualWaypoint];
        if (Vector3.Project(posOnPath - totalPath[actualWaypoint], segment).magnitude < segment.magnitude)
        {
            posOnPath += speed * Time.deltaTime * segment.normalized;
        }
        else
        {
            actualWaypoint++;
            nextWaypoint++;
        }
        
        //ORIENTATING CHARACTER
        transform.forward = Vector3.Lerp(transform.forward, segment, Time.deltaTime * rotationHardness);
        
        //PLACING CHARACTER
        //The character is now placed on the toboggan with a raycast, and the deviation from the initial path is added here
        //Layer mask 9 is for "Toboggan"
        if (Physics.Raycast(posOnPath + Vector3.up * 3 + transform.right * deviation, Vector3.down, out hit, 10, 1 << 9))
        {
            transform.position = Vector3.Slerp(transform.position,hit.point, Time.deltaTime * moveHardness);
        }
    }
}