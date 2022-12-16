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

	protected override void OnCreate()
	{
        m_playerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());

        m_beginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
    {
        if(m_prefab == Entity.Null)
		{
            m_prefab = GetSingleton<PlayerAuthoringComponent>().Prefab;

            return; 
		}

        byte shoot;
        shoot = 0;
        var playerCount = m_playerQuery.CalculateEntityCountWithoutFiltering();

		if (Input.GetKey(KeyCode.Space))
		{
            shoot = 1; 
		}

        if (shoot == 1 && playerCount < 1)
        {
            EntityManager.Instantiate(m_prefab);
            return;
        }       
    }
}
