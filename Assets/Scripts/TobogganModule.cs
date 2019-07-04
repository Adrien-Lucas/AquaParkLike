using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Attach to : Toboggan prefab
/// This component stores all key objects of the toboggan prefab
/// </summary>
public class TobogganModule : MonoBehaviour
{
    [SerializeField] private Transform pathHolder;

    public List<Vector3> Path
    {
        get
        {
            if (pathHolder)
            {
                List<Vector3> path = pathHolder.GetComponentsInChildren<Transform>().Select(t => t.position).ToList();
                path.Remove(pathHolder.position);
                return path;
            }
                
            return new List<Vector3>();
        }
    }

    //The gameObject storing all the path points
    public Transform endLink; //The point at the end of the module
    public float angle;

    [Header("Gizmos parameters")] [SerializeField]
    private Color rayColor = Color.white;

    void OnDrawGizmos()
    {
        Gizmos.color = rayColor;
        for (int i = 0; i < Path.Count; i++)
        {
            if(i > 0) Gizmos.DrawLine(Path[i - 1], Path[i]);
            Gizmos.DrawWireSphere(Path[i], 0.5f);
        }
    }
}