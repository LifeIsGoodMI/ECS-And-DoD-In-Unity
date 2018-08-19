using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Just a file to store a bunch of components for our systems.
/// </summary>


public struct SpawnCooldown : IComponentData
{
    public float value;
}

public struct RotationSpeed : IComponentData
{
    public float value;
}