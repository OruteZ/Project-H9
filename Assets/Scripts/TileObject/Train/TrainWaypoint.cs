using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWaypoint : MonoBehaviour
{
    public Transform GetWaypoint(int waypointIndex)
    {
        return transform.GetChild(waypointIndex);
    }

    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        int nextWaypointIndex = currentWaypointIndex + 1;
        if (nextWaypointIndex == transform.childCount)
        {
            nextWaypointIndex = -1;
        }

        return nextWaypointIndex;
    }

    private void OnDrawGizmos()
    {
        for (int waypoinIndex = 0; waypoinIndex < transform.childCount; waypoinIndex++)
        {
            // 웨이포인트 지점에 원 그리기
            var waypoint = GetWaypoint(waypoinIndex);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(waypoint.position, 0.1f);

            // 다음 웨이포인트까지 선그리기
            int nextWaypointIndex = GetNextWaypointIndex(waypoinIndex);
            if (nextWaypointIndex == -1) return;
            var nextWaypoint = GetWaypoint(nextWaypointIndex);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(waypoint.position, nextWaypoint.position);
        }
    }
}

