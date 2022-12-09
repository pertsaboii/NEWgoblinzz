using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [HideInInspector] public ALL_Health targetHealth;
    public GameObject target;
    private Rigidbody rb;
    private Vector3 targetDir;
    private Collider targetCollider;
    private Transform localTransform;

    [SerializeField] private LayerMask targetLayer;

    private Collider ragdollCollider;
    [SerializeField] private Collider triggerCol;

    [HideInInspector] public GameObject whoSpawnedProjectile;
    private GameObject hitCollider;

    public MeshRenderer mr;
    void Start()
    {
        mr.enabled = false;
        ragdollCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        if (target != null) targetCollider = target.GetComponent<Collider>();
        localTransform = GetComponent<Transform>();
        Invoke("ActivateMesh", .08f); // koska keih‰‰t aluksi v‰‰r‰ss‰ asennossa
    }
    private void FixedUpdate()
    {
        if (target != null)
        {
            rb.velocity = localTransform.forward * speed;

            targetDir = targetCollider.bounds.center - localTransform.position;                   // jotta ei osuisi jalkoihin

            var projectileTargetRotation = Quaternion.LookRotation(targetDir);

            rb.MoveRotation(Quaternion.RotateTowards(localTransform.rotation, projectileTargetRotation, turnSpeed));
        }
    }
    private void Update()
    {
        if (target == null || targetHealth.isDead == true) StartCoroutine(TargetlessProjectile());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target && ((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            hitCollider = other.gameObject;
            ProjectileHit();
        }
        else if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            hitCollider = other.gameObject;
            targetHealth = other.gameObject.GetComponent<ALL_Health>();
            ProjectileHit();
        }
        else return;
    }
    void ProjectileHit()
    {
        if (hitCollider.GetComponent<U_AI>() == true)
        {
            U_AI meleegobbo = hitCollider.GetComponent<U_AI>();
            if (meleegobbo.target == null) meleegobbo.target = whoSpawnedProjectile;
        }
        else if (hitCollider.GetComponent<E_AI>() == true)
        {
            E_AI enemy = hitCollider.GetComponent<E_AI>();
            if (enemy.target == null) enemy.target = whoSpawnedProjectile;
        }

        targetHealth.UpdateHealth(-damage);
        Destroy(gameObject);
    }
    IEnumerator TargetlessProjectile()
    {
        triggerCol.enabled = false;
        rb.useGravity = true;
        ragdollCollider.enabled = true;

        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
    }
    void ActivateMesh()
    {
        mr.enabled = true;
    }
}
