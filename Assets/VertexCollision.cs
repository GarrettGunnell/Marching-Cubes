using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexCollision : MonoBehaviour {


    private void Update() {
        Transform visualization = transform.parent.GetComponent<CubeGrid>().stencil;
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo) && hitInfo.collider.gameObject == gameObject) {
            if (Input.GetMouseButton(0)) {
                Debug.Log(transform.TransformPoint(hitInfo.point));
                if (Input.GetKey(KeyCode.LeftControl)) {
                    transform.parent.parent.GetComponent<CubeMap>().EditVertex(transform.TransformPoint(hitInfo.point), 0);
                } else {
                    transform.parent.parent.GetComponent<CubeMap>().EditVertex(transform.TransformPoint(hitInfo.point), -1);
                }
            }
        }
    }
}
