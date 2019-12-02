using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMap : MonoBehaviour {

    public float size = 2f;

    public int resolution = 8;
    public int chunkResolution = 2;

    public CubeGrid cubeGridPrefab;


    private CubeGrid[, ,] chunks;
    private float chunkSize, cubeSize, halfSize;

    private void Awake() {
        halfSize = size * 0.5f;
        chunkSize = size / chunkResolution;
        cubeSize = chunkSize / resolution;

        chunks = new CubeGrid[resolution, resolution, resolution];
        for (int x = 0; x < chunkResolution; ++x) {
            for (int y = 0; y < chunkResolution; ++y) {
                for (int z = 0; z < chunkResolution; ++z) {
                    CreateChunk(x, y, z);
                }
            }
        }
    }

    private void CreateChunk(int x, int y, int z) {
        CubeGrid chunk = Instantiate(cubeGridPrefab) as CubeGrid;
        chunk.Initialize(resolution, chunkSize);
        chunk.transform.parent = transform;
        chunk.transform.localPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
        chunks[x, y, z] = chunk;
    }
}
