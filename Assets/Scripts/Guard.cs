using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public Transform pathHolder;
    public Transform player;
    public Light spotlight;
    public float visionRange;
    float visionAngle;

    Vector3[] waypoints;
    Vector3 vectorToPlayer;
    public float speed = 3;
    public float turnSpeed = 80;
    public float pauseDuration = 1;

    bool gameOver;

    RaycastHit hit;
    private void Start()
    {
        visionAngle = spotlight.spotAngle;
        player = FindObjectOfType<PlayerController>().transform;

        waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < pathHolder.childCount; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        transform.position = waypoints[0];
        transform.LookAt(waypoints[1]);
        StartCoroutine(FollowPath());
    }
    private void OnDrawGizmos()
    {
        Transform startPoint = pathHolder.GetChild(0);
        Transform previousPoint = startPoint;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position,.3f);
            Gizmos.DrawLine(previousPoint.position, waypoint.position);
            previousPoint = waypoint;
        }    
        Gizmos.DrawLine(previousPoint.position, startPoint.position);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * visionRange);
    }

    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        vectorToPlayer = player.transform.position - transform.position;
        if(Physics.Raycast(transform.position, vectorToPlayer, out hit, visionRange))
        {
            Debug.Log("Player within range");
            spotlight.color = Color.green;
            
            
            float angleToHit = Mathf.Atan2(vectorToPlayer.x, vectorToPlayer.z) * Mathf.Rad2Deg;
            Debug.Log("Angle: " + angleToHit);
            Vector3 vectorToHit = hit.point - transform.position;
            Debug.DrawRay(transform.position, vectorToHit, Color.yellow);

            if (Mathf.Abs(Mathf.DeltaAngle(angleToHit, transform.eulerAngles.y)) < visionAngle/2)
            {
                Debug.Log("Delta: " + Mathf.Abs(Mathf.DeltaAngle(angleToHit, transform.eulerAngles.y)));
                Debug.Log ("Player within field of view");
                spotlight.color = Color.yellow;
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    Debug.Log("PLAYER CAUGHT!");
                    //gameOver = true;
                    
                    spotlight.color = Color.red;
                }
            }
            
        } 
        else {
            Debug.Log("Player out of range");
            spotlight.color = Color.white;
        }
    }

    IEnumerator FollowPath()
    {
        while (!gameOver)
        {
            foreach (Vector3 waypoint in waypoints)
            {           
                if (transform.position != waypoint){
                    yield return StartCoroutine(LookAtTarget(waypoint));
                }
                
                while (transform.position != waypoint)
                {
                    transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);
                    yield return null;
                }

                yield return new WaitForSeconds(pauseDuration);               
            }    
        }
           
    }

    IEnumerator LookAtTarget(Vector3 lookTarget)
    {
        Vector3 directionToWaypoint = (lookTarget - transform.position).normalized;
        float angleToWaypoint = Mathf.Atan2(directionToWaypoint.x, directionToWaypoint.z) * Mathf.Rad2Deg;
        //Vector3 targetEulers = new Vector3(transform.eulerAngles.x, angleToWaypoint, transform.eulerAngles.z);
        
        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, angleToWaypoint)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, angleToWaypoint, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }

        
        // Quaternion targetRotation = Quaternion.LookRotation(lookTarget, Vector3.up);
        // while (Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y) > 0.1f)
        // {
        //     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 0.1f * Time.deltaTime);
        //     yield return null;
        // }       
    }
}


