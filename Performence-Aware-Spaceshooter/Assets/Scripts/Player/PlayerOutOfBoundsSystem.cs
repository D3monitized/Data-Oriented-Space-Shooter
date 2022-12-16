using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine; 

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
public partial class PlayerOutOfBoundsSystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem m_EndFixedStepSimECB;

	protected override void OnCreate()
	{
        m_EndFixedStepSimECB = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<GameSettingsComponent>();
	}

	protected override void OnUpdate()
    {       
        var commandBuffer = m_EndFixedStepSimECB.CreateCommandBuffer().AsParallelWriter();
   
        var settings = GetSingleton<GameSettingsComponent>();

        Entities
        .WithAll<PlayerTag>()
        .ForEach((Entity entity, int entityInQueryIndex, in Translation position) => {

            if(Mathf.Abs(position.Value.x) > settings.levelWidth / 2 ||
               Mathf.Abs(position.Value.y) > settings.levelHeight / 2 ||
               Mathf.Abs(position.Value.z) > settings.levelDepth / 2)
            {
                //If it is out of bounds wee add the DestroyTag component to the entity and return
                commandBuffer.AddComponent(entityInQueryIndex, entity, new DestroyTag());
                return;
            }

        }).ScheduleParallel();

        m_EndFixedStepSimECB.AddJobHandleForProducer(Dependency);
    }
}
