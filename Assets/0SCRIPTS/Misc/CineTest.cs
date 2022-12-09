using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CineTest : MonoBehaviour
{
    private Animator anim;
    private bool startCam = true;
    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("SwitchState", 2f);
    }
    void SwitchState()
    {
        if (startCam == true) anim.Play("MainCam");
        else anim.Play("StartCam");
        startCam = !startCam;
    }
}
