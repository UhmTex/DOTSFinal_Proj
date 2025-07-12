using Unity.Collections;
using Unity.Entities;

public partial struct FireworkDestroySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (p, entity) in
                 SystemAPI.Query<RefRO<FireworkParticle>>().WithEntityAccess())
        {
            if (p.ValueRO.Age >= p.ValueRO.Lifetime)
                ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
    }
}