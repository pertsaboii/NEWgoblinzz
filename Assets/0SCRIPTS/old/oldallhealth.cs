using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class oldallhealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float currentHealth;

    [SerializeField] private Image hpSprite;
    [SerializeField] private GameObject hpBar;

    public GameObject unitThatSpawns;
    [SerializeField] private bool spawnsUnit;
    [SerializeField] private bool isBuilding;
    [SerializeField] private bool dealsDmgOnDeath;

    [HideInInspector] public float deathDmgRadius;
    [SerializeField] private LayerMask deathDmgTargetType;
    [HideInInspector] public float deathDamage;

    [HideInInspector] public bool isDead;
    void Start()
    {
        currentHealth = maxHealth;
        hpSprite.fillAmount = currentHealth / maxHealth;
        hpBar.SetActive(false);
    }
    public void UpdateHealth(float healthChange)
    {
        if (currentHealth <= maxHealth) hpBar.SetActive(true);
        currentHealth += healthChange;
        hpSprite.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0) Death();
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            hpBar.SetActive(false);
        }
    }
    private void Update()
    {
        hpBar.transform.LookAt(new Vector3(1, gamemanager.screenInputCamera.transform.position.y * 10, gamemanager.screenInputCamera.transform.position.z * 10));
    }
    void Death()
    {
        if (gameObject.layer == 7 || gameObject.layer == 8) gamemanager.buildingsAndUnits.Remove(gameObject);
        if (gameObject.CompareTag("Enemy") == true) gamemanager.enemies.Remove(gameObject);
        if (gameObject.CompareTag("Building") == true) gamemanager.buildings.Remove(gameObject);
        if (spawnsUnit == true) Instantiate(unitThatSpawns, transform.position, Quaternion.identity);
        if (isBuilding == true) gamemanager.buildings.Remove(this.gameObject);
        if (dealsDmgOnDeath)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, deathDmgRadius, deathDmgTargetType);
            if (colliders != null) foreach (Collider col in colliders) col.gameObject.GetComponent<ALL_Health>().UpdateHealth(-deathDamage);
        }
        isDead = true;
        Invoke("DestroyGO", .1f);   // tähän myöhemmin: jokaiselle tuhoutuvalle asialle oma scripti joka määrittää mitä tapahtuu kuollessa
    }
    void DestroyGO()
    {
        Destroy(gameObject);
    }
}
