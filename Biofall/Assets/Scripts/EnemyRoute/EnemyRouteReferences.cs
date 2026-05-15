/*
    PURPOSE:
    Stores shared enemy references in one place.

    This prevents every script from needing its own serialized target,
    room list, or NavMeshAgent reference.

    PSEUDOCODE:
    On Awake:
        Get the NavMeshAgent attached to this enemy.

    Other scripts can ask for:
        Target
        RoomPoints
        Agent

    NOTES:
    - The player target should be assigned in the Inspector.
    - Room points should include all possible room destinations.
    - This script is mainly a convenience/reference holder.
*/

using UnityEngine;
using UnityEngine.AI;

public class EnemyRouteReferences : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private RoomPatrolPoint[] roomPoints;

    private NavMeshAgent agent;

    public Transform Target => target;
    public RoomPatrolPoint[] RoomPoints => roomPoints;
    public NavMeshAgent Agent => agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
}