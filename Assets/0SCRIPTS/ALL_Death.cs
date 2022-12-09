using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ALL_Death : MonoBehaviour
{
    private enum DeathFXType
    {
        GobboUnitDeath, EnemyDeath, WanderingDeath, TrollDeath, BuildingDeath
    }
    [Header("General")]
    private float deathTweenTime = 0.15f;
    [SerializeField] private bool hasDeathAnim;
    [SerializeField] private float deathAnimDuration;
    [SerializeField] private DeathFXType deathFXType;
    [SerializeField] private Collider col;
    [Header("Unit Spawn Upon Death")]
    [SerializeField] private bool spawnsUnitOnDeath;
    [SerializeField] private GameObject spawningUnit;
    [SerializeField] private Transform spawnPoint;
    [Header("AoE Dmg Upon Death")]
    [SerializeField] private bool dealsAoeDmgOnDeath;
    [SerializeField] private float deathDmgAmount;
    [SerializeField] private float deathDmgRadius;
    [SerializeField] private LayerMask deathDmgLayerMask;
    void Start()
    {
        if (spawnPoint == null) spawnPoint = transform;
        col = GetComponent<Collider>();
    }

    public IEnumerator Death()
    {
        col.enabled = false;       // jos jotain menee rikki nii tästä ehkä johtuu

        if (gameObject.CompareTag("Enemy"))
        {
            MultiScene.multiScene.money += gameObject.GetComponent<EnemyUnit>().value * MultiScene.multiScene.moneyMult;
            gamemanager.userInterface.score += gameObject.GetComponent<EnemyUnit>().value * MultiScene.multiScene.moneyMult;
            gamemanager.userInterface.UpdateMoneyText();
            gamemanager.userInterface.UpdateScoreText();
        }
        RemoveLayerAndTag(gameObject);

        if (hasDeathAnim == false)
        {
            gameObject.transform.DOPunchScale(transform.localScale * .3f, deathTweenTime, 5, 0.1f);
            yield return new WaitForSeconds(deathTweenTime / 2);

            InstantiateDeathFX();

            yield return new WaitForSeconds(deathTweenTime / 2);
        }
        else
        {
            yield return new WaitForSeconds(deathAnimDuration);
            InstantiateDeathFX();
        }

        if (spawnsUnitOnDeath == true) Instantiate(spawningUnit, transform.position, Quaternion.identity);
        if (dealsAoeDmgOnDeath == true)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, deathDmgRadius, deathDmgLayerMask);
            if (colliders != null) foreach (Collider col in colliders) col.gameObject.GetComponent<ALL_Health>().UpdateHealth(-deathDmgAmount);
        }
        Destroy(gameObject);
    }
    void InstantiateDeathFX()
    {
        if (deathFXType == DeathFXType.GobboUnitDeath) Instantiate(gamemanager.assetBank.FindFX(AssetBank.FXType.GobboUnitDeath), transform.position, Quaternion.identity);
        else if (deathFXType == DeathFXType.EnemyDeath) Instantiate(gamemanager.assetBank.FindFX(AssetBank.FXType.BasicEnemyDeath), transform.position, Quaternion.identity);
        else if (deathFXType == DeathFXType.TrollDeath) Instantiate(gamemanager.assetBank.FindFX(AssetBank.FXType.TrollUnitDeath), transform.position, Quaternion.identity);
        else if (deathFXType == DeathFXType.WanderingDeath) Instantiate(gamemanager.assetBank.FindFX(AssetBank.FXType.WanderingDeath), transform.position, Quaternion.identity);
    }
    void RemoveLayerAndTag(GameObject go)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = 0;
            trans.gameObject.tag = "Untagged";
        }
    }
}
