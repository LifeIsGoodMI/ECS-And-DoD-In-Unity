using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


/// <summary>
/// Removes "dead" birds.
/// 
/// In the classic version, a bird died on collision with anything. Since there's no physics component yet,
/// I'm performing a basic "bounds" check, comparing the bird's current y position to the players y position.
/// If we're close enough to the player's height, that means we've pretty much collided with the player & are dead.
/// </summary>
public class KillerbirdRemoveSystem : ComponentSystem
{
    struct Data
    {
        public readonly int Length; // Auto populated for us, needs to be named Length though.
        public EntityArray killerbirds;
        public ComponentDataArray<Position> positions;
    }

    [Inject] private Data data;



    protected override void OnUpdate()
    {
        for (int i = 0; i < data.Length; i++)
        {
            var ready = IsReadyForRemoval(i);
            if (ready)
                Remove(i);
        }
    }


    /// <summary>
    /// Perform a very basic bounds check.
    /// </summary>
    private bool IsReadyForRemoval (int idx)
    {
        var height = math.abs(Bootstrap.player.position.y - data.positions[idx].Value.y);
        return height <= Bootstrap.gameSettings.boundsThreshold;
    }


    /// <summary>
    /// Actually destroy the entity.
    /// </summary>
    private void Remove (int idx)
    {
        // If we're removing entries from the EntityArray (which is a NativeArray) & accessing at again in the same frame
        // Unity is going to tell us, that this is not allowed since we're trying to access the array when it's being deallocated.

        // For situations like this, we're introduced to command buffers.
        // Command buffers are really common in e.g. graphics programming.
        // They really do what the name says: We're buffering commands, so they can be executed later.
        // Component systems already come with a command buffer called PostUpdateCommands.
        // As the name tells you, all the commands recorded in this buffer are going to be executed after the OnUpdate method has finished.
        PostUpdateCommands.DestroyEntity(data.killerbirds[idx]);
    }
}
