using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex {

    public float value;
    public Vector3 position;
    public Material material;
    public Vector3 globalPosition;


    public Vertex() {}

    public Vertex(float x, float y, float z, int chunkX, int chunkY, int chunkZ, float size, float voxelSize, int resolution, float[,] heightMap, float noiseWeight) {
        int globalX = (int)(x / voxelSize);
        int globalY = (int)(y / voxelSize);
        int globalZ = (int)(z / voxelSize);

        globalX += chunkX * resolution;
        globalY += chunkY * resolution;
        globalZ += chunkZ * resolution;
        globalPosition = new Vector3(globalX, globalY, globalZ);
        this.value = -globalPosition.y + heightMap[(int)globalPosition.z, (int)globalPosition.x] * noiseWeight;
        this.position = new Vector3(x, y, z);
    }


    public Vertex(float x, float y, float z, GameObject o) {
        this.value = 0;
        this.position = new Vector3(x, y, z);
        this.material = o.GetComponent<MeshRenderer>().material;
    }

    public void BecomeXDummyOf(Vertex vertex, float offset) {
        this.value = vertex.value;
        this.position = vertex.position;
        this.position.x += offset;
    }

    public void BecomeZDummyOf(Vertex vertex, float offset) {
        this.value = vertex.value;
        this.position = vertex.position;
        this.position.z += offset;
    }

    public void BecomeYDummyOf(Vertex vertex, float offset) {
        this.value = vertex.value;
        this.position = vertex.position;
        this.position.y += offset;
    }

    public void BecomeXYDummyOf(Vertex vertex, float offset) {
        this.value = vertex.value;
        this.position = vertex.position;
        this.position.x += offset;
        this.position.y += offset;
    }

    public void BecomeZYDummyOf(Vertex vertex, float offset) {
        this.value = vertex.value;
        this.position = vertex.position;
        this.position.y += offset;
        this.position.z += offset;
    }

    public void BecomeXZDummyOf(Vertex vertex, float offset) {
        this.value = vertex.value;
        this.position = vertex.position;
        this.position.x += offset;
        this.position.z += offset;
    }

    public void BecomeXYZDummyOf(Vertex vertex, float offset) {
        this.value = vertex.value;
        this.position = vertex.position;
        this.position.x += offset;
        this.position.y += offset;
        this.position.z += offset;
    }

    public void SetColor(Color color) {
        this.material.color = color;
    }

    public void SetValue(float [,] heightMap, float noiseWeight) {
        this.value = -globalPosition.y + heightMap[(int)globalPosition.z, (int)globalPosition.x] * noiseWeight;
    }

    public float GetValue() {
        return this.value;
    }
}
