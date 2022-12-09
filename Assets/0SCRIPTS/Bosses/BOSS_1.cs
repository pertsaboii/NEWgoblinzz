using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BOSS_1 : MonoBehaviour
{
    private enum State
    {
        Spawn, WalkToChief, SweepAttacking, ThrowAttacking, FetchingSpear, PickingUpSpear, StabAttacking
    }
    private State state;

    [Header("Setup")]
    [SerializeField] private GameObject spear;
    [SerializeField] private LayerMask dropLayerMask;
    [SerializeField] private Transform spearParent;
    private Quaternion spearOriginalRotation;
    private Vector3 spearOriginalPos;
    private B1_Spear spearScript;
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    private Rigidbody rb;
    private GameObject chief;
    private CameraShake cameraShake;
    private ALL_Health targetHealth;
    private EnemyUnit baseScript;

    [Header("Stats")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float walkAnimMult;
    [SerializeField] private float runSpeed;
    [SerializeField] private float runAnimMult;
    [SerializeField] private float sweepDamage;
    [SerializeField] private float sweepAttackRange;
    [SerializeField] private float stabDamage;
    [SerializeField] private float stabAttackRange;
    [SerializeField] private float throwDamage;
    [SerializeField] private float throwAttackRange;
    [SerializeField] private float dropDamage;
    [SerializeField] private float dropDamageRadius;

    [Header("Debug")]
    public GameObject target = null;
    [SerializeField] private string currentState;

    [Header("Look At Turn Speed")]
    [SerializeField] private float turnSpeed;
    private Vector3 targetDir;
    private Transform localTransform;

    // throw variables
    private Vector3 targetCurrentPos;

    // spear timed returnal
    private float timeWithoutSpear;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        baseScript = GetComponent<EnemyUnit>();
        chief = gamemanager.loseCon;
        cameraShake = gamemanager.mainCineCam.GetComponent<CameraShake>();
        anim.SetFloat("WalkSpeed", walkSpeed * walkAnimMult);
        anim.SetFloat("RunSpeed", runSpeed * runAnimMult);
        localTransform = GetComponent<Transform>();
        spearScript = spear.GetComponent<B1_Spear>();
        //spearScript.sweepDamage = sweepDamage;    obsolete koska stab
        spearScript.throwDamage = throwDamage;
        spearOriginalRotation = spear.transform.localRotation;
        spearOriginalPos = spear.transform.localPosition;
        state = State.Spawn;

        Invoke("DropFromSky", 0);
    }
    
    void Update()
    {
        switch (state)
        {
            default:
            case State.Spawn:
                break;
            case State.WalkToChief:
                LookAtTarget();
                ScanArea();
                break;
            case State.SweepAttacking:
                LookAtTarget();
                break;
            case State.StabAttacking:
                LookAtTarget();
                break;
            case State.ThrowAttacking:
                LookAtTarget();
                break;
            case State.FetchingSpear:
                LookAtTarget();
                if (Vector3.Distance(spear.transform.position, transform.position) < 4) PickUpSpear();
                if (timeWithoutSpear >= 4)
                {
                    PickUpSpear();
                    timeWithoutSpear = 0;
                }
                else timeWithoutSpear += Time.deltaTime;
                break;
            case State.PickingUpSpear:
                LookAtTarget();
                break;
        }
        currentState = state.ToString();
        if (state != State.Spawn && rb.velocity != Vector3.zero) rb.velocity = Vector3.zero;
        if (target != null)
            if (targetHealth.isDead == true) target = null;


    }
    void DropFromSky()
    {
        rb.useGravity = true;
        rb.AddForce(Vector3.down * 28000, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == State.Spawn && collision.gameObject.layer == 10)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, dropDamageRadius, dropLayerMask);
            {
                if (cols.Length != 0) foreach (Collider col in cols)
                    {
                        col.gameObject.GetComponent<ALL_Health>().UpdateHealth(-dropDamage);
                    }
            }
            anim.SetTrigger("Land");
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            gameObject.layer = 6;
            Instantiate(gamemanager.assetBank.FindFX(AssetBank.FXType.GroundDustWave), transform.position, Quaternion.identity);
            cameraShake.StartShakeCamera();
            gamemanager.enemyManager.BossIntroText();
        }
    }
    void SpawnDone()    // callataan land-animaation lopussa
    {
        navMeshAgent.enabled = true;
        if (navMeshAgent.isStopped == true) navMeshAgent.isStopped = false;
        navMeshAgent.speed = walkSpeed;
        state = State.WalkToChief;
        navMeshAgent.SetDestination(chief.transform.position);
    }
    void StartWalkToChief()
    {
        if (navMeshAgent.enabled == false) navMeshAgent.enabled = true;
        if (navMeshAgent.isStopped == true) navMeshAgent.isStopped = false;
        anim.SetTrigger("Walk");
        state = State.WalkToChief;
        navMeshAgent.speed = walkSpeed;
        navMeshAgent.SetDestination(chief.transform.position);
    }
    void ScanArea()
    {
        foreach (GameObject unitOrBuilding in gamemanager.buildingsAndUnits)    // sweep attack scan
        {
            if (Vector3.Distance(unitOrBuilding.transform.position, transform.position) < sweepAttackRange && (target == null || targetHealth.isDead == true || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(unitOrBuilding.transform.position, transform.position)))
            {
                target = unitOrBuilding;
                baseScript.target = target;
                targetHealth = target.GetComponent<ALL_Health>();
            }

        }
        if (target != null)
        {
            if (targetHealth.isDead == false)
                if (Vector3.Distance(target.transform.position, transform.position) < sweepAttackRange)
                {
                    if (state != State.SweepAttacking) StartMeleeAttacking(0);
                    return;
                }
        }
        foreach (GameObject unitOrBuilding in gamemanager.buildingsAndUnits)    // stab attack scan
        {
            if (Vector3.Distance(unitOrBuilding.transform.position, transform.position) < stabAttackRange && (target == null || targetHealth.isDead == true || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(unitOrBuilding.transform.position, transform.position)))
            {
                target = unitOrBuilding;
                baseScript.target = target;
                targetHealth = target.GetComponent<ALL_Health>();
            }

        }
        if (target != null)
        {
            if (targetHealth.isDead == false)
                if (Vector3.Distance(target.transform.position, transform.position) < stabAttackRange)
                {
                    Debug.Log("stab");
                    if (state != State.StabAttacking) StartMeleeAttacking(1);
                    return;
                }
        }
        foreach (GameObject unitOrBuilding in gamemanager.buildingsAndUnits)    // ranged attack scan
        {
            if (Vector3.Distance(unitOrBuilding.transform.position, transform.position) < throwAttackRange && (target == null || targetHealth.isDead == true || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(unitOrBuilding.transform.position, transform.position)))
            {
                target = unitOrBuilding;
                baseScript.target = target;
                targetHealth = target.GetComponent<ALL_Health>();
                spearScript.target = target;
                spearScript.targetHealth = targetHealth;
            }

        }
        if (target != null)
            if (targetHealth.isDead == false)
            {
                StartThrowAttacking();
                return;
            }
        if (state != State.WalkToChief)
        {
            target = null;
            StartWalkToChief();
        }
    }
    void StartMeleeAttacking(int attackStyle)
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
        if (attackStyle == 0)
        {
            state = State.SweepAttacking;
            spearScript.meleeDamage = sweepDamage;
            anim.SetTrigger("Sweep");
        }
        else
        {
            state = State.StabAttacking;
            spearScript.meleeDamage = stabDamage;
            anim.SetTrigger("Stab");
        }
    }
    void LookAtTarget()
    {
        if (state == State.WalkToChief) targetDir = chief.transform.position - localTransform.position;
        else if ((state == State.SweepAttacking || state == State.ThrowAttacking || state == State.StabAttacking) && target != null) targetDir = target.transform.position - localTransform.position;
        else if (state == State.FetchingSpear || state == State.PickingUpSpear) targetDir = spear.transform.position - localTransform.position;

        var lookAtTargetRotation = Quaternion.LookRotation(targetDir);

        rb.MoveRotation(Quaternion.RotateTowards(localTransform.rotation, lookAtTargetRotation, turnSpeed));
    }
    void SpearDamageable()
    {
        spearScript.meleeTriggerCol.enabled = !spearScript.meleeTriggerCol.enabled;
        spearScript.canDamage = !spearScript.canDamage;
        Debug.Log(spearScript.canDamage);
    }
    void StartThrowAttacking()
    {
        if (navMeshAgent.enabled == true)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
        }
        anim.SetTrigger("Throw");
        state = State.ThrowAttacking;
        targetCurrentPos = target.GetComponent<Collider>().bounds.center;
    }
    void ThrowSpear()
    {
        spearScript.spearLookAtTarget = !spearScript.spearLookAtTarget;
        spearScript.targetPos = targetCurrentPos;
        spearScript.meleeTriggerCol.enabled = false;
        spearScript.throwTriggerCol.enabled = true;
        spear.transform.parent = null;
        spearScript.rb.isKinematic = false;
        if (target != null && targetHealth.isDead == false) spearScript.rb.AddForce((target.GetComponent<Collider>().bounds.center - spear.transform.position) * 250);
        else spearScript.rb.AddForce((targetCurrentPos - spear.transform.position) * 250);
    }
    void StartFetching()
    {
        if (navMeshAgent.enabled == false) navMeshAgent.enabled = true;
        state = State.FetchingSpear;
        navMeshAgent.speed = runSpeed;
        navMeshAgent.SetDestination(spear.transform.position);
    }
    void PickUpSpear()
    {
        state = State.PickingUpSpear;
        navMeshAgent.isStopped = true;
        spearScript.spearObstacle.enabled = false;
        spearScript.onGroundCol.enabled = false;
        anim.SetTrigger("PickSpear");
    }
    void ReturnSpearParent()    // callataan pick-animaation lopussa
    {
        spear.transform.SetParent(spearParent);
        spear.transform.localPosition = spearOriginalPos;
        spear.transform.localRotation = spearOriginalRotation;
        //spearScript.meleeTriggerCol.enabled = true;       ei tarvii?
    }
}
