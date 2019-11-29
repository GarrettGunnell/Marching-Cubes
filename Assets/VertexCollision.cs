using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexCollision : MonoBehaviour {


    private void Update() {
        if (Input.GetMouseButton(0)) {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo)) {
                if (hitInfo.collider.gameObject == gameObject) {
                    transform.parent.GetComponent<CubeGrid>().EditVertex(transform.TransformPoint(hitInfo.point));
                }
            }
        }
    }
}
