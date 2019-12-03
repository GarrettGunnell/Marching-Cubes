using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMap : MonoBehaviour {

    public float size = 2f;

    public int resolution = 8;
    public int chunkResolution = 2;

    public CubeGrid cubeGridPrefab;

    public bool refresh = false;
    public bool interpolation = false;
    public bool regenerate = false;
    private bool oldRefresh;
    public float isoLevel = 1;
    public float noiseWeight = 5;

    public float[,] heightMap;

    private CubeGrid[, ,] chunks;
    private HeightMapGenerator heightMapGenerator;
    private float chunkSize, cubeSize, halfSize;

    private void Awake() {
        halfSize = size * 0.5f;
        chunkSize = size / chunkResolution;
        cubeSize = chunkSize / resolution;
        oldRefresh = refresh;
        heightMapGenerator = gameObject.GetComponent<HeightMapGenerator>();
        heightMap = heightMapGenerator.Generate(resolution, chunkResolution);

        chunks = new CubeGrid[chunkResolution, chunkResolution, chunkResolution];
        for (int x = 0; x < chunkResolution; ++x) {
            for (int y = 0; y < chunkResolution; ++y) {
                for (int z = 0; z < chunkResolution; ++z) {
                    CreateChunk(x, y, z);
                }
            }
        }
        for (int x = 0; x < chunkResolution; ++x) {
            for (int y = 0; y < chunkResolution; ++y) {
                for (int z = 0; z < chunkResolution; ++z) {
                    chunks[x, y, z].Refresh();
                }
            }
        }
    }

    private void Update() {
        if (regenerate) {
            heightMap = heightMapGenerator.Generate(resolution, chunkResolution);
        }
        if (Input.GetMouseButtonDown(1) && refresh) {
            for (int x = 0; x < chunkResolution; ++x) {
                for (int y = 0; y < chunkResolution; ++y) {
                    for (int z = 0; z < chunkResolution; ++z) {
                        chunks[x, y, z].updateValues(isoLevel, noiseWeight, interpolation, regenerate);
                    }
                }
            }
            for (int x = 0; x < chunkResolution; ++x) {
                for (int y = 0; y < chunkResolution; ++y) {
                    for (int z = 0; z < chunkResolution; ++z) {
                        chunks[x, y, z].Refresh();
                    }
                }
            }
        }
    }

    public void EditVertex(Vector3 point, int value) {
        int vertexX = (int)(point.x / cubeSize);
        int vertexY = (int)(point.y / cubeSize);
        int vertexZ = (int)(point.z / cubeSize);
        int chunkX = vertexX / resolution;
        int chunkY = vertexY / resolution;
        int chunkZ = vertexZ / resolution;

        vertexX -= chunkX * resolution;
        vertexY -= chunkY * resolution;
        vertexZ -= chunkZ * resolution;
        Vertex vertex = chunks[chunkX, chunkY, chunkZ].cubeVertices[vertexX, vertexY, vertexZ];
        Debug.Log(vertex.globalPosition);
        //vertex.SetValue(value);
        if (chunkX > 0) {
            chunks[chunkX - 1, chunkY, chunkZ].Refresh();
        }
        if (chunkZ > 0) {
            chunks[chunkX, chunkY, chunkZ - 1].Refresh();
        }
        if (chunkY > 0) {
            chunks[chunkX, chunkY - 1, chunkZ].Refresh();
        }
        if (chunkX > 0 && chunkY > 0) {
            chunks[chunkX - 1, chunkY - 1, chunkZ].Refresh();
        }
        if (chunkZ > 0 && chunkY > 0) {
            chunks[chunkX, chunkY - 1, chunkZ - 1].Refresh();
        }
        if (chunkX > 0 && chunkZ > 0) {
            chunks[chunkX - 1, chunkY, chunkZ - 1].Refresh();
        }
        if (chunkX > 0 && chunkY > 0 && chunkZ > 0) {
            chunks[chunkX - 1, chunkY - 1, chunkZ - 1].Refresh();
        }
        chunks[chunkX, chunkY, chunkZ].Refresh();
        //Debug.Log(vertexX + ", " + vertexY + ", " + vertexZ);
    }

    private void CreateChunk(int x, int y, int z) {
        CubeGrid chunk = Instantiate(cubeGridPrefab) as CubeGrid;
        chunk.Initialize(resolution, chunkSize, isoLevel, interpolation, x, y, z, heightMap, noiseWeight);
        chunk.transform.parent = transform;
        chunk.transform.localPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
        if (x > 0) {
            chunks[x - 1, y, z].xNeighbor = chunk;
        }
        if (y > 0) {
            chunks[x, y - 1, z].yNeighbor = chunk;
        }
        if (z > 0) {
            chunks[x, y, z - 1].zNeighbor = chunk;
        }
        if (x > 0 && z > 0) {
            chunks[x - 1, y, z - 1].xzNeighbor = chunk;
        }
        if (x > 0 && y > 0) {
            chunks[x - 1, y - 1, z].xyNeighbor = chunk;
        }
        if (z > 0 && y > 0) {
            chunks[x, y - 1, z - 1].zyNeighbor = chunk;
        }
        if (x > 0 && y > 0 && z > 0) {
            chunks[x - 1, y - 1, z - 1].xyzNeighbor = chunk;
        }
        chunks[x, y, z] = chunk;
    }
}
