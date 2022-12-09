using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rigidram : MonoBehaviour
{
    private enum State
    {
        ApproachTarget
    }

    private State state;
    private GameObject target;

    [SerializeField] private float buildingDamage;
    [SerializeField] private float unitColDamage;

    private int currentBuildingAmount;

    private ALL_Health healthScript;

    private Rigidbody rb;

    private float speed;
    [SerializeField] private float startSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelerationPerS;
    [SerializeField] private float turnSpeed;
    private Vector3 targetDir;
    private Collider targetCollider;
    private Transform localTransform;

    private EnemyUnit baseScript;

    private bool impactDone;

    [SerializeField] private Transform hitFXSpawnPoint;

    private CameraShake cameraShake;

    void Start()
    {
        speed = startSpeed;
        rb = GetComponent<Rigidbody>();
        healthScript = GetComponent<ALL_Health>();
        localTransform = GetComponent<Transform>();
        baseScript = GetComponent<EnemyUnit>();
        cameraShake = gamemanager.screenInputCamera.GetComponent<CameraShake>();
        LockOnTarget();
    }

    void LockOnTarget()
    {
        foreach (GameObject building in gamemanager.buildings)
        {
            if (target == null || Vector3.Distance(building.transform.position, gameObject.transform.position) < Vector3.Distance(target.transform.position, gameObject.transform.position))
            {
                target = building;
                state = State.ApproachTarget;
                targetCollider = target.GetComponent<Collider>();
            }
        }
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
        if (speed <= maxSpeed) speed += Time.deltaTime * accelerationPerS;

        if (target != null) baseScript.target = target;                                 // jos on suorituskykyongelmia niin tämän voi siirtää voideihin
        else baseScript.target = null;
    }
    private void FixedUpdate()
    {
        if (target != null)
        {
            rb.velocity = localTransform.forward * speed;

            targetDir = target.transform.position - localTransform.position;                   // jos kääntyy vituiksi niin tätä muokkaamalla voi korjata

            var projectileTargetRotation = Quaternion.LookRotation(targetDir);

            rb.MoveRotation(Quaternion.RotateTowards(localTransform.rotation, projectileTargetRotation, turnSpeed));
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (state == State.ApproachTarget && collision.gameObject.CompareTag("Building"))
        {
            if (impactDone == false) ImpactToTarget();
        }
        // unitin dmg testi
        else if (collision.collider.CompareTag("Unit"))
        {
            collision.collider.gameObject.GetComponent<ALL_Health>().UpdateHealth(-unitColDamage);
            speed -= 1;
        }
    }
    void ImpactToTarget()
    {
        Instantiate(gamemanager.assetBank.FindFX(AssetBank.FXType.BatteringRamHit), hitFXSpawnPoint.position, Quaternion.identity);
        cameraShake.StartShakeCamera();
        impactDone = true;
        target.GetComponent<ALL_Health>().UpdateHealth(-buildingDamage);
        healthScript.UpdateHealth(-healthScript.currentHealth - 1);
    }
}
