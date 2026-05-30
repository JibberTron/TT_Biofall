using UnityEngine;
using UnityEngine.AI;

public class EnemyRoomFailsafe : MonoBehaviour
{
    [Header("Enemy Detection")]
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Failsafe Settings")]
    [SerializeField] private float maxTimeInRoom = 45f;
    [SerializeField] private Transform teleportPoint;

    private EnemyTracker currentEnemy;

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

        currentEnemy = new EnemyTracker(other.transform, agent);
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentEnemy == null || other.transform != currentEnemy.enemyTransform)
        {
            return;
        }

        currentEnemy.timer += Time.deltaTime;

        if (currentEnemy.timer >= maxTimeInRoom)
        {
            TeleportEnemy(currentEnemy);
            currentEnemy = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentEnemy != null && other.transform == currentEnemy.enemyTransform)
        {
            currentEnemy = null;
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

        public EnemyTracker(Transform transform, NavMeshAgent navAgent)
        {
            enemyTransform = transform;
            agent = navAgent;
            timer = 0f;
        }
    }
}