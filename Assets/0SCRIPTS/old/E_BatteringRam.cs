using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObstacleAgent))]
public class E_BatteringRam : MonoBehaviour
{
    private enum State
    {
        ApproachTarget, Attack, Roaming
    }

    private State state;
    private GameObject target;

    private ObstacleAgent agent;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackDamage;

    private GameObject spawningEnemy;

    private int currentBuildingAmount;

    private ALL_Health healthScript;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        healthScript = GetComponent<ALL_Health>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        agent = GetComponent<ObstacleAgent>();
        //spawningEnemy = healthScript.unitThatSpawns;
        LockOnTarget();
    }

    void LockOnTarget()
    {
        if (gamemanager.buildings.Count != 0)
        {
            currentBuildingAmount = gamemanager.buildings.Count;

            foreach (GameObject building in gamemanager.buildings)
            {
                if (target == null || Vector3.Distance(building.transform.position, gameObject.transform.position) < Vector3.Distance(target.transform.position, gameObject.transform.position))
                {
                    target = building;
                    state = State.ApproachTarget;
                    agent.SetDestination(target.transform.position);
                }
            }
        }
        else DeployTroop();
    }
    void Update()
    {
        switch (state)
        {
            default:
            case State.ApproachTarget:
                if (currentBuildingAmount != gamemanager.buildings.Count) LockOnTarget();
                break;
        }
        if (rb.velocity != Vector3.zero) rb.velocity = Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (state == State.ApproachTarget && collision.gameObject == target)
        {           
            ImpactToTarget();
        }
    }
    void ImpactToTarget()
    {
        target.GetComponent<ALL_Health>().UpdateHealth(-attackDamage);
        healthScript.UpdateHealth(-healthScript.currentHealth -1);
    }
    void DeployTroop()
    {
        healthScript.UpdateHealth(-healthScript.currentHealth - 1);
    }
}
