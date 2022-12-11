using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class enemymanager : MonoBehaviour
{
    [System.Serializable]
    public class Enemy
    {
        public GameObject enemyPrefab;
        public List<int> stages;
    }
    [System.Serializable]
    public class Boss
    {
        public GameObject bossPrefab;
        public List<int> spawnsInDay;
        public Transform spawnPoint;
        public string name;
        public string title;
        public Color32 textColor;
    }

    [SerializeField] private Enemy[] enemies;
    [SerializeField] private Boss[] bosses;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Debug")]
    [SerializeField] private List<GameObject> enemySpawnPool;
    public int stage = 1;
    [SerializeField] private int day = 0;

    [Header("Enemy Resources")]
    [SerializeField] private float EnemyStartResources;
    private float currentEnemyResources;

    [Header("Difficulty Settings")]
    [SerializeField] private float easyResPerSMult;
    [SerializeField] private float easySpawnIntMult;
    [SerializeField] private float hardResPerSMult;
    [SerializeField] private float hardSpawnIntMult;
    [SerializeField] private float dayMult;
    private float resourcesPerSMult = 1;
    private float spawnIntervalMult = 1;

    [Header("Stages")]
    [SerializeField] private int stagesInDay;
    [SerializeField] private float stageChangeInterval;
    [SerializeField] private float s1ResourcesPerS;
    [SerializeField] private float s1SpawnInterval;
    [SerializeField] private float s2ResourcesPerS;
    [SerializeField] private float s2SpawnInterval;
    [SerializeField] private float s3ResourcesPerS;
    [SerializeField] private float s3SpawnInterval;
    [SerializeField] private float s4ResourcesPerS;
    [SerializeField] private float s4SpawnInterval;
    [SerializeField] private float s5ResourcesPerS;
    [SerializeField] private float s5SpawnInterval;

    [Header("Other")]
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private TMP_Text bossTitleText;

    private float resourcesPerS;
    private float spawnInterval;
    private GameObject nextEnemy;
    private int previousSpawnPoint;
    private float maxEnemyResources = 10;
    private float timeBtwSpawns;
    private float timeBtwStageChanges;
    private GameObject currentBoss;
    private ALL_Health bossHealthScript;

    //sounds
    private bool dayEndMusicFadeStarted;

    //day night cycle
    private Animator anim;

    void Start()
    {
        if (MultiScene.multiScene.difficulty == 0)
        {
            resourcesPerSMult = easyResPerSMult;
            spawnIntervalMult = easySpawnIntMult;
        }
        else if (MultiScene.multiScene.difficulty == 2)
        {
            resourcesPerSMult = hardResPerSMult;
            spawnIntervalMult = hardSpawnIntMult;
        }
        enemySpawnPool = new List<GameObject>();
        anim = GetComponent<Animator>();
        gamemanager.dayCycleAnim.SetFloat("AnimLenght", 1 / (stageChangeInterval * 4) * 6);
        previousSpawnPoint = Random.Range(0, enemySpawnPoints.Length);
        spawnInterval = s1SpawnInterval;
        resourcesPerS = s1ResourcesPerS;
        currentEnemyResources = EnemyStartResources;
        timeBtwSpawns = 0;
        UpdateEnemyList();
        PickRandomEnemy();
    }
    void Update()
    {
        if (currentEnemyResources < maxEnemyResources) currentEnemyResources += Time.deltaTime * resourcesPerS * resourcesPerSMult * (dayMult * day +1);

        if (timeBtwSpawns >= spawnInterval / spawnIntervalMult / (dayMult * day +1))
        {
            SpawnEnemy();
            timeBtwSpawns = 0;
        }
        else timeBtwSpawns += Time.deltaTime;

        if (timeBtwStageChanges >= stageChangeInterval && stage != 5)
        {
            timeBtwStageChanges = 0;
            NextStage();
        }
        else timeBtwStageChanges += Time.deltaTime;
        if (stage == 5 && bossHealthScript != null) if (bossHealthScript.isDead == true)
            {
                dayEndMusicFadeStarted = false;
                timeBtwStageChanges = 0;
                gamemanager.dayCycleAnim.SetTrigger("BossKilled");
                StartCoroutine(SoundManager.Instance.DayFade());
                NextStage();
            }
        if (stage == 4 && timeBtwStageChanges >= stageChangeInterval - 3 && dayEndMusicFadeStarted == false) StartCoroutine(DayEndMusicFade());
    }
    IEnumerator DayEndMusicFade()
    {
        dayEndMusicFadeStarted = true;
        SoundManager.Instance.FadeMusic(3, false, 0);
        yield return new WaitForSeconds(2);
        gamemanager.musicPlayer.PlaySecondaryMusic();
    }
    void UpdateEnemyList()
    {
        if (enemySpawnPool.Count != 0) enemySpawnPool.Clear();
        foreach (Enemy enemy in enemies)
        {
            if (enemy.stages.Contains(stage)) enemySpawnPool.Add(enemy.enemyPrefab);
        }
    }
    void SpawnEnemy()
    {
        float nextEnemyCost = nextEnemy.GetComponent<EnemyUnit>().unitCost;

        if (nextEnemyCost <= currentEnemyResources && gamemanager.state == gamemanager.State.RunTime)
        {
            int randomSpawnPoint = Random.Range(0, enemySpawnPoints.Length);
            while (randomSpawnPoint == previousSpawnPoint) randomSpawnPoint = Random.Range(0, enemySpawnPoints.Length);
            previousSpawnPoint = randomSpawnPoint;
            Instantiate(nextEnemy, enemySpawnPoints[randomSpawnPoint].position, Quaternion.identity);
            currentEnemyResources -= nextEnemyCost;
            PickRandomEnemy();
        }
    }
    void PickRandomEnemy()
    {
        int randomEnemy = Random.Range(0, enemySpawnPool.Count);
        nextEnemy = enemySpawnPool[randomEnemy];
    }
    void NextStage()
    {
        if (stage == stagesInDay)
        {
            stage = 1;
            day += 1;
        }
        else stage += 1;

        if (stage == stagesInDay) SpawnBoss();

        UpdateEnemyList();

        if (stage == 1)
        {
            spawnInterval = s1SpawnInterval;
            resourcesPerS = s1ResourcesPerS;
        }
        else if (stage == 2)
        {
            spawnInterval = s2SpawnInterval;
            resourcesPerS = s2ResourcesPerS;
        }
        else if (stage == 3)
        {
            spawnInterval = s3SpawnInterval;
            resourcesPerS = s3ResourcesPerS;
        }
        else if (stage == 4)
        {
            spawnInterval = s4SpawnInterval;
            resourcesPerS = s4ResourcesPerS;
        }
        else if (stage == 5)
        {
            spawnInterval = s5SpawnInterval;
            resourcesPerS = s5ResourcesPerS;
        }
    }
    void SpawnBoss()
    {
        foreach (Boss boss in bosses)
        {
            if (boss.spawnsInDay.Contains(day)) currentBoss = Instantiate(boss.bossPrefab, boss.spawnPoint.position, boss.spawnPoint.rotation);     // sit kun on useempi bossi niin if(boss.spawnsInDay == day)
            bossHealthScript = currentBoss.GetComponent<ALL_Health>();
            bossNameText.text = boss.name;
            bossNameText.color = boss.textColor;
            bossTitleText.text = boss.title;
            bossTitleText.color = boss.textColor;
            bossHealthScript.maxHealth *= resourcesPerSMult * (dayMult * day + 1);
            bossHealthScript.currentHealth *= resourcesPerSMult * (dayMult * day + 1);
        }       
    }
    public void BossIntroText()
    {
        bossNameText.transform.parent.GetComponent<Animator>().SetTrigger("Reveal");
    }
}
