using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmove : MonoBehaviour
{
    private enum State
    {
        Roaming, ChaseTarget, Attack
    }

    private State state;
    private Rigidbody rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float targetScanningRange;
    public GameObject target = null;

    private float timeBtwWalks;
    [SerializeField] private float walkCycleTime;
    [SerializeField] private float idleTime;
    private Vector3 originalPos;
    [SerializeField] private float wanderingRange;

    private Vector3 randomDir;
    private int idleMultiplier;

    [SerializeField] private float attackRange;

    private Animator anim;

    [SerializeField] private LayerMask layerMask;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("WalkSpeed", moveSpeed / 2);
        rb = GetComponent<Rigidbody>();
        timeBtwWalks = 0;
        idleMultiplier = 0;
        state = State.Roaming;
        anim.SetInteger("State", 0);

        originalPos = transform.position;
    }


    void Update()
    {
        switch (state)
        {
            default:
            case State.Roaming:
                if (timeBtwWalks <= 0)
                {
                    StartCoroutine("RandomMovement");
                    timeBtwWalks = walkCycleTime + idleTime;
                }
                else timeBtwWalks -= Time.deltaTime;

                if (Vector3.Distance(transform.position, originalPos) >= wanderingRange) transform.LookAt(originalPos);

                ScanArea();
                break;
            case State.ChaseTarget:
                if (target != null) transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                ApproachTarget();
                if (target == null) ReturnToRoam();
                break;
            case State.Attack:
                idleMultiplier = 0;
                anim.SetInteger("State", 2);
                if (target == null) ReturnToRoam();
                break;
        }
    }
    void ReturnToRoam()
    {
        timeBtwWalks = 0;
        state = State.Roaming;
    }

    void ScanArea()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, targetScanningRange * 2, layerMask);
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                if (target == null)
                {
                    target = col.gameObject;
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
                if (col.gameObject == target) state = State.Attack;
                else
                {
                    target = col.gameObject;
                    state = State.Attack;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Unit") && target == null) transform.LookAt(originalPos);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * moveSpeed * idleMultiplier;
    }

    IEnumerator RandomMovement()
    {
        idleMultiplier = 0;
        anim.SetInteger("State", 0);

        yield return new WaitForSeconds(idleTime);

        anim.SetInteger("State", 1);
        idleMultiplier = 1;
        randomDir = new Vector3(Random.Range(-1f, 1f), transform.position.y, Random.Range(-1f, 1f));
        transform.LookAt(randomDir);
    }
}