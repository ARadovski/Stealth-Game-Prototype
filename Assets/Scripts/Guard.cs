using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public Transform pathHolder;

    Vector3[] waypoints;
    public float speed = 3;
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
                while (transform.position != waypoint)
                {
                    transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);
                    yield return null;
                }

                yield return new WaitForSeconds(pauseDuration);
            }    
        }
           
    }
}
