using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private AudioClip spawnSound;
    void Start()
    {
        SoundManager.Instance.PlaySFXSound(spawnSound);
        gamemanager.buildingsAndUnits.Add(gameObject);
    }
}
