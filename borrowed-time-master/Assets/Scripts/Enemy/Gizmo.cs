using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo : MonoBehaviour
{
    public float size = 0.5f;

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(transform.position, size);
    }

}
