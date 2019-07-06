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
    private int actualWaypoint = 0;
    private int nextWaypoint => actualWaypoint + 1;
    [HideInInspector] public Vector3 posOnPath;
    public static bool Moving; //is Static because all characters starts the competition at the same time

    //Movements vars
    [SerializeField] private float speed = 1;
    private float speedMultiplicator = 1;
    [SerializeField] private float moveHardness = 1;
    [SerializeField] private float rotationHardness = 1;
    [SerializeField] private float tobogganWidth = 2;
    [HideInInspector] public float deviation; //Clamped value between -1 and 1, 0 is the center of the toboggan
    [SerializeField] private float offsetFromGround;
    
    //Flying parameters
    [SerializeField] private float flyingSpeed = 1;
    [SerializeField] private float fallSpeed = 1;
    
    //Obstacles parameters
    [SerializeField] private float speedReductionMultiplicator = 0.5f;
    [SerializeField] private float speedReductionTime = 1;
    

    [HideInInspector] public bool onPath = true; //Says is the player is following the path or flying
    [SerializeField] private float ejectionThresold;
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
                Debug.Log(actualWaypoint + " ; " + nextWaypoint + " ; " + posOnPath + " ; " + Vector3.Project(posOnPath - totalPath[actualWaypoint], segment).magnitude + " <? " + segment.magnitude);
                if (Vector3.Project(posOnPath - totalPath[actualWaypoint], segment).magnitude < segment.magnitude)
                {
                    float deviationMultiplicator = (1f - Math.Abs(deviation) / 4f);
                    posOnPath += speed * speedMultiplicator * deviationMultiplicator * Time.deltaTime * segment.normalized;
                }
                else
                {
                    actualWaypoint++;
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
                BumpCharacterOut();
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
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            other.gameObject.SetActive(false);
            StartCoroutine(KnockedOut());
        }
    }

    private void BumpCharacterOut()
    {
        onPath = false;
        
        //X rotation axis reset
        Vector3 newRot = transform.eulerAngles;
        newRot.x = 0;
        transform.eulerAngles = newRot;
        
        //Bumping out
        transform.position += ((Mathf.Sign(deviation) * transform.right + Vector3.up) * ejectionForce);
    }

    private IEnumerator KnockedOut()
    {
        speedMultiplicator -= speedReductionMultiplicator;
        
        yield return new WaitForSeconds(speedReductionTime);

        speedMultiplicator += speedReductionMultiplicator;
    }
}