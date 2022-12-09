using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemymeleedmg : MonoBehaviour
{
    [SerializeField] private float dmgAmount;
    private testenemy targetFindingScript;
    private ALL_Health enemyHealth;
    private bool targetFound;
    [HideInInspector] public bool dealDmg;
    private bool dmgDealed;
    void Start()
    {
        targetFindingScript = GetComponent<testenemy>();
    }

    void Update()
    {
        if (targetFindingScript.target != null && targetFound == false)
        {
            enemyHealth = targetFindingScript.target.GetComponent<ALL_Health>();
            targetFound = true;
        }
        else targetFound = false;

        if (targetFindingScript.target != null && dealDmg == true && dmgDealed == false) StartCoroutine("DealDmg");
    }
    IEnumerator DealDmg()
    {
        dmgDealed = true;
        enemyHealth.UpdateHealth(-dmgAmount);

        yield return new WaitForSeconds(.05f);

        dmgDealed = false;
    }
}
