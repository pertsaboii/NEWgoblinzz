using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoDrawer : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private Transform gizmoOrigin;
    [SerializeField] private bool drawGizmo;

    private void OnDrawGizmos()     // aoe radius debug
    {
        Gizmos.color = Color.yellow;
        if (gameObject.name == "U_tankgobbo") Gizmos.DrawWireSphere(gizmoOrigin.position, radius);
        else if (drawGizmo == true) Gizmos.DrawWireSphere(new Vector3(transform.position.x, 0, transform.position.z), radius);
    }
}
