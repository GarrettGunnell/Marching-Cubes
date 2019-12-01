using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGrid : MonoBehaviour {

    public GameObject cubePrefab;
    public int resolution = 8;
    public float size = 8;

    private Mesh mesh;
    private List<int> triangles;
    private List<Vector3> vertices;

    private Vertex[, ,] cubeVertices;
    private Material[, ,] vertexMaterials;
    private float voxelSize, halfSize;

    private void Awake() {
        cubeVertices = new Vertex[resolution, resolution, resolution];
        vertexMaterials = new Material[resolution, resolution, resolution];
        voxelSize = size / resolution;
        halfSize = voxelSize / 2;

        for (int x = 0; x < resolution; ++x) {
            for (int y = 0; y < resolution; ++y) {
                for (int z = 0; z < resolution; ++z) {
                    //Debug.Log(x + ", " + y + ", " + z);
                    CreateVertex(x, y, z);
                }
            }
        }
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Marching Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        SetVertexColors();
    }

    public void EditVertex(Vector3 point, bool state) {
        int vertexX = (int)(point.x / voxelSize);
        int vertexY = (int)(point.y / voxelSize);
        int vertexZ = (int)(point.z / voxelSize);
        Vertex vertex = cubeVertices[vertexX, vertexY, vertexZ];
        if (state) {
            vertex.SetValue(1);
        } else {
            vertex.SetValue(0);
        }
        SetVertexColors();
        Triangulate();
        Debug.Log(vertexX + ", " + vertexY + ", " + vertexZ);
    }

    private void Triangulate() {
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();

        Vertex[] cube = FindCube(0, 0, 0);
        int triIndex = 0;
        int index = DetermineTriangleIndex(cube);
        for (int i = 0; i < 16; ++i) {
            int edge = Tables.triangulation[index, i];

            if (edge == -1) {
                continue;
            }

            Vector3 point = new Vector3(0, 0, 0);
            switch (edge) {
    		case 0:
                point = new Vector3(cube[0].x, cube[0].y, cube[1].z / 2);
    			break;
    		case 1:
                point = new Vector3(cube[2].x - halfSize, cube[1].y, cube[1].z);
    			break;
    		case 2:
                point = new Vector3(cube[2].x, cube[2].y, cube[2].z - halfSize);
    			break;
    		case 3:
                point = new Vector3(cube[3].x - halfSize, cube[3].y, cube[3].z);
    			break;
    		case 4:
                point = new Vector3(cube[4].x, cube[4].y, cube[5].z - halfSize);
    			break;
    		case 5:
                point = new Vector3(cube[6].x - halfSize, cube[5].y, cube[5].z);
    			break;
    		case 6:
                point = new Vector3(cube[7].x, cube[7].y, cube[6].z - halfSize);
    			break;
    		case 7:
                point = new Vector3(cube[7].x - halfSize, cube[7].y, cube[7].z);
    			break;
    		case 8:
                point = new Vector3(cube[0].x, cube[4].y - halfSize, cube[0].z);
    			break;
    		case 9:
                point = new Vector3(cube[1].x, cube[5].y - halfSize, cube[1].z);
    			break;
    		case 10:
                point = new Vector3(cube[2].x, cube[6].y - halfSize, cube[2].z);
    			break;
    		case 11:
                point = new Vector3(cube[3].x, cube[7].y - halfSize, cube[3].z);
    			break;
    		}

            vertices.Add(point);
            triangles.Add(triIndex);
            ++triIndex;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
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

        cube[0] = cubeVertices[x, y, z];
        cube[1] = cubeVertices[x, y, z + 1];
        cube[2] = cubeVertices[x + 1, y, z + 1];
        cube[3] = cubeVertices[x + 1, y, z];
        cube[4] = cubeVertices[x, y + 1, z];
        cube[5] = cubeVertices[x, y + 1, z + 1];
        cube[6] = cubeVertices[x + 1, y + 1, z + 1];
        cube[7] = cubeVertices[x + 1, y + 1, z];

        return cube;
    }

    private void CreateVertex(int x, int y, int z) {
        cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, 0);
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
                    int vertexValue = cubeVertices[x, y, z].GetValue();

                    if (vertexValue > 0) {
                        vertexMaterials[x, y, z].color = Color.white;
                    } else {
                        vertexMaterials[x, y, z].color = Color.black;
                    }
                }
            }
        }
    }
}
