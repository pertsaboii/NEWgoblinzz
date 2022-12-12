using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class SpawnDoScale : MonoBehaviour
{
    [SerializeField] private bool isBuilding;
    private Vector3 originalScale;
    void Start()
    {
        originalScale = transform.localScale;
        if (isBuilding == false) NotBuildingScale();
        else StartCoroutine(BuildingScale());
    }

    void NotBuildingScale()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(originalScale, .15f);
    }
    IEnumerator BuildingScale()
    {
        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
        obstacle.enabled = false;
        transform.localScale = Vector3.zero;
        transform.DOScale(originalScale, .15f);
        yield return new WaitForSeconds(.15f);
        obstacle.enabled = true;
    }
}
