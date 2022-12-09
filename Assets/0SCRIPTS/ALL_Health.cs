using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(ALL_Death))]
public class ALL_Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float currentHealth;

    [SerializeField] private Image hpSprite;
    [SerializeField] private GameObject hpBar;

    [HideInInspector] public bool isDead;
    private ALL_Death deathScript;

    [Header("Scale Pop When Damaged")]
    [SerializeField] private bool scalePopsWhenDamaged;
    private Vector3 originalScale;
    void Start()
    {
        originalScale = transform.localScale;
        deathScript = GetComponent<ALL_Death>();
        currentHealth = maxHealth;
        hpSprite.fillAmount = currentHealth / maxHealth;
        hpBar.SetActive(false);
    }
    public void UpdateHealth(float healthChange)
    {
        if (currentHealth <= maxHealth) hpBar.SetActive(true);
        currentHealth += healthChange;
        hpSprite.fillAmount = currentHealth / maxHealth;
        if (healthChange < 0 && scalePopsWhenDamaged == true) StartCoroutine("ScalePop");
        if (currentHealth <= 0 && isDead == false) ZeroHealthPoints();
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            hpBar.SetActive(false);
        }
    }
    private void Update()
    {
        //hpBar.transform.LookAt(new Vector3(1, gamemanager.camera.transform.position.y * 10, gamemanager.camera.transform.position.z * 10));
        //hpBar.transform.LookAt(new Vector3(transform.localPosition.x, gamemanager.camera.transform.position.y * 10, transform.localPosition.z));
        hpBar.transform.LookAt(new Vector3(transform.position.x - gamemanager.screenInputCamera.transform.position.x, gamemanager.screenInputCamera.transform.position.y * 10, transform.position.z - gamemanager.screenInputCamera.transform.position.z));
        //hpBar.transform.LookAt(new Vector3(transform.position.x - gamemanager.camera.transform.position.x, gamemanager.camera.transform.position.y * 10, gamemanager.camera.transform.position.z - transform.position.z));
    }
    void ZeroHealthPoints()
    {
        isDead = true;
        if (gameObject.layer == 7 || gameObject.layer == 8) gamemanager.buildingsAndUnits.Remove(gameObject);
        if (gameObject.CompareTag("Enemy") == true) gamemanager.enemies.Remove(gameObject);
        if (gameObject.CompareTag("Building") == true) gamemanager.buildings.Remove(gameObject);
        hpBar.SetActive(false);
        StartCoroutine(deathScript.Death());
    }
    IEnumerator ScalePop()
    {
        gameObject.transform.DOPunchScale(transform.localScale * .1f, .15f, 5, 0.1f);
        yield return new WaitForSeconds(.15f);
        if (transform.localScale != originalScale)
        {
            transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutSine);
        }
    }
}
