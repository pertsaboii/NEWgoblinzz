using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObstacleAgent), typeof(ALL_Health), typeof(All_AttackScript))]
public class oldenemyai : MonoBehaviour
{
    private enum State
    {
        Roaming, ChaseTarget, Attack, WalkToMiddle, OutSideOfScreen
    }
    private State state;

    private ObstacleAgent agent;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float targetScanningRange;
    //private float originalTargetScanningRange;
    //private float timeWithOutTarget;
    //[SerializeField] private float timeBeforeScanningRadiusIncreases;

    public GameObject target = null;

    //private float timeBtwWalks;
    /*[SerializeField] private float walkCycleTime;
    [SerializeField] private float idleTime;
    [SerializeField] private float wanderingRange;
    private Vector3 controlAreaPos;
    private bool controlAreaFound;
    private Vector3 randomPos;*/

    [SerializeField] private float attackRange;

    private Animator anim;

    [SerializeField] private LayerMask layerMask;

    private All_AttackScript attackScript;
    private float attackDistance;

    [SerializeField] private string currentState;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        anim.SetFloat("WalkSpeed", moveSpeed / 2);          //--- jos run anim on liian hidas/nopea niin t‰t‰ voi s‰‰t‰‰
        //timeBtwWalks = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        agent = GetComponent<ObstacleAgent>();
        attackScript = GetComponent<All_AttackScript>();
        //originalTargetScanningRange = targetScanningRange;
        //if (gameObject.layer == 0) StartWalkToScreen();
        CheckForEnemies();
        //else if (Vector3.Distance(new Vector3(0, transform.position.y, 0), transform.position) > 8) StartWalkToMiddle();
        //else ReturnToRoam();
    }

    void Update()
    {
        switch (state)
        {
            default:
            case State.OutSideOfScreen:
                break;
            case State.Roaming:
                /*if (Vector3.Distance(randomPos, transform.position) < 0.1f && navMeshAgent.enabled == true) navMeshAgent.ResetPath();
                if (controlAreaFound && Vector3.Distance(controlAreaPos, transform.position) >= wanderingRange)
                {
                    anim.SetInteger("State", 1);
                    agent.SetDestination(controlAreaPos);
                }
                else if (timeBtwWalks <= 0)
                {
                    StartCoroutine("RandomMovement");
                    timeBtwWalks = walkCycleTime + idleTime;
                }
                else timeBtwWalks -= Time.deltaTime;
                if (target != null) StartChaseState();
                ScanArea();
                timeWithOutTarget += Time.deltaTime;
                if (timeWithOutTarget >= timeBeforeScanningRadiusIncreases) targetScanningRange += Time.deltaTime * 2;*/
                break;
            case State.ChaseTarget:
                anim.SetInteger("State", 1); // joskus j‰‰ attack animation p‰‰lle, t‰m‰ est‰‰ sen
                if (target != null) agent.SetDestination(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                ApproachTarget();
                //if (target == null) ReturnToRoam();
                if (target == null) StartWalkToMiddle();
                break;
            case State.Attack:
                if (target != null && Vector3.Distance(target.transform.position, transform.position) > attackDistance + 0.3f) StartChaseState();
                if (navMeshAgent.enabled == true) navMeshAgent.ResetPath();
                if (target == null)
                {
                    foreach (GameObject unitOrBuilding in gamemanager.buildingsAndUnits)
                    {
                        if (Vector3.Distance(unitOrBuilding.transform.position, transform.position) < attackRange && (target == null || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(unitOrBuilding.transform.position, transform.position)))
                        {
                            target = unitOrBuilding;
                            attackDistance = Vector3.Distance(target.transform.position, transform.position);
                            attackScript.target = target;
                            attackScript.targetHealth = target.GetComponent<ALL_Health>();
                            attackScript.targetInRange = true;
                            //attackScript.SwitchToAttackState();
                        }
                    }
                    if (target == null) StartWalkToMiddle();
                    else attackScript.SwitchToAttackState();

                    /*Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, layerMask);
                    if (colliders != null)
                    {
                        foreach (Collider col in colliders)
                        {
                            if (target == null)
                            {
                                target = col.gameObject;
                                attackScript.target = target;
                                attackScript.targetHealth = target.GetComponent<ALL_Health>();
                                attackScript.targetInRange = true;
                                attackScript.SwitchToAttackState();
                            }
                        }
                        if (target == null)
                        {
                            if (controlAreaFound == true) ReturnToRoam();
                            else StartWalkToMiddle();
                        }
                    }*/
                }
                if (target != null) transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                break;
            case State.WalkToMiddle:
                anim.SetInteger("State", 1);    // v‰lill‰ anim attack state j‰‰ p‰‰lle, t‰m‰ est‰‰ sen
                ScanArea();
                /*if (Vector3.Distance(new Vector3(0, transform.position.y, 0), transform.position) < 8)
                {
                    state = State.Roaming;
                    controlAreaPos = transform.position;
                    controlAreaFound = true;
                }*/
                if (target != null) StartChaseState();
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
    void StartWalkToScreen()
    {
        anim.SetInteger("State", 1);
        agent.SetDestination(new Vector3(0, transform.position.y, 0));
        state = State.OutSideOfScreen;
    }
    void CheckForEnemies()
    {
        foreach (GameObject unitOrBuilding in gamemanager.buildingsAndUnits)
        {
            if (Vector3.Distance(unitOrBuilding.transform.position, transform.position) < attackRange && (target == null || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(unitOrBuilding.transform.position, transform.position)))
            {
                target = unitOrBuilding;
                attackScript.target = target;
                attackScript.targetHealth = target.GetComponent<ALL_Health>();
                attackScript.targetInRange = true;
                //StartAttackState();
            }
        }
        if (target == null) StartWalkToMiddle();
        else StartAttackState();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyOutsideScreen")) StartWalkToScreen();
        if (other.CompareTag("Enemy Tag Activator")) CheckForEnemies();
    }
    void StartWalkToMiddle()
    {
        anim.SetInteger("State", 1);
        agent.SetDestination(new Vector3(0, transform.position.y, 0));
        state = State.WalkToMiddle;
    }

    void ScanArea()
    {
        foreach (GameObject unitOrBuilding in gamemanager.buildingsAndUnits)
        {
            if (Vector3.Distance(unitOrBuilding.transform.position, transform.position) < targetScanningRange && (target == null || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(unitOrBuilding.transform.position, transform.position)))
            {
                target = unitOrBuilding;
                attackScript.target = target;
                attackScript.targetHealth = target.GetComponent<ALL_Health>();
                StartChaseState();
            }
        }
        /*Collider[] colliders = Physics.OverlapSphere(transform.position, targetScanningRange, layerMask);
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                if (target == null)
                {
                    target = col.gameObject;
                    attackScript.target = target;
                    attackScript.targetHealth = target.GetComponent<ALL_Health>();
                    StartChaseState();
                }
            }
        }*/
    }
    void StartChaseState()
    {
        attackScript.targetInRange = false;
        attackScript.target = target;
        attackScript.targetHealth = target.GetComponent<ALL_Health>();
        anim.SetInteger("State", 1);
        state = State.ChaseTarget;
    }
    void ApproachTarget()
    {
        if (Vector3.Distance(target.transform.position, transform.position) < attackRange) StartAttackState();

        foreach (GameObject unitOrBuilding in gamemanager.buildingsAndUnits)
        {
            if (unitOrBuilding != target && Vector3.Distance(unitOrBuilding.transform.position, transform.position) < attackRange)
            {
                target = unitOrBuilding;
                attackScript.target = target;
                attackScript.targetHealth = target.GetComponent<ALL_Health>();
                StartAttackState();
            }
        }
        /*Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, layerMask);
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                if (col.gameObject == target) StartAttackState();
                else
                {
                    target = col.gameObject;
                    attackScript.target = target;
                    attackScript.targetHealth = target.GetComponent<ALL_Health>();
                    StartAttackState();
                }
            }
        }*/
    }
    void StartAttackState()
    {
        attackDistance = Vector3.Distance(target.transform.position, transform.position);
        state = State.Attack;
        attackScript.SwitchToAttackState();
    }
    /*void ReturnToRoam()
    {
        timeBtwWalks = 0;
        anim.SetInteger("State", 0);
        state = State.Roaming;
    }*/

    /*IEnumerator RandomMovement()
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
    }*/
}