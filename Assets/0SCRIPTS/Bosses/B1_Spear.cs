using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class B1_Spear : MonoBehaviour
{
    // melee specs
    [HideInInspector] public bool canDamage;
    [HideInInspector] public float meleeDamage;
    public Collider meleeTriggerCol;

    [Header("Throw")]
    public NavMeshObstacle spearObstacle;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public float throwDamage;
    public Collider onGroundCol;
    public Collider throwTriggerCol;
    [HideInInspector] public bool spearLookAtTarget;
    [HideInInspector] public Vector3 targetPos;
    private Transform localTransform;
    [HideInInspector] public GameObject target;
    [HideInInspector] public ALL_Health targetHealth;

    //debug
    [SerializeField] private bool debugMode;
    [SerializeField] private GameObject debugTarget;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        localTransform = GetComponent<Transform>();
    }
    private void Update()
    {
        if (spearLookAtTarget == true || debugMode == true) SpearLookAtTarget();
        if (target != null && gameObject.transform.parent == null)                       // t‰m‰n pit‰isi est‰‰ sen ett‰ spear lent‰‰ ikuisesti mutta aiheuttaa spearin bugaamista
            if (targetHealth.isDead == true)
            {
                throwTriggerCol.enabled = false;
                spearLookAtTarget = false;
                rb.isKinematic = true;
                onGroundCol.enabled = true;
                spearObstacle.enabled = true;
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage == true && (other.gameObject.layer == 7 || other.gameObject.layer == 8)) other.gameObject.GetComponent<ALL_Health>().UpdateHealth(-meleeDamage);
        else if (rb.isKinematic == false && (other.gameObject.layer == 7 || other.gameObject.layer == 8))
        {
            throwTriggerCol.enabled = false;
            spearLookAtTarget = false;
            rb.isKinematic = true;
            other.gameObject.GetComponent<ALL_Health>().UpdateHealth(-throwDamage);
            onGroundCol.enabled = true;
            spearObstacle.enabled = true;
        }
    }
    void SpearLookAtTarget()
    {
        if (debugMode == true) targetPos = debugTarget.transform.GetComponent<Collider>().bounds.center;
        var projectileTargetRotation = Quaternion.LookRotation(targetPos - localTransform.position);

        rb.MoveRotation(Quaternion.RotateTowards(localTransform.rotation, projectileTargetRotation, 50));
    }
}
