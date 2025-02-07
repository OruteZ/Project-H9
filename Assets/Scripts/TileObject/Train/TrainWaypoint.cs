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
            // ��������Ʈ ������ �� �׸���
            var waypoint = GetWaypoint(waypoinIndex);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(waypoint.position, 0.1f);

            // ���� ��������Ʈ���� ���׸���
            int nextWaypointIndex = GetNextWaypointIndex(waypoinIndex);
            if (nextWaypointIndex == -1) return;
            var nextWaypoint = GetWaypoint(nextWaypointIndex);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(waypoint.position, nextWaypoint.position);
        }
    }
}

