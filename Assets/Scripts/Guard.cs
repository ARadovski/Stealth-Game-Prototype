using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    Transform pathHolder;
    Transform player;

    public LayerMask obstacleMask;
    public Light spotlight;
    Color spotlightOrigColor;
    public float visionRange;
    float visionAngle;
    public float timeToAlarm = 2;
    float timeInSight;

    Vector3[] waypoints = new Vector3[1];
    Vector3 vectorToPlayer;
    public float speed = 3;
    public float turnSpeed = 80;
    public float pauseDuration = 1;
    RaycastHit hit;
    GameStates gameStates;
    private void Start()
    {
        gameStates = GameObject.FindObjectOfType<GameStates>();
        gameStates.OnGameOver += StopAllCoroutines;

        visionAngle = spotlight.spotAngle;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        spotlightOrigColor = spotlight.color;

        PopulatePath();

        transform.position = waypoints[0];
        transform.LookAt(waypoints[1]);
        StartCoroutine(FollowPath());
    }
    private void OnDrawGizmos()
    {
        PopulatePath();

        Vector3 startPoint = waypoints[0];
        Vector3 previousPoint = startPoint;
        foreach (Vector3 waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint,.3f);
            Gizmos.DrawLine(previousPoint, waypoint);
            previousPoint = waypoint;
        }    
        Gizmos.DrawLine(previousPoint, startPoint);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * visionRange);
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            spotlight.color = Color.Lerp(spotlight.color, Color.red, Time.deltaTime * timeToAlarm);
            timeInSight += Time.deltaTime;
            if (timeInSight >= timeToAlarm && !gameStates.winner)
            {
                gameStates.gameOver = true;
            }
        }
        else 
        {
            timeInSight = 0;
            spotlight.color = spotlightOrigColor;
        }
    }

    void PopulatePath()
    {
        pathHolder = transform.Find("Path");
        waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < pathHolder.childCount; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
    }

    // DEMO SOLUTION
    bool CanSeePlayer()
    {
        if (Vector3.Distance(player.position, transform.position) < visionRange)
        {
            Vector3 dirToPlayer = player.position - transform.position;
            if (Vector3.Angle(dirToPlayer, transform.forward) < visionAngle/2)
            {
                if (!Physics.Linecast(transform.position, player.position, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath()
    {
        while (true)
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
    }

    // IEnumerator AlarmTimer()
    // {
    //     spotlight.color = Color.Lerp(spotlight.color, Color.red, Time.deltaTime * timeToAlarm);

    //     yield return null;
    // }

    // MY SOLUTION TO PLAYER DETECTION
    // private void FixedUpdate()
    // {
    //     vectorToPlayer = player.transform.position - transform.position;
    //     if(Physics.Raycast(transform.position, vectorToPlayer, out hit, visionRange))
    //     {
    //         Debug.Log("Player within range");
    //         spotlight.color = Color.green;
                       
    //         float angleToHit = Mathf.Atan2(vectorToPlayer.x, vectorToPlayer.z) * Mathf.Rad2Deg;
    //         Debug.Log("Angle: " + angleToHit);
    //         Vector3 vectorToHit = hit.point - transform.position;
    //         Debug.DrawRay(transform.position, vectorToHit, Color.yellow);

    //         if (Mathf.Abs(Mathf.DeltaAngle(angleToHit, transform.eulerAngles.y)) < visionAngle/2)
    //         {
    //             Debug.Log("Delta: " + Mathf.Abs(Mathf.DeltaAngle(angleToHit, transform.eulerAngles.y)));
    //             Debug.Log ("Player within field of view");
    //             spotlight.color = Color.yellow;
    //             if (hit.collider.gameObject.CompareTag("Player"))
    //             {
    //                 Debug.Log("PLAYER CAUGHT!");
    //                 //gameOver = true;                   
    //                 spotlight.color = Color.red;
    //             }
    //         }
            
    //     } 
    //     else {
    //         Debug.Log("Player out of range");
    //         spotlight.color = Color.white;
    //     }
    // }
}


