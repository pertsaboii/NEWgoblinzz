using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXdestroy : MonoBehaviour
{
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        Invoke("Destroy", ps.main.startLifetimeMultiplier + 0.5f);
    }
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
