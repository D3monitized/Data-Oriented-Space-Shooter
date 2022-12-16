using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial class PlayerDestructionSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimEcb;

    protected override void OnCreate()
    {
        m_EndSimEcb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EndSimEcb.CreateCommandBuffer().AsParallelWriter();

        Entities
        .WithAll<PlayerTag, DestroyTag>()
        .ForEach((Entity entity, int entityInQueryIndex) => {

            commandBuffer.DestroyEntity(entityInQueryIndex, entity);

        }).ScheduleParallel();

        m_EndSimEcb.AddJobHandleForProducer(Dependency);
    }
}
