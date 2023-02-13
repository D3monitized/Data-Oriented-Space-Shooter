using System;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BulletSpawnOffsetComponent : IComponentData
{
	public float3 Value;
}
