using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyRoomFailsafe : MonoBehaviour
{
    [Header("Enemy Detection")]
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Failsafe Settings")]
    [SerializeField] private float maxTimeInRoom = 45f;
    [SerializeField] private Transform teleportPoint;

    private List<EnemyTracker> enemiesInRoom = new List<EnemyTracker>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(enemyTag))
        {
            return;
        }

        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            return;
        }

        enemiesInRoom.Add(new EnemyTracker(other.transform, agent));

        foreach (EnemyTracker enemy in enemiesInRoom)
        {
            if (enemy.enemyTransform == other.transform)
            {
                return;
            }
        }
    }

    private void Update()
    {
        for (int i = enemiesInRoom.Count - 1; i >= 0; i--)
        {
            EnemyTracker enemy = enemiesInRoom[i];

            if (enemy == null || enemy.enemyTransform == null)
            {
                enemiesInRoom.RemoveAt(i);
                continue;
            }

            enemy.CheckForStateChange();

            if (enemy.ShouldSkipTeleport())
            {
                continue;
            }

            enemy.timer += Time.deltaTime;

            if (enemy.timer >= maxTimeInRoom)
            {
                TeleportEnemy(enemy);
                enemiesInRoom.RemoveAt(i);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = enemiesInRoom.Count - 1; i >= 0; i--)
        {
            if (enemiesInRoom[i].enemyTransform == other.transform)
            {
                enemiesInRoom.RemoveAt(i);
                return;
            }
        }
    }

    private void TeleportEnemy(EnemyTracker enemy)
    {
        if (teleportPoint == null)
        {
            Debug.LogWarning($"{gameObject.name} has no teleport point assigned.");
            return;
        }

        enemy.agent.Warp(teleportPoint.position);

        Debug.Log($"{enemy.enemyTransform.name} was moved by room failsafe.");
    }

    private class EnemyTracker
    {
        public Transform enemyTransform;
        public NavMeshAgent agent;
        public float timer;

        private enemyBrain brain;
        private enemyBrain.EnemyState lastState;

        private bool IsSafeTimerState(enemyBrain.EnemyState state)
        {
            return state == enemyBrain.EnemyState.ROAMING
                || state == enemyBrain.EnemyState.INVESTIGATING;
        }

        public EnemyTracker(Transform transform, NavMeshAgent navAgent)
        {
            enemyTransform = transform;
            agent = navAgent;
            timer = 0f;

            brain = transform.GetComponent<enemyBrain>();

            if (brain != null)
            {
                lastState = brain.CurrentState;
            }
        }

        public void CheckForStateChange()
        {
            if (brain == null)
            {
                return;
            }

            bool wasSafeTimerState = IsSafeTimerState(lastState);
            bool isSafeTimerState = IsSafeTimerState(brain.CurrentState);

            // Only reset if the enemy moves between safe timer states and unsafe states.
            // Do NOT reset when switching between ROAMING and INVESTIGATING.
            if (wasSafeTimerState != isSafeTimerState)
            {
                timer = 0f;
            }

            lastState = brain.CurrentState;
        }

        public bool ShouldSkipTeleport()
        {
            if (brain == null)
            {
                return false;
            }

            return !IsSafeTimerState(brain.CurrentState);
        }
    }
}