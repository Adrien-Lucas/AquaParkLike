using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TobogganGenerator : MonoBehaviour
{
    [SerializeField] private Transform endPoint;

    [SerializeField] private List<Transform> types = new List<Transform>();

    [SerializeField] private Transform startModule;

    [SerializeField] private int length = 10;

    private int _iterations = 0;

    // Start is called before the first frame update
    void Start()
    {
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
        }

        if (infos.endLink)
        {
            float yRotation = infos.endLink.eulerAngles.y;
            parent.localEulerAngles += yRotation * Vector3.up;
        }
        
        return parent;
    }
}