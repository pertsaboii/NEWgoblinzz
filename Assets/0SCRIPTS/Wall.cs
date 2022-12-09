using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private bool lookAtMiddle;
    [SerializeField] private AudioClip spawnSound;
    void Start()
    {
        SoundManager.Instance.PlaySFXSound(spawnSound);
        gamemanager.buildings.Add(this.gameObject);
        if (lookAtMiddle == true) gameObject.transform.LookAt(gamemanager.loseCon.transform.position);
    }
}
