using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGrid : MonoBehaviour {

    public GameObject cubePrefab;
    public int resolution = 8;
    public float size = 8;

    private bool[, ,] vertices;
    private float distanceMod;

    void Awake() {
        vertices = new bool[resolution, resolution, resolution];
        distanceMod = size / resolution;
        for (int x = 0; x < resolution; ++x) {
            for (int y = 0; y < resolution; ++y) {
                for (int z = 0; z < resolution; ++z) {
                    GameObject o = Instantiate(cubePrefab) as GameObject;
                    o.transform.parent = transform;
                    o.transform.localPosition = new Vector3(x * distanceMod, y * distanceMod, z * distanceMod);
                    o.transform.localScale = Vector3.one * distanceMod * 0.05f;
                }
            }
        }
    }
}
