using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObstacleAgent))]
public class NMtestmove : MonoBehaviour
{
    private ObstacleAgent agent;

    [SerializeField] private bool isActive;
    [SerializeField] private GameObject destinationObject;

    private Vector3 lookAtPos;
    void Awake()
    {
        agent = GetComponent<ObstacleAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && isActive)
        {
            Ray moveposition = gamemanager.screenInputCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(moveposition, out var hitInfo))
            {
                agent.SetDestination(hitInfo.point);
                lookAtPos = hitInfo.point;
            }
        }
        else if (destinationObject != null && isActive == false && Vector3.Distance(destinationObject.transform.position, transform.position) >= gameObject.GetComponent<NavMeshAgent>().stoppingDistance)
        {
            agent.SetDestination(destinationObject.transform.position);
            lookAtPos = destinationObject.transform.position;
        }
        if (agent.agent.enabled == false) transform.LookAt(new Vector3(lookAtPos.x, transform.position.y, lookAtPos.z));
    }
}
