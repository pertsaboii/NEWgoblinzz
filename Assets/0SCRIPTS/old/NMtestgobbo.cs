using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObstacleAgent))]
public class NMtestgobbo : MonoBehaviour
{
    private enum State
    {
        Roaming, ChaseTarget, Attack
    }

    private ObstacleAgent agent;
    private NavMeshAgent navMeshAgent;

    private State state;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float targetScanningRange;
    //private float originalTargetScanningRange;
    //private float timeWithOutTarget;
    //[SerializeField] private float timeBeforeScanningRadiusIncreases;
    public GameObject target = null;

    private float timeBtwWalks;
    [SerializeField] private float walkCycleTime;
    [SerializeField] private float idleTime;
    private Vector3 originalPos;
    [SerializeField] private float wanderingRange;
    private Vector3 randomPos;

    [SerializeField] private float attackRange;

    private Animator anim;

    [SerializeField] private LayerMask layerMask;

    private NEWmeleedmg attackScript;
    private float attackDistance;

    [SerializeField] private string currentState;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        anim.SetFloat("WalkSpeed", moveSpeed / 2);        //--- jos run anim on liian hidas/nopea niin t‰t‰ voi s‰‰t‰‰
        timeBtwWalks = 0;
        state = State.Roaming;
        anim.SetInteger("State", 0);
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        agent = GetComponent<ObstacleAgent>();
        attackScript = GetComponent<NEWmeleedmg>();
        //originalTargetScanningRange = targetScanningRange;
        originalPos = transform.position;
    }

    void Update()
    {
        switch (state)
        {
            default:
            case State.Roaming:
                if (Vector3.Distance(randomPos, transform.position) < 0.1f && navMeshAgent.enabled == true) navMeshAgent.ResetPath();
                if (Vector3.Distance(transform.position, originalPos) >= wanderingRange)
                {
                    anim.SetInteger("State", 1);
                    agent.SetDestination(originalPos);
                }
                else if (timeBtwWalks <= 0)
                {
                    StartCoroutine("RandomMovement");
                    timeBtwWalks = walkCycleTime + idleTime;
                }
                else timeBtwWalks -= Time.deltaTime;  
                ScanArea();
                /*timeWithOutTarget += Time.deltaTime;
                if (timeWithOutTarget >= timeBeforeScanningRadiusIncreases) targetScanningRange += Time.deltaTime;*/
                break;
            case State.ChaseTarget:
                anim.SetInteger("State", 1);
                if (target != null) agent.SetDestination(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                ApproachTarget();
                if (target == null) ReturnToRoam();
                break;
            case State.Attack:
                if (target != null && Vector3.Distance(target.transform.position, transform.position) > attackDistance + 0.2f) StartChaseState();
                if (navMeshAgent.enabled == true) navMeshAgent.ResetPath();
                anim.SetInteger("State", 2);
                if (target == null && attackScript.currentTargetDead == true)
                {
                    Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, layerMask);
                    if (colliders != null)
                    {
                        foreach (Collider col in colliders)
                        {
                            if (target == null)
                            {
                                target = col.gameObject;
                                attackScript.target = target;
                                attackScript.currentTargetDead = false;
                                attackScript.targetInRange = true;
                            }
                        }
                    }
                    if (target == null)
                    {
                        attackScript.currentTargetDead = false;
                        attackScript.targetInRange = false;
                        ReturnToRoam();
                    }
                }
                if (target == null)
                {
                    attackScript.currentTargetDead = false;
                    attackScript.targetInRange = false;
                    ReturnToRoam();
                }
                if (target != null)
                {
                    attackScript.currentTargetDead = false;
                    transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                }
                if (attackScript.attackStateInitiated == false && attackScript.currentTargetDead == false) StartCoroutine(attackScript.Attack());
                break;
        }
        /*if (target != null)
        {
            timeWithOutTarget = 0;
            targetScanningRange = originalTargetScanningRange;
        }*/
        currentState = state.ToString();

        if (rb.velocity != Vector3.zero) rb.velocity = Vector3.zero;
    }
    void ReturnToRoam()
    {
        timeBtwWalks = 0;
        attackScript.currentTargetDead = false;
        target = null;
        state = State.Roaming;
    }
    void ScanArea()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, targetScanningRange, layerMask);
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                if (target == null)
                {
                    target = col.gameObject;
                    attackScript.target = target;
                    state = State.ChaseTarget;
                }
            }
        }
    }
    void ApproachTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, layerMask);
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                if (col.gameObject == target) StartAttackState();
                else
                {
                    target = col.gameObject;
                    attackScript.target = target;
                    StartAttackState();
                }
            }
        }
    }
    void StartAttackState()
    {
        attackDistance = Vector3.Distance(target.transform.position, transform.position);
        state = State.Attack;
        attackScript.targetInRange = true;
        attackScript.attackStateInitiated = false;
    }
    void StartChaseState()
    {
        attackScript.attackStateInitiated = false;
        attackScript.targetInRange = false;
        state = State.ChaseTarget;
    }
    IEnumerator RandomMovement()
    {
        randomPos = transform.position;
        if (navMeshAgent.enabled == true) navMeshAgent.ResetPath();
        anim.SetInteger("State", 0);

        yield return new WaitForSeconds(idleTime);

        anim.SetInteger("State", 1);

        while (Vector3.Distance(randomPos, transform.position) < 4) CalculateRandomNavMeshPoint();
        agent.SetDestination(randomPos);
    }
    private void CalculateRandomNavMeshPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderingRange;
        randomDirection += originalPos;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderingRange, 1))
        {
            finalPosition = hit.position;
        }
        randomPos = finalPosition;
    }
}
