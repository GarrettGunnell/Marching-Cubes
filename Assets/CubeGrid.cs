using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGrid : MonoBehaviour {

    public GameObject cubePrefab;
    public int resolution = 8;
    public float size = 8;

    private Vertex[, ,] vertices;
    private Material[, ,] vertexMaterials;
    private float voxelSize, halfSize;

    private void Awake() {
        vertices = new Vertex[resolution, resolution, resolution];
        vertexMaterials = new Material[resolution, resolution, resolution];
        voxelSize = size / resolution;
        halfSize = size / 2;

        for (int x = 0; x < resolution; ++x) {
            for (int y = 0; y < resolution; ++y) {
                for (int z = 0; z < resolution; ++z) {
                    //Debug.Log(x + ", " + y + ", " + z);
                    CreateVertex(x, y, z);
                }
            }
        }
        SetVertexColors();
    }

    public void EditVertex(Vector3 point, bool state) {
        int vertexX = (int)(point.x / voxelSize);
        int vertexY = (int)(point.y / voxelSize);
        int vertexZ = (int)(point.z / voxelSize);
        Vertex vertex = vertices[vertexX, vertexY, vertexZ];
        if (state) {
            vertex.SetValue(1);
        } else {
            vertex.SetValue(0);
        }
        SetVertexColors();
        Vertex[] cube = FindCube(0, 0, 0);
        int index = DetermineTriangleIndex(cube);
        Debug.Log(index);
        Debug.Log(vertexX + ", " + vertexY + ", " + vertexZ);
    }

    private void Triangulate() {

    }

    private int DetermineTriangleIndex(Vertex[] cube) {
        int index = 0;
        if (cube[0].GetValue() > 0) index |= 1;
        if (cube[1].GetValue() > 0) index |= 2;
        if (cube[2].GetValue() > 0) index |= 4;
        if (cube[3].GetValue() > 0) index |= 8;
        if (cube[4].GetValue() > 0) index |= 16;
        if (cube[5].GetValue() > 0) index |= 32;
        if (cube[6].GetValue() > 0) index |= 64;
        if (cube[7].GetValue() > 0) index |= 128;

        return index;
    }

    private Vertex[] FindCube(int x, int y, int z) {
        Vertex[] cube = new Vertex[8];

        cube[0] = vertices[x, y, z];
        cube[1] = vertices[x, y, z + 1];
        cube[2] = vertices[x + 1, y, z + 1];
        cube[3] = vertices[x + 1, y, z];
        cube[4] = vertices[x, y + 1, z];
        cube[5] = vertices[x, y + 1, z + 1];
        cube[6] = vertices[x + 1, y + 1, z + 1];
        cube[7] = vertices[x + 1, y + 1, z];

        return cube;
    }

    private void CreateVertex(int x, int y, int z) {
        vertices[x, y, z] = new Vertex(0);
        GameObject o = Instantiate(cubePrefab) as GameObject;
        BoxCollider ob = o.AddComponent<BoxCollider>();
        o.transform.parent = transform;
        o.transform.localPosition = new Vector3(x * voxelSize, y * voxelSize, z * voxelSize);
        o.transform.localScale = Vector3.one * voxelSize * 0.1f;
        vertexMaterials[x, y, z] = o.GetComponent<MeshRenderer>().material;
    }

    private void SetVertexColors() {
        for (int x = 0; x < resolution; ++x) {
            for (int y = 0; y < resolution; ++y) {
                for (int z = 0; z < resolution; ++z) {
                    int vertexValue = vertices[x, y, z].GetValue();

                    if (vertexValue > 0) {
                        vertexMaterials[x, y, z].color = Color.black;
                    } else {
                        vertexMaterials[x, y, z].color = Color.white;
                    }
                }
            }
        }
    }
}
