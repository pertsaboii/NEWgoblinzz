using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CostText : MonoBehaviour
{
    private TMP_Text costText;
    void Start()
    {
        costText = GetComponent<TMP_Text>();
        costText.text = transform.parent.parent.GetComponent<Card>().cost.ToString();
    }
}
