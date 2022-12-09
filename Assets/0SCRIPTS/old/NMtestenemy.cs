using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObstacleAgent))]
public class NMtestenemy : MonoBehaviour
{
    private enum State
    {
        Roaming, ChaseTarget, Attack, WalkToMiddle
    }
    private State state;

    private ObstacleAgent agent;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float targetScanningRange;
    private float originalTargetScanningRange;
    private float timeWithOutTarget;
    [SerializeField] private float timeBeforeScanningRadiusIncreases;

    public GameObject target = null;

    private float timeBtwWalks;
    [SerializeField] private float walkCycleTime;
    [SerializeField] private float idleTime;
    [SerializeField] private float wanderingRange;
    private Vector3 controlAreaPos;
    private bool controlAreaFound;
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
        anim.SetFloat("WalkSpeed", moveSpeed / 2);          //--- jos run anim on liian hidas/nopea niin t‰t‰ voi s‰‰t‰‰
        timeBtwWalks = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        agent = GetComponent<ObstacleAgent>();
        attackScript = GetComponent<NEWmeleedmg>();
        originalTargetScanningRange = targetScanningRange;
        //ApproachTarget();                                 //--- v‰lill‰ spawnaava unitti j‰‰ bugaamaan attack stateen jos t‰m‰ on p‰‰ll‰
        if (target == null && Vector3.Distance(new Vector3(0, transform.position.y, 0), transform.position) > 8) StartWalkToMiddle();
        else if (target == null) ReturnToRoam();

    }
    
    void Update()
    {
        switch (state)
        {
            default:
            case State.Roaming:
                if (Vector3.Distance(randomPos, transform.position) < 0.1f && navMeshAgent.enabled == true) navMeshAgent.ResetPath();
                if (controlAreaFound && Vector3.Distance(controlAreaPos, transform.position) > wanderingRange) agent.SetDestination(controlAreaPos);
                else
                {
                    if (timeBtwWalks <= 0)
                    {
                        StartCoroutine("RandomMovement");
                        timeBtwWalks = walkCycleTime + idleTime;
                    }
                    else timeBtwWalks -= Time.deltaTime;
                }
                ScanArea();
                timeWithOutTarget += Time.deltaTime;
                if (timeWithOutTarget >= timeBeforeScanningRadiusIncreases) targetScanningRange += Time.deltaTime * 2;
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
                        if (controlAreaFound == true) ReturnToRoam();
                        else StartWalkToMiddle();
                    }
                }
                if (target != null)
                {
                    attackScript.currentTargetDead = false;
                    transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                }
                if (attackScript.attackStateInitiated == false && attackScript.currentTargetDead == false) StartCoroutine(attackScript.Attack());
                if (target == null)
                {
                    attackScript.currentTargetDead = false;
                    attackScript.targetInRange = false;
                    if (controlAreaFound == true) ReturnToRoam();
                    else StartWalkToMiddle();
                }
                break;
            case State.WalkToMiddle:
                ScanArea();
                if (Vector3.Distance(new Vector3(0, transform.position.y, 0), transform.position) < 8)
                {
                    state = State.Roaming;
                    controlAreaPos = transform.position;
                    controlAreaFound = true;
                }
                break;
        }
        if (target != null)
        {
            timeWithOutTarget = 0;
            targetScanningRange = originalTargetScanningRange;
        }
        currentState = state.ToString();

        if (rb.velocity != Vector3.zero) rb.velocity = Vector3.zero;
    }
    void StartWalkToMiddle()
    {
        state = State.WalkToMiddle;
        anim.SetInteger("State", 1);
        agent.SetDestination(new Vector3(0, transform.position.y, 0));
        attackScript.currentTargetDead = false;
    }
    void ReturnToRoam()
    {
        anim.SetInteger("State", 1);
        timeBtwWalks = 0;
        attackScript.currentTargetDead = false;
        target = null;
        state = State.Roaming;
    }
    void ScanArea()
    {
        target = null;      //

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
        attackScript.targetInRange = true;
        attackScript.attackStateInitiated = false;
        attackScript.currentTargetDead = false;
        state = State.Attack;
    }
    void StartChaseState()
    {
        attackScript.attackStateInitiated = false;
        attackScript.targetInRange = false;
        attackScript.currentTargetDead = false;
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

        if (controlAreaFound == false) randomDirection += transform.position;
        else randomDirection += controlAreaPos;

        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderingRange, 1))
        {
            finalPosition = hit.position;
        }
        randomPos = finalPosition;
    }
}
