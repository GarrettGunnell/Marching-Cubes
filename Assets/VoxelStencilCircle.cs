using UnityEngine;

public class VoxelStencilCircle : VoxelStencil {

    private float sqrRadius;

    public override void Initialize(bool fillType, float radius) {
        base.Initialize(fillType, radius);
        sqrRadius = radius * radius;
    }

    public override void Apply(Voxel voxel) {
        float x = voxel.position.x - centerX;
        float y = voxel.position.y - centerY;
        if (x * x + y * y <= sqrRadius) {
            voxel.state = fillType;
        }
    }
}
