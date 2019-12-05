using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class CubeGrid : MonoBehaviour {

    public GameObject cubePrefab;
    public int resolution;
    public float size;
    public bool isDemonstration;
    public CubeGrid xNeighbor, yNeighbor, zNeighbor, xyNeighbor, xzNeighbor, zyNeighbor, xyzNeighbor;
    public float isoLevel;

    private Mesh mesh;
    private List<int> triangles;
    private List<Vector3> vertices;
    public Vertex[, ,] cubeVertices;
    private Material[, ,] vertexMaterials;
    private Vertex dummy1, dummy2, dummy3, dummy4, dummy5, dummy6, dummy7;
    private float voxelSize, halfSize;
    private int triIndex;
    private float[,] heightMap;
    private bool interpolation;

    public void Initialize(int resolution, float size, float isoLevel, bool interpolation, int chunkX, int chunkY, int chunkZ, float[,] heightMap, float noiseWeight) {
        this.resolution = resolution;
        this.interpolation = interpolation;
        this.size = size;
        this.isoLevel = isoLevel;
        this.heightMap = heightMap;
        cubeVertices = new Vertex[resolution, resolution, resolution];
        vertexMaterials = new Material[resolution, resolution, resolution];
        voxelSize = size / resolution;
        halfSize = voxelSize / 2;

        for (int x = 0; x < resolution; ++x) {
            for (int y = 0; y < resolution; ++y) {
                for (int z = 0; z < resolution; ++z) {
                    //Debug.Log(x + ", " + y + ", " + z);
                    CreateVertex(x, y, z, chunkX, chunkY, chunkZ, noiseWeight);
                }
            }
        }
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Marching Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();

        dummy1 = new Vertex();
        dummy2 = new Vertex();
        dummy3 = new Vertex();
        dummy4 = new Vertex();
        dummy5 = new Vertex();
        dummy6 = new Vertex();
        dummy7 = new Vertex();
        //Refresh();
    }

    public void updateValues(float isoLevel, float noiseWeight, bool interpolation, bool regenerate) {
        this.isoLevel = isoLevel;
        this.interpolation = interpolation;
        if (regenerate) {
            heightMap = transform.parent.GetComponent<CubeMap>().heightMap;
            for (int x = 0; x < resolution; ++x) {
                for (int y = 0; y < resolution; ++y) {
                    for (int z = 0; z < resolution; ++z) {
                        cubeVertices[x, y, z].SetValue(heightMap, noiseWeight);
                    }
                }
            }
        }
    }

    public void Refresh() {
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();
        triIndex = 0;
        for (int x = 0; x < resolution - 1; ++x) {
            for (int y = 0; y < resolution - 1; ++y) {
                for (int z = 0; z < resolution - 1; ++z) {
                    Vertex[] cube = FindCube(x, y, z);
                    Triangulate(cube);
                }
            }
        }

        if (xNeighbor != null) {
            TriangulateAcrossX();
        }
        if (zNeighbor != null) {
            TriangulateAcrossZ();
        }
        if (yNeighbor != null) {
            TriangulateAcrossY();
        }
        if (zyNeighbor != null) {
            TriangulateAcrossZY();
        }
        if (xyNeighbor != null) {
            TriangulateAcrossXY();
        }
        if (xzNeighbor != null) {
            TriangulateAcrossXZ();
        }
        if (xyzNeighbor != null) {
            TriangulateAcrossXYZ();
        }
    }

    public void TriangulateAcrossX() {
        for (int y = 0; y < resolution - 1; ++y) {
            for (int z = 0; z < resolution - 1; ++z) {
                Vertex[] cube = new Vertex[8];
                dummy1.BecomeXDummyOf(xNeighbor.cubeVertices[0, y, z], size);
                dummy2.BecomeXDummyOf(xNeighbor.cubeVertices[0, y, z + 1], size);
                dummy3.BecomeXDummyOf(xNeighbor.cubeVertices[0, y + 1, z], size);
                dummy4.BecomeXDummyOf(xNeighbor.cubeVertices[0, y + 1, z + 1], size);

                cube[0] = cubeVertices[resolution - 1, y, z];
                cube[1] = dummy1;
                cube[2] = dummy2;
                cube[3] = cubeVertices[resolution - 1, y, z + 1];
                cube[4] = cubeVertices[resolution - 1, y + 1, z];
                cube[5] = dummy3;
                cube[6] = dummy4;
                cube[7] = cubeVertices[resolution - 1, y + 1, z + 1];

                Triangulate(cube);
            }
        }
    }

    public void TriangulateAcrossZ() {
        for (int x = 0; x < resolution - 1; ++x) {
            for (int y = 0; y < resolution - 1; ++y) {
                Vertex[] cube = new Vertex[8];
                dummy1.BecomeZDummyOf(zNeighbor.cubeVertices[x + 1, y, 0], size);
                dummy2.BecomeZDummyOf(zNeighbor.cubeVertices[x, y, 0], size);
                dummy3.BecomeZDummyOf(zNeighbor.cubeVertices[x + 1, y + 1, 0], size);
                dummy4.BecomeZDummyOf(zNeighbor.cubeVertices[x, y + 1, 0], size);

                cube[0] = cubeVertices[x, y, resolution - 1];
                cube[1] = cubeVertices[x + 1, y, resolution - 1];
                cube[2] = dummy1;
                cube[3] = dummy2;
                cube[4] = cubeVertices[x, y + 1, resolution - 1];
                cube[5] = cubeVertices[x + 1, y + 1, resolution - 1];
                cube[6] = dummy3;
                cube[7] = dummy4;

                Triangulate(cube);
            }
        }
    }

    public void TriangulateAcrossY() {
        for (int x = 0; x < resolution - 1; ++x) {
            for (int z = 0; z < resolution - 1; ++z) {
                Vertex[] cube = new Vertex[8];
                dummy1.BecomeYDummyOf(yNeighbor.cubeVertices[x, 0, z], size);
                dummy2.BecomeYDummyOf(yNeighbor.cubeVertices[x + 1, 0, z], size);
                dummy3.BecomeYDummyOf(yNeighbor.cubeVertices[x + 1, 0, z + 1], size);
                dummy4.BecomeYDummyOf(yNeighbor.cubeVertices[x, 0, z + 1], size);

                cube[0] = cubeVertices[x, resolution - 1, z];
                cube[1] = cubeVertices[x + 1, resolution - 1, z];
                cube[2] = cubeVertices[x + 1, resolution - 1, z + 1];
                cube[3] = cubeVertices[x, resolution - 1, z + 1];
                cube[4] = dummy1;
                cube[5] = dummy2;
                cube[6] = dummy3;
                cube[7] = dummy4;

                Triangulate(cube);
            }
        }
    }

    public void TriangulateAcrossZY() {
        for (int x = 0; x < resolution - 1; ++x) {
            Vertex[] cube = new Vertex[8];
            dummy1.BecomeZDummyOf(zNeighbor.cubeVertices[x + 1, resolution - 1, 0], size);
            dummy2.BecomeZDummyOf(zNeighbor.cubeVertices[x, resolution - 1, 0], size);
            dummy3.BecomeYDummyOf(yNeighbor.cubeVertices[x, 0, resolution - 1], size);
            dummy4.BecomeYDummyOf(yNeighbor.cubeVertices[x + 1, 0, resolution - 1], size);
            dummy5.BecomeZYDummyOf(zyNeighbor.cubeVertices[x + 1, 0, 0], size);
            dummy6.BecomeZYDummyOf(zyNeighbor.cubeVertices[x, 0, 0], size);

            cube[0] = cubeVertices[x, resolution - 1, resolution - 1];
            cube[1] = cubeVertices[x + 1, resolution - 1, resolution - 1];
            cube[2] = dummy1;
            cube[3] = dummy2;
            cube[4] = dummy3;
            cube[5] = dummy4;
            cube[6] = dummy5;
            cube[7] = dummy6;

            Triangulate(cube);
        }
    }

    public void TriangulateAcrossXY() {
        for (int z = 0; z < resolution - 1; ++z) {
            Vertex[] cube = new Vertex[8];
            dummy1.BecomeXDummyOf(xNeighbor.cubeVertices[0, resolution - 1, z], size);
            dummy2.BecomeXDummyOf(xNeighbor.cubeVertices[0, resolution - 1, z + 1], size);
            dummy3.BecomeYDummyOf(yNeighbor.cubeVertices[resolution - 1, 0, z], size);
            dummy4.BecomeXYDummyOf(xyNeighbor.cubeVertices[0, 0, z], size);
            dummy5.BecomeXYDummyOf(xyNeighbor.cubeVertices[0, 0, z + 1], size);
            dummy6.BecomeYDummyOf(yNeighbor.cubeVertices[resolution - 1, 0, z + 1], size);

            cube[0] = cubeVertices[resolution - 1, resolution - 1, z];
            cube[1] = dummy1;
            cube[2] = dummy2;
            cube[3] = cubeVertices[resolution - 1, resolution - 1, z + 1];
            cube[4] = dummy3;
            cube[5] = dummy4;
            cube[6] = dummy5;
            cube[7] = dummy6;

            Triangulate(cube);
        }
    }

    public void TriangulateAcrossXZ() {
        for (int y = 0; y < resolution - 1; ++y) {
            Vertex[] cube = new Vertex[8];
            dummy1.BecomeXDummyOf(xNeighbor.cubeVertices[0, y, resolution - 1], size);
            dummy2.BecomeXZDummyOf(xzNeighbor.cubeVertices[0, y, 0], size);
            dummy3.BecomeZDummyOf(zNeighbor.cubeVertices[resolution - 1, y, 0], size);
            dummy4.BecomeXDummyOf(xNeighbor.cubeVertices[0, y + 1, resolution - 1], size);
            dummy5.BecomeXZDummyOf(xzNeighbor.cubeVertices[0, y + 1, 0], size);
            dummy6.BecomeZDummyOf(zNeighbor.cubeVertices[resolution - 1, y + 1, 0], size);

            cube[0] = cubeVertices[resolution - 1, y, resolution - 1];
            cube[1] = dummy1;
            cube[2] = dummy2;
            cube[3] = dummy3;
            cube[4] = cubeVertices[resolution - 1, y + 1, resolution - 1];
            cube[5] = dummy4;
            cube[6] = dummy5;
            cube[7] = dummy6;

            Triangulate(cube);
        }
    }

    public void TriangulateAcrossXYZ() {
        Vertex[] cube = new Vertex[8];
        dummy1.BecomeXDummyOf(xNeighbor.cubeVertices[0, resolution - 1, resolution - 1], size);
        dummy2.BecomeXZDummyOf(xzNeighbor.cubeVertices[0, resolution - 1, 0], size);
        dummy3.BecomeZDummyOf(zNeighbor.cubeVertices[resolution - 1, resolution - 1, 0], size);
        dummy4.BecomeYDummyOf(yNeighbor.cubeVertices[resolution - 1, 0, resolution - 1], size);
        dummy5.BecomeXYDummyOf(xyNeighbor.cubeVertices[0, 0, resolution - 1], size);
        dummy6.BecomeXYZDummyOf(xyzNeighbor.cubeVertices[0, 0, 0], size);
        dummy7.BecomeZYDummyOf(zyNeighbor.cubeVertices[resolution - 1, 0, 0], size);

        cube[0] = cubeVertices[resolution - 1, resolution - 1, resolution - 1];
        cube[1] = dummy1;
        cube[2] = dummy2;
        cube[3] = dummy3;
        cube[4] = dummy4;
        cube[5] = dummy5;
        cube[6] = dummy6;
        cube[7] = dummy7;

        Triangulate(cube);
    }

    public void EditVertex(Vector3 point, bool state) {
        int vertexX = (int)(point.x / voxelSize);
        int vertexY = (int)(point.y / voxelSize);
        int vertexZ = (int)(point.z / voxelSize);
        Vertex vertex = cubeVertices[vertexX, vertexY, vertexZ];
        if (state) {
            //vertex.SetValue(-1);
        } else {
            //vertex.SetValue(0);
        }
        Refresh();
        //Debug.Log(vertexX + ", " + vertexY + ", " + vertexZ);
    }

    private void Triangulate(Vertex[] cube) {
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
        mesh.RecalculateNormals();
    }

    private Vector3 interpolateVertices(Vertex a, Vertex b) {
        Vector3 point;
        if (interpolation) {
            float t = (isoLevel - a.GetValue()) / (b.GetValue() - a.GetValue());
             point = a.position + t * (b.position - a.position);
        } else {
             point = a.position + ((b.position - a.position) / 2);
        }

        return point;
    }

    private int DetermineTriangleIndex(Vertex[] cube) {
        int index = 0;
        if (cube[0].GetValue() > isoLevel) index |= 1;
        if (cube[1].GetValue() > isoLevel) index |= 2;
        if (cube[2].GetValue() > isoLevel) index |= 4;
        if (cube[3].GetValue() > isoLevel) index |= 8;
        if (cube[4].GetValue() > isoLevel) index |= 16;
        if (cube[5].GetValue() > isoLevel) index |= 32;
        if (cube[6].GetValue() > isoLevel) index |= 64;
        if (cube[7].GetValue() > isoLevel) index |= 128;
        return index;
    }

    private Vertex[] FindCube(int x, int y, int z) {
        Vertex[] cube = new Vertex[8];

        cube[0] = cubeVertices[x, y, z];
        //Debug.Log(cube[0].GetValue());
        cube[1] = cubeVertices[x + 1, y, z];
        cube[2] = cubeVertices[x + 1, y, z + 1];
        cube[3] = cubeVertices[x, y, z + 1];
        cube[4] = cubeVertices[x, y + 1, z];
        cube[5] = cubeVertices[x + 1, y + 1, z];
        cube[6] = cubeVertices[x + 1, y + 1, z + 1];
        cube[7] = cubeVertices[x, y + 1, z + 1];

        if (isDemonstration) {
            for (int i = 0; i < 8; ++i) {
                float vertexValue = cube[i].GetValue();

                if (vertexValue > isoLevel) {
                    cube[i].SetColor(Color.white);
                } else {
                    cube[i].SetColor(Color.black);
                }
            }
        }

        return cube;
    }

    private void CreateVertex(int x, int y, int z, int chunkX, int chunkY, int chunkZ, float noiseWeight) {
        if (isDemonstration) {
            GameObject o = Instantiate(cubePrefab) as GameObject;
            SphereCollider ob = o.AddComponent<SphereCollider>();
            o.transform.parent = transform;
            o.transform.localPosition = new Vector3(x * voxelSize, y * voxelSize, z * voxelSize);
            o.transform.localScale = Vector3.one * voxelSize * 0.1f;
            //Debug.Log(o.transform.position);
            cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, o);
        } else {
            cubeVertices[x, y, z] = new Vertex(x * voxelSize, y * voxelSize, z * voxelSize, chunkX, chunkY, chunkZ, size, voxelSize, resolution, heightMap, noiseWeight);
        }
    }

}
