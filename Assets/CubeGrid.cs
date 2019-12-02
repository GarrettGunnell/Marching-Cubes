using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGrid : MonoBehaviour {

    public GameObject cubePrefab;
    public Transform stencil;
    public int resolution = 8;
    public float size = 8;
    public bool isDemonstration;

    private Mesh mesh;
    private MeshCollider meshc;
    private List<int> triangles;
    private List<Vector3> vertices;
    private Vertex[, ,] cubeVertices;
    private Material[, ,] vertexMaterials;
    private static string[] radiusNames = {"0", "1", "2", "3", "4", "5"};
    private int radiusIndex = 3;
    private float voxelSize, halfSize;
    private int triIndex;

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
        meshc = gameObject.AddComponent<MeshCollider>();
        mesh.name = "Marching Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        Refresh();
    }

    private void Update() {
		RaycastHit hitInfo;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo) && hitInfo.collider.gameObject == gameObject) {
            stencil.localScale = Vector3.one * radiusIndex;
            stencil.localPosition = transform.TransformPoint(hitInfo.point);
			stencil.gameObject.SetActive(true);
            if (Input.GetMouseButtonDown(0)) {
                Vector3 p = transform.TransformPoint(hitInfo.point);
                Debug.Log((int)(p.x / voxelSize) + ", " + (int)(p.y / voxelSize) + ", " + (int)(p.z / voxelSize));
                EditVertices(p, -1);
            } else if (Input.GetMouseButtonDown(1)) {
                Vector3 p = transform.TransformPoint(hitInfo.point);
                Debug.Log((int)(p.x / voxelSize) + ", " + (int)(p.y / voxelSize) + ", " + (int)(p.z / voxelSize));
                EditVertices(p, 0);
            }
		}
		else {
			stencil.gameObject.SetActive(false);
		}
    }

    private void OnGUI() {
        GUILayout.BeginArea(new Rect(4f, 4f, 150f, 500f));
        GUILayout.Label("Radius");
        radiusIndex = GUILayout.SelectionGrid(radiusIndex, radiusNames, 6);
        GUILayout.EndArea();
    }

    public void EditVertices(Vector3 point, int value) {
        for (int x = 0; x < resolution; ++x) {
            for (int y = 0; y < resolution; ++y) {
                for (int z = 0; z < resolution; ++z) {
                    Vector3 p = new Vector3(cubeVertices[x, y, z].x - point.x, cubeVertices[x, y, z].y - point.y, cubeVertices[x, y, z].z - point.z);
                    if (p.x * p.x + p.y * p.y + p.z * p.z <= radiusIndex * radiusIndex) {
                        cubeVertices[x, y, z].SetValue(value);
                    }
                }
            }
        }
        Refresh();
    }

    private void Refresh() {
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();
        triIndex = 0;
        for (int x = 0; x < resolution - 1; ++x) {
            for (int y = 0; y < resolution - 1; ++y) {
                for (int z = 0; z < resolution - 1; ++z) {
                    Triangulate(x, y, z);
                }
            }
        }
    }

    public void EditVertex(Vector3 point, bool state) {
        int vertexX = (int)(point.x / voxelSize);
        int vertexY = (int)(point.y / voxelSize);
        int vertexZ = (int)(point.z / voxelSize);
        Vertex vertex = cubeVertices[vertexX, vertexY, vertexZ];
        if (state) {
            vertex.SetValue(-1);
        } else {
            vertex.SetValue(0);
        }
        Refresh();
        //Debug.Log(vertexX + ", " + vertexY + ", " + vertexZ);
    }

    private void Triangulate(int x, int y, int z) {
        Vertex[] cube = FindCube(x, y, z);
        int index = DetermineTriangleIndex(cube);
        //Debug.Log(index);
        for (int i = 15; i >= 0; --i) {
            int edge = Tables.triangulation[index, i];

            if (edge == -1) {
                continue;
            }

            Vector3 point = new Vector3(0, 0, 0);
            switch (edge) {
    		case 0:
                point = new Vector3(cube[0].x, cube[0].y, cube[1].z - halfSize);
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
        meshc.sharedMesh = mesh;
        mesh.RecalculateNormals();
    }

    private int DetermineTriangleIndex(Vertex[] cube) {
        int index = 0;
        if (cube[0].GetValue() < 0) index |= 1;
        if (cube[1].GetValue() < 0) index |= 2;
        if (cube[2].GetValue() < 0) index |= 4;
        if (cube[3].GetValue() < 0) index |= 8;
        if (cube[4].GetValue() < 0) index |= 16;
        if (cube[5].GetValue() < 0) index |= 32;
        if (cube[6].GetValue() < 0) index |= 64;
        if (cube[7].GetValue() < 0) index |= 128;

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

        if (isDemonstration) {
            for (int i = 0; i < 8; ++i) {
                int vertexValue = cube[i].GetValue();

                if (vertexValue < 0) {
                    cube[i].SetColor(Color.white);
                } else {
                    cube[i].SetColor(Color.black);
                }
            }
        }

        return cube;
    }

    private void CreateVertex(int x, int y, int z) {
        if (isDemonstration) {
            GameObject o = Instantiate(cubePrefab) as GameObject;
            SphereCollider ob = o.AddComponent<SphereCollider>();
            o.transform.parent = transform;
            o.transform.localPosition = new Vector3(x * voxelSize, y * voxelSize, z * voxelSize);
            o.transform.localScale = Vector3.one * voxelSize * 0.1f;
            if (y == 0) {
                cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, -1, o);
            } else {
                cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, 0, o);
            }
        } else {
            if (y == 0) {
                cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, -1);
            } else {
                cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, 0);
            }
        }
    }

}
