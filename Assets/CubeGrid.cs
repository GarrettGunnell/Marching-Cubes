using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGrid : MonoBehaviour {

    public GameObject cubePrefab;
    public int resolution = 8;
    public float size = 8;

    private bool[, ,] vertices;
    private float voxelSize, halfSize;

    private void Awake() {
        vertices = new bool[resolution, resolution, resolution];
        voxelSize = size / resolution;
        halfSize = size / 2;

        for (int x = 0; x < resolution; ++x) {
            for (int y = 0; y < resolution; ++y) {
                for (int z = 0; z < resolution; ++z) {
                    GameObject o = Instantiate(cubePrefab) as GameObject;
                    BoxCollider ob = o.AddComponent<BoxCollider>();
                    o.transform.parent = transform;
                    o.transform.localPosition = new Vector3(x * voxelSize, y * voxelSize, z * voxelSize);
                    o.transform.localScale = Vector3.one * voxelSize * 0.1f;
                }
            }
        }
    }

    public void EditVertex(Vector3 point) {
        int vertexX = (int)(point.x / voxelSize);
        int vertexY = (int)(point.y / voxelSize);
        int vertexZ = (int)(point.z / voxelSize);
        Debug.Log(vertexX + ", " + vertexY + ", " + vertexZ);
    }
}
