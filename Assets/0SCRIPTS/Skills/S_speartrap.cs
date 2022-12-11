using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_speartrap : MonoBehaviour
{
    private Animator anim;
    private BoxCollider bc;

    [SerializeField] private float damage;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip triggerSound;
    void Start()
    {
        anim = GetComponent<Animator>();
        bc = GetComponent<BoxCollider>();
        SoundManager.Instance.PlaySFXSound(spawnSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            SoundManager.Instance.PlaySFXSound(triggerSound);
            bc.enabled = false;
            anim.SetTrigger("Activate");
        }
    }
    public void TrapDamage()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 1.3f, layerMask);
        foreach  (Collider enemy in cols)
        {
            if (enemy.gameObject.GetComponent<ALL_Health>().isDead == false) enemy.gameObject.GetComponent<ALL_Health>().UpdateHealth(-damage);
        }
        
    }
    public void DestroyTrap()
    {
        Destroy(gameObject);
    }
}
