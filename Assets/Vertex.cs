using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex {

    public int value = 0;
    public float x, y, z;
    public Material material;

    public Vertex(float x, float y, float z, int value, GameObject o) {
        this.value = value;
        this.x = x;
        this.y = y;
        this.z = z;
        this.material = o.GetComponent<MeshRenderer>().material;
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
