using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTagActivator : MonoBehaviour
{
    void Start()
    {
        gameObject.layer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy Tag Activator"))
        {
            gameObject.layer = 6;
        }
    }
}
