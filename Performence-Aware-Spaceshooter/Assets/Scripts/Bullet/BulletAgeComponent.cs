using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BulletAgeComponent : IComponentData
{
	public float maxAge;
	public float age; 

    public BulletAgeComponent(float maxAge)
	{
		this.maxAge = maxAge;
		age = 0; 
	}
}
