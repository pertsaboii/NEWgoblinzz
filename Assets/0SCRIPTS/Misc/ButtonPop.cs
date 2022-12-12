using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonPop : MonoBehaviour
{
    private Button button;
    private Vector3 originalScale;
    [SerializeField] private float popScale = .35f;
    [SerializeField] private float popTime = .3f;
    [SerializeField] private int popVibrato = 8;
    [SerializeField] private float popElasticity = 1f;

    void Start()
    {
        originalScale = transform.localScale;
        button = GetComponent<Button>();
        button.onClick.AddListener(StartPopButton);
    }

    void StartPopButton()
    {
        StartCoroutine(PopButton());
    }
    IEnumerator PopButton()
    {
        transform.DOPunchScale(Vector3.one * popScale, popTime, popVibrato, popElasticity).SetUpdate(true);
        yield return new WaitForSecondsRealtime(popTime);
        if (transform.localScale != originalScale) transform.localScale = originalScale;
    }
}
