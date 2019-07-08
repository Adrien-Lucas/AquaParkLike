using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

/// <summary>
/// Attach to : Character prefab
/// This component manages all the movements of the characters
/// </summary>
public class Movements : MonoBehaviour
{
    //Path follow vars
    private List<Vector3> totalPath;
    [NonSerialized] public int actualWaypoint = 0;
    private int nextWaypoint => actualWaypoint + 1;
    [HideInInspector] public Vector3 posOnPath;
    public static bool Moving; //is Static because all characters starts the competition at the same time

    [Header("Path follow parameters")]
    [NonSerialized] public float deviation; //Clamped value between -1 and 1, 0 is the center of the toboggan
    [NonSerialized] public bool deviationModifAuthorization = true;
    [NonSerialized] public float absDeviationAcceleration;
    [NonSerialized] public float speedMultiplicator = 1;
    [NonSerialized] public bool onPath = true; //Says is the player is following the path or flying

    [SerializeField] private float speed = 1;
    [SerializeField] private float moveHardness = 1;
    [SerializeField] private float rotationHardness = 1;
    [SerializeField] private float tobogganWidth = 2;
    [SerializeField] private float offsetFromGround;
    
    private float _lastDeviation;
    
    [Header("Flying parameters")]

    public float ejectionThresold;

    [SerializeField] private float ejectionForce;
    
    [Header("Flying parameters")]
    
    [SerializeField] private float flyingSpeed = 1;
    [SerializeField] private float fallSpeed = 1;
    
    [Header("Obstacles parameters")]
    
    [SerializeField] private float obstacleSpeedMultiplicator = 0.5f;
    [SerializeField] private float obstacleReductionTime = 1;
    
    //Privates
    private Animator _animator;
    private Player _player;
    private InGameUI _inGameUi;

    // Start is called before the first frame update
    void Start()
    {
        _inGameUi = FindObjectOfType<InGameUI>();
        _player = GetComponent<Player>();
        _animator = GetComponent<Animator>();
        totalPath = TobogganGenerator.TotalPath;
    }

    // Draws a red sphere on the path according to the character's position on path
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
            Vector3 segment = totalPath[nextWaypoint] - totalPath[actualWaypoint]; //The segment between actual point and next point
            
            //Moving reference position on path
            if (Moving)
            {
                //The character's position is projected on segment, if this vector is longer than segment, it means that next point has been passed
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

            //EJECTION DETECTION
            if (Mathf.Abs(deviation) > ejectionThresold)
            {
                StartFly();
            }
        }
        else if(!onPath) //The character is flying
        {
            transform.position += Time.deltaTime * flyingSpeed * transform.forward - transform.up * fallSpeed;

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
        
        //The deviation acceleration is computed here, its used for collisions between characters
        absDeviationAcceleration = Mathf.Abs(_lastDeviation - deviation);
        _lastDeviation = deviation;
    }

    /// <summary>
    /// The function is called when the character reaches the end of the circuit
    /// </summary>
    void PlayEnd()
    {
        //Final jump animation setup
        Transform characterHolder = new GameObject().transform;
        characterHolder.parent = transform.parent;
        characterHolder.position = transform.position;
        Vector3 scale = characterHolder.localScale;
        scale.z *= Random.Range(1f, 1.2f);
        characterHolder.localScale = scale;
        characterHolder.localEulerAngles = Vector3.up * (transform.eulerAngles.y + Random.Range(-5f,5f));
            
        transform.parent = characterHolder.transform;
        enabled = false;
        _animator.enabled = true;   
        
        if(_player)
            _inGameUi.End();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle")) //The character is slowed down when colliding with an obstacle
        {
            other.gameObject.SetActive(false);
            StartCoroutine(TempMultiplicator(obstacleSpeedMultiplicator, obstacleReductionTime));
        }
    }
    
    /// <summary>
    /// This function makes the character flying
    /// </summary>
    private void StartFly()
    {
        onPath = false;
        //X rotation axis reset
        Vector3 newRot = transform.eulerAngles;
        newRot.x = 0;
        transform.eulerAngles = newRot;
        
        //Bumping out
        transform.position += ((Mathf.Sign(deviation) * transform.right + Vector3.up) * ejectionForce);
    }

    //TEMPORARY BEHAVIOUR FUNCTIONS
    
    public void ApplyTempDeviation(float modification, float duration) { StartCoroutine(TempDeviation(modification, duration)); }
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
        
        deviationModifAuthorization = true;
    }

    public void ApplyTempMultiplicator(float multiplicator, float duration) { StartCoroutine(TempMultiplicator(multiplicator, duration)); }
    private IEnumerator TempMultiplicator(float multiplicator, float duration)
    {
        speedMultiplicator += (multiplicator - 1f);
        
        yield return new WaitForSeconds(duration);

        speedMultiplicator -= (multiplicator -1f);
    }
}