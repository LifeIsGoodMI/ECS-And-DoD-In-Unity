using UnityEngine;

// Note all those new namespaces.
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.SceneManagement;


/// <summary>
/// This class basically functions as an initial game manager. 
/// Since a lot of stuff depends on MonoBehaviours & the like (especially Serialization, which includes inspectors so you can tweak settings on the fly directly in the editor)
/// it is used mostly to interact with the ECS. We're pretty much just grabbing data from the editor and passing it to the ECS.
/// Typically, we are going to do some preparation here for future entities such as defining entity archetypes.
/// </summary>
public class Bootstrap
{
    public static EntityArchetype killerbirdArchetype;
    public static MeshInstanceRenderer killerbirdRenderer;
    public static GameSettings gameSettings;
    public static Transform player;

    /// <summary>
    /// The attribute is going to call the Init method automatically for us when the game starts.
    /// Since we're not deriving from MonoBehaviour we're pretty much replacing a bunch of events such as Awake.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init ()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;

        // Grab the GameSettings MonoBehaviour to access our data.
        gameSettings = Object.FindObjectOfType<GameSettings>();

        // Archetypes are basically just a prefab.
        // We can define a bunch of components that we expect entites to have when we pass this archetype.
        // This can speed up entity instantiation since Unity has the possibility to add all of those components in a batch and align memory accordingly.

        // Grab the ECS EntityManager, which is basically the link to our entities.
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        /*
         * Unity already provides a bunch of structures that are commonly used. (they will be introducing more and more structures over time)
         * So far, we had all transform settings combined in one component (Transform) but often times we're only interested in 1 or 2 settings.
         * (E.g. how often do you really need the scale vector of the transform class ?)
         * We can align our memory according to our current needs really easy with these new components.
         * - Position contains a 3D vector (float3) to describe an entites position. (2D equivalent = Position2D)
         * - Rotation contains a quaternion to describe an entites rotation. (2D equivalent = Heading2D)
         * - MoveSpeed contains a float to describe an entites movement speed (simple as that.)
         * - TransformMatrix contains a 4x4 matrix (float4x4) that is required by the MeshInstanceRenderer (the new MeshRenderer)
         *   In computer games, translation (position), orientation (rotation) and scaling are usually all defined by a single 4x4 matrix.
         *   This is abstracted from us most of the time by classes such as Transform, that separate all those components.
         *   But don't worry. You won't have to perform operations with this matrix.
        */
        
        killerbirdArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(TransformMatrix), typeof(MoveSpeed), typeof(RotationSpeed), typeof(MeshInstanceRenderer));
        killerbirdRenderer = GetLookFromPrototype<KillerbirdPrototype>();
        
        KillerbirdSpawnSystem.SetupComponentData(entityManager);

        player = GetPlayer();
    }


    /// <summary>
    /// Find a GameObject in the scene with a MonoBehaviour of type T
    /// Return the MeshInstanceRendererComponent of this GameObject.
    /// </summary>
    private static MeshInstanceRenderer GetLookFromPrototype<T>() where T : MonoBehaviour
    {
        var prototype = Object.FindObjectOfType<T>();
        var result = prototype.GetComponent<MeshInstanceRendererComponent>();
        return result.Value;
    }

    /// <summary>
    /// Find the player through the Player MonoBehaviour.
    /// </summary>
    private static Transform GetPlayer ()
    {
        var p = Object.FindObjectOfType<Player>();
        return p.transform;
    }
}
