using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Attach to : Game manager object
/// This component generates a toboggan and starts the game when the generation is done
/// </summary>
public class TobogganGenerator : MonoBehaviour
{
    [Header("Toboggan parameters")]
    
    [SerializeField] private Transform endPoint;
    [SerializeField] private List<Transform> types = new List<Transform>();
    [SerializeField] private Transform startModule;
    [SerializeField] private int length = 10;

    [Header("Lobby parameters")] 
    
    [SerializeField] private int AINumber;
    [SerializeField] private Transform AIPrefab;
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private string cameraLobbyPlaceHolderGameObjectName = "CameraLobbyPlaceHolder";
    
    private int _iterations = 0;
    private float spawnPosPercent; //percent of the start toboggan
    [NonSerialized] public static List<Vector3> TotalPath = new List<Vector3>();

    // Start is called before the first frame update
    public void Start()
    {
        TotalPath.Clear();
        _iterations = 0;
        Movements.Moving = false;
        FindObjectOfType<CameraFollow>().target = null;
        GenerateParentModule(endPoint);
    }

    /// <summary>
    /// Recursive function that generates a module
    /// </summary>
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
            SpawnCharacters(); //The game is initialized here
        }
    }

    Transform InstantiateModule(Transform actual, Transform type)
    {
        TobogganModule infos = actual.GetComponent<TobogganModule>();
        Transform parent = Instantiate(type, infos.endLink.position, Quaternion.identity, transform);
        
        if (infos.endLink)
        {
            float yRotation = infos.endLink.eulerAngles.y;
            parent.localEulerAngles += yRotation * Vector3.up;
        }
        
        //Add module's path to total path
        TotalPath.InsertRange(0, parent.GetComponent<TobogganModule>().Path);
        
        return parent;
    }


    void SpawnCharacters()
    {
        SpawnOneCharacter(playerPrefab);
        for (int i = 0; i < AINumber; i++)
            SpawnOneCharacter(AIPrefab);

        Transform camHolder = GameObject.Find(cameraLobbyPlaceHolderGameObjectName).transform;
        FindObjectOfType<Camera>().transform.SetPositionAndRotation(camHolder.position, camHolder.rotation);
    }

    void SpawnOneCharacter(Transform prefab)
    {
        Vector3 spawnPos = TotalPath[0] + (TotalPath[1] - TotalPath[0]) * spawnPosPercent;
        Transform character = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
        spawnPosPercent += 1f / (AINumber + 1); //+1 is because of the player
        character.GetComponent<Movements>().posOnPath = spawnPos;
    }
}