using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCards : MonoBehaviour
{
    [HideInInspector] public GameObject selectedCardUnit;
    [HideInInspector] public GameObject selectedCard;
    [HideInInspector] public float selectedCardCost;
    [HideInInspector] public string cardPlace;

    [SerializeField] private LayerMask layerMask;

    private Animator chiefAnim;

    private void Start()
    {
        chiefAnim = gamemanager.loseCon.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && selectedCard != null && IsPointerOverUI() == false)
        {
            if (gamemanager.userInterface.resourceSlider.value >= selectedCardCost)
            {
                Ray checkObjectsRay = gamemanager.screenInputCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(checkObjectsRay, out RaycastHit checkRaycastHit, layerMask) == true)
                    if (checkRaycastHit.collider.gameObject.layer == 11)
                    {
                        Ray ray = gamemanager.screenInputCamera.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out RaycastHit raycastHit))
                        {
                            if (raycastHit.point.x < gamemanager.loseCon.transform.position.x) chiefAnim.SetTrigger("Command2");
                            else chiefAnim.SetTrigger("Command1");

                            Instantiate(selectedCardUnit, new Vector3(raycastHit.point.x, 0, raycastHit.point.z + 1f), Quaternion.identity);
                            selectedCardUnit = null;
                            gamemanager.userInterface.resourceSlider.value -= selectedCardCost;

                            Destroy(selectedCard);
                            gamemanager.userInterface.anim.SetInteger("CardSelected", 0);

                            if (cardPlace == "1") gamemanager.userInterface.SpawnCardOne();
                            else if (cardPlace == "2") gamemanager.userInterface.SpawnCardTwo();
                            else if (cardPlace == "3") gamemanager.userInterface.SpawnCardThree();
                            else if (cardPlace == "4") gamemanager.userInterface.SpawnCardFour();
                        }
                    }
            }
        }
    }
    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
    public void ResetSelectedCard()
    {
        selectedCard = null;
        selectedCardCost = 0;
        selectedCardUnit = null;
    }
}
