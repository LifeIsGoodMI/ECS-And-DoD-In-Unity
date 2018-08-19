using UnityEngine;

/// <summary>
/// Exposes common data to the Unity editor, until ECS components can be serialized in the editor.
/// </summary>
public class GameSettings : MonoBehaviour
{
    public float spawnCooldown = 0.001f;

    public float moveSpeed = 10.0f;
    public float rotationSpeed = 3.0f;

    public float boundsThreshold = 0.25f;
}
