using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class S_shroomtrap : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;

    [SerializeField] private float damage;
    [SerializeField] private float initialDamage;
    [SerializeField] private float damageInterval;
    [SerializeField] private int damageCycles;
    [SerializeField] private float cloudRadius;
    private Collider col;
    [SerializeField] MeshRenderer mesh;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private AudioClip spawnSound;

    private void Start()
    {
        col = GetComponent<Collider>();
        SoundManager.Instance.PlaySFXSound(spawnSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            col.enabled = false;
            StartCoroutine("DamageCloud");
            other.gameObject.GetComponent<ALL_Health>().UpdateHealth(-initialDamage);
        }
    }
    IEnumerator DamageCloud()
    {
        Instantiate(ps, transform.position, Quaternion.identity);
        Invoke("DisableMesh", .15f);
        transform.DOPunchScale(Vector3.one * 0.4f, .15f, 5, 0.1f);

        for (int i = 0; i < damageCycles; i++)
        {
            yield return new WaitForSeconds(damageInterval);

            Collider[] enemies = Physics.OverlapSphere(transform.position, cloudRadius, layerMask);
            if (enemies.Length != 0)
            {
                foreach (Collider enemy in enemies)
                {
                    enemy.gameObject.GetComponent<ALL_Health>().UpdateHealth(-damage);
                }
            }
        }
        Destroy(gameObject);
    }
    void DisableMesh()
    {
        mesh.enabled = false;
    }
}
