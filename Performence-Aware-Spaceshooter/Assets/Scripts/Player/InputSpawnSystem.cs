using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine; 

public partial class InputSpawnSystem : SystemBase
{
    private EntityQuery m_playerQuery;

    private BeginSimulationEntityCommandBufferSystem m_beginSimECB;

    private Entity m_prefab;

    private Entity m_bulletPrefab;
    private float m_PerSecond = 10f;
    private float m_nextTime = 0; 

	protected override void OnCreate()
	{
        m_playerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());

        m_beginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<GameSettingsComponent>(); 
	}

	protected override void OnUpdate()
    {
        if(m_prefab == Entity.Null || m_bulletPrefab == Entity.Null)
		{
            m_prefab = GetSingleton<PlayerAuthoringComponent>().Prefab;
            m_bulletPrefab = GetSingleton<BulletAuthoringComponent>().Prefab; 

            return; 
		}

        byte shoot;
        shoot = 0;
        byte selfDestruct;
        selfDestruct = 0;
        var playerCount = m_playerQuery.CalculateEntityCountWithoutFiltering();

		if (Input.GetKey(KeyCode.Space))
		{
            shoot = 1; 
		}

		if (Input.GetKey(KeyCode.P))
		{
            selfDestruct = 1;
		}

        if (shoot == 1 && playerCount < 1)
        {
            EntityManager.Instantiate(m_prefab);
            return;
        }

        var commandBuffer = m_beginSimECB.CreateCommandBuffer().AsParallelWriter();
        //We must declare our local variables before the .ForEach()
        var gameSettings = GetSingleton<GameSettingsComponent>();
        var bulletPrefab = m_bulletPrefab;

        //we are going to implement rate limiting for shooting
        var canShoot = false;
        if (UnityEngine.Time.time >= m_nextTime)
        {
            canShoot = true;
            m_nextTime += (1 / m_PerSecond);
        }

        Entities
        .WithAll<PlayerTag>()
        .ForEach((Entity entity, int entityInQueryIndex, in Translation position, in Rotation rotation,
                in VelocityComponent velocity, in BulletSpawnOffsetComponent bulletOffset) =>
        {
            if(selfDestruct == 1)
			{
                commandBuffer.AddComponent(entityInQueryIndex, entity, new DestroyTag { });
			}
            
            //If we don't have space bar pressed we don't have anything to do
            if (shoot != 1 || !canShoot)
            {
                return;
            }

            // We create the bullet here
            var bulletEntity = commandBuffer.Instantiate(entityInQueryIndex, bulletPrefab);

            //we set the bullets position as the player's position + the bullet spawn offset
            //math.mul(rotation.Value,bulletOffset.Value) finds the position of the bullet offset in the given rotation
            //think of it as finding the LocalToParent of the bullet offset (because the offset needs to be rotated in the players direction)
            var newPosition = new Translation { Value = position.Value }; //Value = position.Value + math.mul(rotation.Value, bulletOffset.Value).xyz
            commandBuffer.SetComponent(entityInQueryIndex, bulletEntity, newPosition);


            // bulletVelocity * math.mul(rotation.Value, new float3(0,0,1)).xyz) takes linear direction of where facing and multiplies by velocity
            // adding to the players physics Velocity makes sure that it takes into account the already existing player velocity (so if shoot backwards while moving forwards it stays in place)
            var vel = new VelocityComponent { Value = (gameSettings.bulletVelocity * math.mul(rotation.Value, new float3(0, 0, 1)).xyz) + velocity.Value };

            commandBuffer.SetComponent(entityInQueryIndex, bulletEntity, vel);

        }).ScheduleParallel();

        m_beginSimECB.AddJobHandleForProducer(Dependency);
    }


}

