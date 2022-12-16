using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class InputMovementSystem : SystemBase
{
	protected override void OnCreate()
	{
		RequireSingletonForUpdate<GameSettingsComponent>();
	}

	protected override void OnUpdate()
	{
		//we must declare our local variables to be able to use them in the .ForEach() below
		var gameSettings = GetSingleton<GameSettingsComponent>();
		var deltaTime = Time.DeltaTime;

		//we will control thrust with WASD"
		byte thrust, reverseThrust;
		thrust = reverseThrust = 0;

		float hor = 0;
		float ver = 0;

		//we grab "WASD" for thrusting

		hor = Input.GetAxis("Horizontal");

		if (Input.GetKey(KeyCode.W))		
			thrust = 1;		
		if (Input.GetKey(KeyCode.S))		
			reverseThrust = 1;		

		if (Input.GetKey(KeyCode.LeftShift))
			ver = -1;
		if (Input.GetKey(KeyCode.LeftControl))
			ver = 1;		

		Entities
		.WithAll<PlayerTag>()
		.ForEach((Entity entity, ref Rotation rotation, ref VelocityComponent velocity) =>
		{
			if (thrust == 1)
			{   //thrust forward of where the player is facing
				velocity.Value += (math.mul(rotation.Value, new float3(0, 0, 1)).xyz) * gameSettings.playerForce * deltaTime;
			}
			if (reverseThrust == 1)
			{   //thrust backwards of where the player is facing
				velocity.Value += (math.mul(rotation.Value, new float3(0, 0, -1)).xyz) * gameSettings.playerForce * deltaTime;
			}
			if (hor != 0 || ver != 0)
			{
				float lookSpeedH = 1f;
				float lookSpeedV = 1f; 

				Quaternion currentQuaternion = rotation.Value;
				float yaw = currentQuaternion.eulerAngles.y;
				float pitch = currentQuaternion.eulerAngles.x;

				yaw += lookSpeedH * hor;
				pitch -= lookSpeedV * ver;
				Quaternion newQuaternion = Quaternion.identity;
				newQuaternion.eulerAngles = new Vector3(pitch, yaw, 0);
				rotation.Value = newQuaternion;
			}

		}).ScheduleParallel();
	}
}
