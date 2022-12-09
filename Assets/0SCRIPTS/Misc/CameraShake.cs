using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    [SerializeField] private float shakeTime;
    void Start()
    {
        originalPos = transform.position;
    }

    private IEnumerator ShakeCamera()
    {
        gamemanager.mainCineCam.transform.DOShakePosition(shakeTime, .3f, 10, 90, false, true, ShakeRandomnessMode.Full);
        yield return new WaitForSeconds(shakeTime);
        if (gamemanager.mainCineCam.transform.position != originalPos)
        {
            gamemanager.mainCineCam.transform.DOMove(originalPos, 0.2f, false).SetEase(Ease.OutSine);
        }
    }
    public void StartShakeCamera()
    {
        StartCoroutine(ShakeCamera());
    }
}
