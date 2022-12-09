using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObstacleAgent), typeof(ALL_Health), typeof(All_AttackScript))]
public class U_AI : MonoBehaviour
{
    private enum State
    {
        CheckForEnemies, Attack, Roaming, ChaseTarget
    }

    private ObstacleAgent agent;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private bool isRanged;

    private State state;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkAnimMult = 2;
    [SerializeField] private float targetScanningRange;
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

    private All_AttackScript attackScript;
    private float attackDistance;

    private Rigidbody rb;

    [SerializeField] private string currentState;
    private bool spawnCheckDone;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        anim.SetFloat("WalkSpeed", moveSpeed / walkAnimMult);
        timeBtwWalks = 0;
        anim.SetInteger("State", 0);
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        agent = GetComponent<ObstacleAgent>();
        attackScript = GetComponent<All_AttackScript>();
        originalPos = transform.position;
        state = State.CheckForEnemies;
    }

    void Update()
    {
        switch (state)
        {
            default:
            case State.CheckForEnemies:
                if (spawnCheckDone == false)
                {
                    CheckForEnemies();
                    spawnCheckDone = true;
                }
                break;
            case State.Roaming:
                ScanArea();
                if (Vector3.Distance(randomPos, transform.position) < 0.1f && navMeshAgent.enabled == true)
                {
                    anim.SetInteger("State", 0);
                    navMeshAgent.ResetPath();
                }
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
                break;
            case State.ChaseTarget:
                if (anim.GetInteger("State") != 1) anim.SetInteger("State", 1); // joskus j‰‰ attack animation p‰‰lle, t‰m‰ est‰‰ sen               
                if (target != null) agent.SetDestination(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                if (target == null) ReturnToRoam();
                else ApproachTarget();
                break;
            case State.Attack:
                if (isRanged == true && target != null && Vector3.Distance(target.transform.position, transform.position) > attackRange) StartChaseState();
                else if (isRanged == false && target != null && Vector3.Distance(target.transform.position, transform.position) > attackDistance + 0.3f) StartChaseState();
                if (navMeshAgent.enabled == true) navMeshAgent.ResetPath();
                if (target == null)
                {
                    if (isRanged == true)
                    {
                        foreach (GameObject enemy in gamemanager.enemies)
                        {
                            if (enemy.gameObject.layer == 6 && Vector3.Distance(enemy.transform.position, transform.position) < attackRange && (target == null || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(enemy.transform.position, transform.position)))
                            {
                                target = enemy;
                            }
                        }
                        if (target == null) ReturnToRoam();
                        else StartAttackState();
                    }
                    else
                    {
                        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, layerMask);
                        if (colliders.Length != 0)
                        {
                            foreach (Collider col in colliders)
                            {
                                if (target == null)
                                {
                                    target = col.gameObject;
                                    StartAttackState();
                                    return;
                                }
                            }
                        }
                    }
                    if (target == null) ReturnToRoam();
                }
                if (target != null) transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                break;
        }

        if (rb.velocity != Vector3.zero) rb.velocity = Vector3.zero;

        if (attackScript.target != null && attackScript.targetHealth.isDead == true)    // t‰m‰n voisi siirt‰‰ all_healthin death voidiin
        {
            attackScript.target = null;
            target = null;
        }
        currentState = state.ToString();
    }
    void CheckForEnemies()
    {
        if (isRanged == true)
        {
            foreach (GameObject enemy in gamemanager.enemies)
            {
                if (enemy.gameObject.layer == 6 && Vector3.Distance(enemy.transform.position, transform.position) < attackRange && (target == null || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(enemy.transform.position, transform.position)))
                {
                    target = enemy;
                }
            }
            if (target != null)
            {
                StartAttackState();
                return;
            }
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, layerMask);
            if (colliders.Length != 0)
            {
                foreach (Collider col in colliders)
                {
                    if (target == null)
                    {
                        target = col.gameObject;
                        StartAttackState();
                        return;
                    }
                }
            }
        }
        if (target == null) ReturnToRoam();
        else StartChaseState();
    }
    void ScanArea()
    {
        foreach (GameObject enemy in gamemanager.enemies)
        {
            if (enemy.gameObject.layer == 6 && Vector3.Distance(enemy.transform.position, transform.position) < targetScanningRange && (target == null || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(enemy.transform.position, transform.position)))
            {
                target = enemy;
                attackScript.target = target;
                attackScript.targetHealth = target.GetComponent<ALL_Health>();
            }
        }
        if (target != null) StartChaseState();
    }
    void StartChaseState()
    {
        anim.SetInteger("State", 1);
        attackScript.targetInRange = false;
        attackScript.target = target;
        attackScript.targetHealth = target.GetComponent<ALL_Health>();
        state = State.ChaseTarget;
    }
    void ApproachTarget()
    {
        if (isRanged == true)
        {
            if (Vector3.Distance(target.transform.position, transform.position) < attackRange)
            {
                StartAttackState();
                return;
            }

            foreach (GameObject enemy in gamemanager.enemies)
            {
                if (enemy != target && Vector3.Distance(enemy.transform.position, transform.position) < attackRange)
                {
                    target = enemy;
                    StartAttackState();
                    return;
                }
            }
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, layerMask);
            if (colliders.Length != 0)
            {
                foreach (Collider col in colliders)
                {
                    if (col.gameObject == target)
                    {
                        StartAttackState();
                        return;
                    }
                    else
                    {
                        target = col.gameObject;
                        StartAttackState();
                        return;
                    }
                }
            }
        }
    }
    void StartAttackState()
    {
        attackDistance = Vector3.Distance(target.transform.position, transform.position);
        attackScript.target = target;
        attackScript.targetHealth = target.GetComponent<ALL_Health>();
        attackScript.targetInRange = true;
        state = State.Attack;
        attackScript.SwitchToAttackState();
    }
    void ReturnToRoam()
    {
        timeBtwWalks = 0;
        anim.SetInteger("State", 0);
        state = State.Roaming;
    }
    IEnumerator RandomMovement()
    {
        if (state == State.Roaming)
        {
            randomPos = transform.position;
            if (navMeshAgent.enabled == true) navMeshAgent.ResetPath();
            anim.SetInteger("State", 0);

            yield return new WaitForSeconds(idleTime);

            anim.SetInteger("State", 1);

            while (Vector3.Distance(randomPos, transform.position) < 4) CalculateRandomNavMeshPoint();
            agent.SetDestination(randomPos);
        }
        else StopCoroutine(RandomMovement());
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
