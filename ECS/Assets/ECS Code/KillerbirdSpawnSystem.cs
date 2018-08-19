using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

/// <summary>
/// Spawns new killerbirds constantly.
/// </summary>
public class KillerbirdSpawnSystem : ComponentSystem
{
    struct Data
    {
        public ComponentDataArray<SpawnCooldown> spawnCooldown;
    }

    /// <summary>
    /// The Inject attribute is going to pick the types defined in our Data struct (SpawnTime, KillerBird)
    /// and inject said types as dependencies into this component system.
    /// Note that we have put our components into an ComponentDataArray, even though we only have 1 instance of both components.
    /// That is because the inject attribute is looking for a generic type. 
    /// (ComponentDataArray can take any type that is a struct & implements the IComponentData interface)
    /// From this generic type, the type of the first generic argument is then grabbed & injected into our ComponentSystem as a dependency.
    /// </summary>
    [Inject] private Data data;


    /// <summary>
    /// We need to initialize our SpawnCooldown component,
    /// to fulfill the dependencies injected into the system by the [Inject] attribute.
    /// 
    /// OnUpdate won't be called until we do so.
    /// </summary>
    public static void SetupComponentData(EntityManager entityManager)
    {
        var stateEntity = entityManager.CreateEntity(typeof(SpawnCooldown));
        entityManager.SetComponentData(stateEntity, new SpawnCooldown { value = 0.0f });
    }


    protected override void OnUpdate()
    {
        var isReady = IsReady();
        if (isReady)
            Spawn();
    }


    /// <summary>
    /// This method simply checks if we're ready to spawn the next enemy.
    /// 
    /// Note the usage of value types here. Since there's no ref return yet (apparently comming soon),
    /// we'll have to put our altered object back into our data.spawnCooldown array at the end.
    /// </summary>
    /// <returns></returns>
    private bool IsReady ()
    {
        var time = data.spawnCooldown[0].value - Time.deltaTime;
        var ready = time <= 0.0f;

        if (ready)
            time = Bootstrap.gameSettings.spawnCooldown;

        data.spawnCooldown[0] = new SpawnCooldown { value = time };

        return ready;
    }

    private void Spawn ()
    {
        var spawnCenter = new float3(0, 14, 0); // Just some random elevated spawn point. We could of course pass in something from the editor here...
        var x = Random.Range(-15, 15);
        var pos = new float3(spawnCenter.x + x, spawnCenter.y, spawnCenter.z);

        var entity = EntityManager.CreateEntity(Bootstrap.killerbirdArchetype);
        EntityManager.SetComponentData(entity, new Position { Value = pos });
        EntityManager.SetComponentData(entity, new Rotation { Value = quaternion.identity});
        EntityManager.SetComponentData(entity, new MoveSpeed { speed = Bootstrap.gameSettings.moveSpeed });
        EntityManager.SetComponentData(entity, new RotationSpeed { value = Bootstrap.gameSettings.rotationSpeed });
        EntityManager.SetSharedComponentData(entity, Bootstrap.killerbirdRenderer);

        // Note: We could also create entities in batches, instead of instantiating one at a time.
        // Sample, instantiate 100 killerbirds at once, and put those entities into the killerbirdSwarm array.
        // var killerbirdSwarm = new NativeArray<Entity>(100, Allocator.Temp);
        // EntityManager.CreateEntity(Bootstrap.killerbirdArchetype, killerbirdSwarm);
    }
}
