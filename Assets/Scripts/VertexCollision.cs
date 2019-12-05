using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexCollision : MonoBehaviour {


    private void Update() {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo) && hitInfo.collider.gameObject == gameObject) {
            if (Input.GetMouseButton(0)) {
                Debug.Log(transform.TransformPoint(hitInfo.point));
                transform.parent.parent.GetComponent<CubeMap>().EditVertex(transform.TransformPoint(hitInfo.point), 1);
            } else if (Input.GetMouseButton(1)) {
                transform.parent.parent.GetComponent<CubeMap>().EditVertex(transform.TransformPoint(hitInfo.point), 0);
            }
        }
    }
}
