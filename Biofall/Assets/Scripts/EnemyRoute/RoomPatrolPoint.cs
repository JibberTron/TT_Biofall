/*
    PURPOSE:
    Represents a room destination that the enemy can choose to patrol toward.

    Each room point can have:
    - A room name for debugging.
    - Route points leading toward the room.
    - Wander points inside the room.
    - Base interest, which affects how likely the room is to be chosen.
    - Activity interest, which increases when noise or player activity happens nearby.

    PSEUDOCODE:
    Enemy chooses this room:
        Follow this room's route points.
        Move to the room point.
        Wander between room wander points.
        Then choose a new room.

    Gizmos:
        Green sphere = room destination.
        Red spheres = route/travel points.
        Cyan cubes = wander/search points.

    NOTES:
    - Base Interest is the room's default importance.
    - Activity Interest is temporary and can be boosted by noise.
    - This script stores data. It does not move the enemy by itself.
*/

using UnityEngine;

public class RoomPatrolPoint : MonoBehaviour
{
    [Header("Room Info")]
    public string roomName = "Room";

    [Header("Route To This Room")]
    public Transform[] routePoints;

    [Header("Room Wander Points")]
    public Transform[] wanderPoints;

    [Header("Room Selection Weight")]
    public float baseInterest = 1f;
    public float activityInterest = 0f;

    [Header("Debug")]
    public bool drawGizmos = true;
    public float gizmoSize = 0.75f;

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);

        if (routePoints != null)
        {
            Gizmos.color = Color.red;

            foreach (Transform point in routePoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, gizmoSize * 0.5f);
                }
            }
        }

        if (wanderPoints != null)
        {
            Gizmos.color = Color.cyan;

            foreach (Transform point in wanderPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireCube(point.position, Vector3.one * gizmoSize * 0.5f);
                }
            }
        }
    }
}