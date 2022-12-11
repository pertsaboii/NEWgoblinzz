using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private bool spawnDoScalePop = false;
    void Start()
    {
        SoundManager.Instance.PlaySFXSound(spawnSound);
        gamemanager.buildingsAndUnits.Add(gameObject);

        // spawn doscale pop
        if (spawnDoScalePop == true)
        {
            transform.localScale = Vector3.zero;
            Vector3 originalScale = transform.localScale;
            transform.DOScale(originalScale, .1f);
        }
    }
}
