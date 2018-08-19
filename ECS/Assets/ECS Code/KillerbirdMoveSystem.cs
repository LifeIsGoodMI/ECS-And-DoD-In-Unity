using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Moves & rotates every spawned killerbird to the player.
/// </summary>
public class KillerbirdMoveSystem : ComponentSystem
{
    /// <summary>
    /// We're using the Inject attribute, so our data here need's to be initialized before the system runs.
    /// However, we don't need a SetupComponentData method this time, as we are initializing our data already 
    /// in the Spawn method of the KillerbirdSpawnSystem!
    /// </summary>
    struct Data
    {
        public readonly int Length; // Auto populated for us, needs to be named Length though.
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Rotation> rotations;

        // Not writing to those at the moment.
        public readonly ComponentDataArray<MoveSpeed> moveSpeeds;
        public readonly ComponentDataArray<RotationSpeed> rotSpeeds;
    }

    [Inject] private Data data;


    protected override void OnUpdate()
    {
        var deltaTime = Time.deltaTime;
        for (int i = 0; i < data.Length; i++)
        {
            Move(i, deltaTime);
        }
    }


    private void Move (int idx, float deltaTime)
    {
        var resultRot = quaternion.identity;

        var targetPos = (float3)Bootstrap.player.position;
        var pos = data.positions[idx].Value;

        var dir = targetPos - pos;

        // Note, how we can pass in a Vector3 here, even though dir is a float3.
        // Structures like quaternion, float3 etc. have implicit conversion operators to their EC counterparts (Quaternion, Vector3, ...)
        if (!dir.Equals(Vector3.zero))
        {
            var angle = math.atan2(dir.y, dir.x);
            // Note: Quaternion.AngleAxis used degrees. quaternion.axisAngle, however, uses radians.
            //       No need to multiply the angle by Rad2Deg.
            var targetRot = quaternion.axisAngle(Vector3.forward, angle);

            resultRot = math.slerp(data.rotations[idx].Value, targetRot, deltaTime * data.rotSpeeds[idx].value);
        }

        // There's no physics component yet, so we'll have to alter the position directly for the moment.
        pos += math.normalize(dir) * data.moveSpeeds[idx].speed * deltaTime;

        // We're working with value types. Don't forget to copy our data back into our components.
        data.rotations[idx] = new Rotation(resultRot);
        data.positions[idx] = new Position(pos);
    }
}
