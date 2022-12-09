using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle), typeof(ALL_Health))]
public class B_rangedtower : MonoBehaviour
{
    private enum State
    {
        ScanForEnemies, Attack
    }

    private State state;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] projectileSpawnPoints;

    [SerializeField] private float attackSpeed;
    private float timeBtwAttacks;
    [SerializeField] private float attackRange;
    [SerializeField] private AudioClip spawnSound;

    private ALL_Health targetHealth;
    public GameObject target;
    public List<GameObject> targets;
    void Start()
    {
        gamemanager.buildings.Add(gameObject);
        gamemanager.buildingsAndUnits.Add(gameObject);
        SoundManager.Instance.PlaySFXSound(spawnSound);
        state = State.ScanForEnemies;
    }
    void Update()
    {
        switch (state)
        {
            default:
            case State.ScanForEnemies:
                ScanArea();
                break;
            case State.Attack:
                if (target == null || targetHealth.isDead == true) StartScanState();
                else if (timeBtwAttacks >= attackSpeed)
                {
                    timeBtwAttacks = 0;
                    SpawnProjectile();
                }
                else timeBtwAttacks += Time.deltaTime;
                break;
        }
    }
    void StartScanState()
    {
        state = State.ScanForEnemies;
    }
    void StartAttackState()
    {
        state = State.Attack;
        timeBtwAttacks = 0;
    }
    void ScanArea()
    {
        foreach (GameObject enemy in gamemanager.enemies)
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) < attackRange && (target == null || Vector3.Distance(target.transform.position, transform.position) > Vector3.Distance(enemy.transform.position, transform.position)))
            {
                target = enemy;
                targetHealth = target.GetComponent<ALL_Health>();
                StartAttackState();
            }
        }
    }
    void SpawnProjectile()
    {
        Transform nearestSpawnPoint = projectileSpawnPoints[0];
        foreach (Transform spawnPoint in projectileSpawnPoints)
        {
            if (nearestSpawnPoint == null || Vector3.Distance(target.transform.position, nearestSpawnPoint.position) < Vector3.Distance(target.transform.position, spawnPoint.position))
            {
                nearestSpawnPoint = spawnPoint;
            }
        }
        GameObject spawnedProjectile = Instantiate(projectilePrefab, nearestSpawnPoint.position, Quaternion.identity);
        projectile projectile = spawnedProjectile.GetComponent<projectile>();
        projectile.target = target;
        projectile.targetHealth = targetHealth;
        projectile.whoSpawnedProjectile = this.gameObject;
    }
}
