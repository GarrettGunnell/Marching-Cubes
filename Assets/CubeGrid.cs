using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
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

    public void Initialize(int resolution, float size) {
        this.resolution = resolution;
        this.size = size;
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
        stencil.gameObject.SetActive(false);
        meshc = gameObject.AddComponent<MeshCollider>();
        mesh.name = "Marching Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        Refresh();
    }
    /*
    private void Update() {
		RaycastHit hitInfo;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo) && hitInfo.collider.gameObject == gameObject) {
            stencil.localScale = Vector3.one * radiusIndex;
            stencil.localPosition = transform.TransformPoint(hitInfo.point);
			stencil.gameObject.SetActive(true);
            if (Input.GetMouseButtonDown(0)) {
                Vector3 p = transform.TransformPoint(hitInfo.point);
                UpdateMesh(p, -1);
            } else if (Input.GetMouseButtonDown(1)) {
                Vector3 p = transform.TransformPoint(hitInfo.point);
                UpdateMesh(p, 0);
            }
		}
		else {
			stencil.gameObject.SetActive(false);
		}
    }
    */
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

    private void UpdateMesh(Vector3 point, int value) {
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();
        triIndex = 0;
        for (int x = 0; x < resolution - 1; ++x) {
            for (int y = 0; y < resolution - 1; ++y) {
                for (int z = 0; z < resolution - 1; ++z) {
                    UpdateCubeValues(x, y, z, point, value);
                    Triangulate(x, y, z);
                }
            }
        }
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

    private void UpdateCubeValues(int x, int y, int z, Vector3 point, int value) {
        Vertex[] cube = FindCube(x, y, z);

        for (int i = 0; i < 8; ++i) {
            Vector3 p = new Vector3(cube[i].x - point.x, cube[i].y - point.y, cube[i].z - point.z);
            if (p.x * p.x + p.y * p.y + p.z * p.z <= radiusIndex * radiusIndex) {
                cube[i].SetValue(value);
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
        for (int i = 0; Tables.triangulation[index, i] != -1; i += 3) {
            int edge = Tables.triangulation[index, i];

            int a0 = Tables.cornerIndexAFromEdge[Tables.triangulation[index, i]];
            int b0 = Tables.cornerIndexBFromEdge[Tables.triangulation[index, i]];
            int a1 = Tables.cornerIndexAFromEdge[Tables.triangulation[index, i + 1]];
            int b1 = Tables.cornerIndexBFromEdge[Tables.triangulation[index, i + 1]];
            int a2 = Tables.cornerIndexAFromEdge[Tables.triangulation[index, i + 2]];
            int b2 = Tables.cornerIndexBFromEdge[Tables.triangulation[index, i + 2]];

            Vector3 pointA = interpolateVertices(cube[a0], cube[b0]);
            Vector3 pointB = interpolateVertices(cube[a1], cube[b1]);
            Vector3 pointC = interpolateVertices(cube[a2], cube[b2]);


            vertices.Add(pointA);
            vertices.Add(pointB);
            vertices.Add(pointC);
            triangles.Add(triIndex);
            triangles.Add(triIndex + 1);
            triangles.Add(triIndex + 2);
            triIndex += 3;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        meshc.sharedMesh = mesh;
        mesh.RecalculateNormals();
    }

    private Vector3 interpolateVertices(Vertex a, Vertex b) {
        Vector3 point = new Vector3(
            a.x + ((b.x - a.x) / 2),
            a.y + ((b.y - a.y) / 2),
            a.z + ((b.z - a.z) / 2)
        );

        return point;
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
        cube[1] = cubeVertices[x + 1, y, z];
        cube[2] = cubeVertices[x + 1, y, z + 1];
        cube[3] = cubeVertices[x, y, z + 1];
        cube[4] = cubeVertices[x, y + 1, z];
        cube[5] = cubeVertices[x + 1, y + 1, z];
        cube[6] = cubeVertices[x + 1, y + 1, z + 1];
        cube[7] = cubeVertices[x, y + 1, z + 1];

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
                cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, 0, o);
            } else {
                cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, 0, o);
            }
        } else {
            if (y == 0) {
                cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, 0);
            } else {
                cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, 0);
            }
        }
    }

}
