using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Movements : MonoBehaviour
{
    //Path follow vars
    List<Vector3> totalPath;
    private int actualWaypoint = 0;
    private int nextWaypoint => actualWaypoint + 1;
    [HideInInspector] public Vector3 posOnPath;
    public static bool Moving; //is Static because all characters starts the competition at the same time

    //Movements vars
    [NonSerialized] public float deviation; //Clamped value between -1 and 1, 0 is the center of the toboggan
    [NonSerialized] public bool deviationModifAuthorization = true;
    [NonSerialized] public float absDeviationAcceleration;
    
    [SerializeField] private float speed = 1;
    [SerializeField] private float moveHardness = 1;
    [SerializeField] private float rotationHardness = 1;
    [SerializeField] private float tobogganWidth = 2;
    [SerializeField] private float offsetFromGround;
    
    public float speedMultiplicator = 1;
    private float _lastDeviation;
    
    //Flying parameters
    [SerializeField] private float flyingSpeed = 1;
    [SerializeField] private float fallSpeed = 1;
    
    //Obstacles parameters
    [SerializeField] private float obstacleSpeedMultiplicator = 0.5f;
    [SerializeField] private float obstacleReductionTime = 1;
    

    [NonSerialized] public bool onPath = true; //Says is the player is following the path or flying
    public float ejectionThresold;
    [SerializeField] private float ejectionForce;

    // Start is called before the first frame update
    void Start()
    {
        totalPath = TobogganGenerator.TotalPath;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(posOnPath, 0.5f);
    }

    void Update()
    {
        RaycastHit hit;
        if (onPath && nextWaypoint < totalPath.Count)
        {
            Vector3 segment = totalPath[nextWaypoint] - totalPath[actualWaypoint];
            //Moving reference position on path
            if (Moving)
            {
                if (Vector3.Project(posOnPath - totalPath[actualWaypoint], segment).magnitude < segment.magnitude)
                {
                    float deviationMultiplicator = (1f - Math.Abs(deviation) / 4f); //The character is faster when he is near the center of the toboggan
                    posOnPath += speed * speedMultiplicator * deviationMultiplicator * Time.deltaTime * segment.normalized;
                }
                else
                {
                    actualWaypoint++;

                    if (actualWaypoint == totalPath.Count - 1)//End
                    {
                        PlayEnd();
                    }
                }
            }


            //ORIENTATING CHARACTER
            transform.forward = Vector3.Lerp(transform.forward, segment, Time.deltaTime * rotationHardness);

            //PLACING CHARACTER
            //The character is now placed on the toboggan with a raycast, and the deviation from the initial path is added here
            //Layer mask 9 is for "Toboggan"

            if (Physics.Raycast(posOnPath + Vector3.up + transform.right * deviation * tobogganWidth /2f, Vector3.down, out hit, 10,
                1 << 9))
            {
                transform.position = Vector3.Slerp(transform.position, hit.point + hit.normal * offsetFromGround, Time.deltaTime * moveHardness);
            }

            if (Mathf.Abs(deviation) > ejectionThresold)
            {
                StartFly();
            }
        }
        else if(!onPath) //The character is flying
        {
            transform.position += Time.deltaTime * flyingSpeed * transform.forward - transform.up * fallSpeed ;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, 1<< 9))
            {
                //Find approximate position on path
                List<Vector3> nearests = totalPath.OrderBy(p => Vector3.Distance(transform.position, p)).ToList();
                if (nearests.Count > 0)
                {
                    int nearestId = totalPath.IndexOf(nearests[0]);
                    actualWaypoint = nearestId;
                    posOnPath = totalPath[actualWaypoint];
                    onPath = true;
                }
            }
        }
        
        absDeviationAcceleration = Mathf.Abs(_lastDeviation - deviation);
        _lastDeviation = deviation;
    }

    void PlayEnd()
    {
        Transform characterHolder = new GameObject().transform;
        characterHolder.position = transform.position;
        Vector3 scale = characterHolder.localScale;
        scale.z *= Random.Range(1f, 1.2f);
        characterHolder.localScale = scale;
        characterHolder.localEulerAngles = Vector3.up * (transform.eulerAngles.y + Random.Range(-5f,5f));
            
        transform.parent = characterHolder.transform;
        GetComponent<Movements>().enabled = false;
        GetComponent<Animator>().enabled = true;   
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            other.gameObject.SetActive(false);
            StartCoroutine(TempMultiplicator(obstacleSpeedMultiplicator, obstacleReductionTime));
        }
    }

    private void StartFly()
    {
        onPath = false;
        //deviation = 0; //Reset deviation to avoid reejection when touching a toboggan again
        //X rotation axis reset
        Vector3 newRot = transform.eulerAngles;
        newRot.x = 0;
        transform.eulerAngles = newRot;
        
        //Bumping out
        transform.position += ((Mathf.Sign(deviation) * transform.right + Vector3.up) * ejectionForce);
    }

    public void ApplyTempDeviation(float modification, float duration)
    {
        StartCoroutine(TempDeviation(modification, duration));
    }

    private IEnumerator TempDeviation(float modification, float duration)
    {
        deviationModifAuthorization = false;

        float timer = 0;

        //Using a while is usually not safe, but the lines here are full safe
        while (timer < duration)
        {
            float delta = Time.fixedDeltaTime * (modification / duration);
            deviation += delta;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        if(GetComponent<Player>())
            Debug.Log(deviation);
        deviationModifAuthorization = true;
    }

    public void ApplyTempMultiplicator(float multiplicator, float duration)
    {
        StartCoroutine(TempMultiplicator(multiplicator, duration));
    }
    
    private IEnumerator TempMultiplicator(float multiplicator, float duration)
    {
        speedMultiplicator += multiplicator;
        
        yield return new WaitForSeconds(duration);

        speedMultiplicator -= multiplicator;
    }
}