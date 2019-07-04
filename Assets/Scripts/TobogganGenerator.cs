using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TobogganGenerator : MonoBehaviour
{
    [Header("Toboggan parameters")]
    
    [SerializeField] private Transform endPoint;
    [SerializeField] private List<Transform> types = new List<Transform>();
    [SerializeField] private Transform startModule;
    [SerializeField] private int length = 10;

    [Header("Character spawner parameters")] 
    
    [SerializeField] private Transform playerPrefab;
    
    private int _iterations = 0;
    public static List<Vector3> TotalPath = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        TotalPath.Clear();
        GenerateParentModule(endPoint);
    }

    void GenerateParentModule(Transform module)
    {
        _iterations++;
        if (_iterations < length)
        {
            Transform type = types[Random.Range(0, types.Count)];
            GenerateParentModule(InstantiateModule(module, type));
        }
        else
        {
            InstantiateModule(module, startModule);
            //Path is cleaned from duplicates
            TotalPath = TotalPath.Distinct().ToList();
            SpawnCharacters(module.GetComponent<TobogganModule>());
        }
    }

    Transform InstantiateModule(Transform actual, Transform type)
    {
        TobogganModule infos = actual.GetComponent<TobogganModule>();
        Transform parent = Instantiate(type, infos.endLink.position, Quaternion.identity);

        if (parent.GetComponent<TobogganModule>())
        {
            Vector3 rotationAxis = Vector3.Cross(parent.forward, parent.up);
            parent.RotateAround(parent.position, rotationAxis, parent.GetComponent<TobogganModule>().angle);
            
            //Add module's path to total path
            TotalPath.InsertRange(0, infos.Path);
        }

        if (infos.endLink)
        {
            float yRotation = infos.endLink.eulerAngles.y;
            parent.localEulerAngles += yRotation * Vector3.up;
        }
        
        return parent;
    }

    void SpawnCharacters(TobogganModule module)
    {
        SpawnPlayer(module);
    }

    void SpawnPlayer(TobogganModule module)
    {
        Instantiate(playerPrefab,
            module.Path[0] + (module.Path[1] - module.Path[0]) * Random.Range(0f, 1f) + Vector3.up,
            Quaternion.identity);
    }
}