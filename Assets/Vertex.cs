using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex {

    public int value = 0;
    public Vector3 position;
    public Material material;

    public Vertex() {}

    public Vertex(float x, float y, float z, int value) {
        this.value = value;
        this.position = new Vector3(x, y, z);
    }


    public Vertex(float x, float y, float z, int value, GameObject o) {
        this.value = value;
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

    public void SetValue(int value) {
        this.value = value;
    }

    public int GetValue() {
        return this.value;
    }
}
