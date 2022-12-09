using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(NavMeshObstacle))]
public class ObstacleAgent : MonoBehaviour
{
    [SerializeField] private float carvingTime = 0.5f;
    [SerializeField] private float carvingMoveTreshold = 0.1f;

    [HideInInspector] public NavMeshAgent agent;
    private NavMeshObstacle obstacle;

    private float lastMoveTime;
    private Vector3 lastPosition;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;

        lastPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(lastPosition, transform.position) > carvingMoveTreshold)
        {
            lastMoveTime = Time.time;
            lastPosition = transform.position;
        }
        if (lastMoveTime + carvingTime < Time.time)
        {
            agent.enabled = false;
            obstacle.enabled = true;
        }
    }

    public void SetDestination(Vector3 position)
    {
        obstacle.enabled = false;

        lastMoveTime = Time.time;
        lastPosition = transform.position;

        StartCoroutine(MoveAgent(position));
    }

    private IEnumerator MoveAgent(Vector3 position)
    {
        yield return null;
        agent.enabled = true;
        agent.SetDestination(position);
    }
}
