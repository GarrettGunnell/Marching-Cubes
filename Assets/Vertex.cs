using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex {

    public int value = 0;

    public Vertex(int value) {
        this.value = value;
    }

    public void SetValue(int value) {
        this.value = value;
    }

    public int GetValue() {
        return this.value;
    }
}
