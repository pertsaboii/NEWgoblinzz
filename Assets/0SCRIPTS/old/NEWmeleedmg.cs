using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEWmeleedmg : MonoBehaviour
{
    public bool attackStateInitiated;
    public bool currentTargetDead;
    public bool targetInRange;
    private Animator anim;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int attackDamage;
    [HideInInspector] public GameObject target;
    ALL_Health targetHealth;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    public IEnumerator Attack()
    {
        if (target != null)
        {
            currentTargetDead = false; 
            targetHealth = target.GetComponent<ALL_Health>();
            anim.SetFloat("AttackSpeed", attackSpeed);

            while (target != null && targetInRange == true)
            {
                attackStateInitiated = true;
                anim.SetInteger("State", 3);
                anim.SetInteger("State", 2);

                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length * 0.5f);

                if (targetInRange == true && target != null) targetHealth.UpdateHealth(-attackDamage);

                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length * 0.5f);

                if (target == null)
                {
                    currentTargetDead = true;
                    attackStateInitiated = false;
                    targetInRange = false;
                    yield break;
                }
                else if (targetInRange == false)
                {
                    currentTargetDead = false;
                    attackStateInitiated = false;
                    yield break;
                }
            }
            if (target == null)
            {
                currentTargetDead = true;
                attackStateInitiated = false;
                targetInRange = false;
            }
        }
    }
}
