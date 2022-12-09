using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseCon : MonoBehaviour
{
    void Start()
    {
        gamemanager.buildings.Add(gameObject);
        gamemanager.buildingsAndUnits.Add(gameObject);
    }
}
