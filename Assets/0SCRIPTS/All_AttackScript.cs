using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class All_AttackScript : MonoBehaviour
{
    private enum State
    {
        NotAttacking, Attacking
    }
    private State state;

    public bool targetInRange;
    private Animator anim;

    [SerializeField] private float attackSpeed;

    [SerializeField] private int attackDamage;
    [HideInInspector] public GameObject target;
    [HideInInspector] public ALL_Health targetHealth;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [SerializeField] private Transform aoeDmgOrigin;
    [SerializeField] private float aoeRadius;
    [SerializeField] private LayerMask aoeDmgTargets;
    [SerializeField] private GameObject aoeFX;

    [SerializeField] private string currentState;

    private CameraShake cameraShake;
    void Start()
    {
        anim = GetComponent<Animator>();
        state = State.NotAttacking;
        if (aoeFX != null) cameraShake = gamemanager.mainCineCam.GetComponent<CameraShake>();         // tähän myöhemmin typet enumeilla
    }

    void Update()
    {
        switch (state)
        {
            default:
            case State.NotAttacking:
                break;
            case State.Attacking:
                if (anim.GetInteger("State") != 2) anim.SetInteger("State", 2);
                if (target == null || targetInRange == false) state = State.NotAttacking;
                if (target == null) targetInRange = false;
                break;
        }
        currentState = state.ToString();
    }
    public void SwitchToAttackState()
    {
        anim.SetInteger("State", 2);
        state = State.Attacking;
        anim.SetFloat("AttackSpeed", attackSpeed);
    }
    void SingleTargetMeleeDmg()
    {
        if (target != null && targetInRange == true) targetHealth.UpdateHealth(-attackDamage);      // jos haluaa että dmg välittyy targettiin joka on rangen ulkopuolella tässä vaiheessa niin targetinrangen voi ottaa pois
        else targetInRange = false;
    }
    void SpawnProjectile()
    {
        GameObject spawnedProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        projectile projectile = spawnedProjectile.GetComponent<projectile>();
        projectile.target = target;
        projectile.targetHealth = targetHealth;
        projectile.whoSpawnedProjectile = this.gameObject;
    }
    void AoeMeleeDmg()
    {
        Instantiate(aoeFX, aoeDmgOrigin.transform.position, Quaternion.identity);
        cameraShake.StartShakeCamera();
        Collider[] colliders = Physics.OverlapSphere(aoeDmgOrigin.position, aoeRadius, aoeDmgTargets);
        {
            foreach (Collider col in colliders)
            {
                col.GetComponent<ALL_Health>().UpdateHealth(-attackDamage);
            }
        }
    }
}
