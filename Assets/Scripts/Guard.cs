using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public Transform pathHolder;

    Vector3[] waypoints;
    public float speed = 3;
    public float turnSpeed = 80;
    public float pauseDuration = 1;

    bool gameOver;
    private void Start()
    {
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


