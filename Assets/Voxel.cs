using System;
using UnityEngine;

[Serializable]
public class Voxel {

    public bool state;

    public float xEdge, yEdge;

    public Voxel() {}

    public Voxel(int x, int y, float size) {
        position.x = (x + 0.5f) * size;
        position.y = (y + 0.5f) * size;

        xEdge = position.x + size * 0.5f;
        yEdge = position.y + size * 0.5f;
    }

    public void BecomeXDummyOf(Voxel voxel, float offset) {
		state = voxel.state;
		position = voxel.position;
		position.x += offset;
		xEdge = voxel.xEdge + offset;
		yEdge = voxel.yEdge;
	}

	public void BecomeYDummyOf(Voxel voxel, float offset) {
		state = voxel.state;
		position = voxel.position;
		position.y += offset;
		xEdge = voxel.xEdge;
		yEdge = voxel.yEdge + offset;
	}

	public void BecomeXYDummyOf(Voxel voxel, float offset) {
		state = voxel.state;
		position = voxel.position;
		position.x += offset;
		position.y += offset;
		xEdge = voxel.xEdge + offset;
		yEdge = voxel.yEdge + offset;
	}
}
