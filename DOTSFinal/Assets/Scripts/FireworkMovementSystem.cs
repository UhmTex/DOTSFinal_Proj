using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct FireworkMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (xform, particle) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<FireworkParticle>>())
        {
            ref var p = ref particle.ValueRW;
            ref var t = ref xform.ValueRW;

            p.Velocity.y += p.Gravity * dt;
            t.Position += p.Velocity * dt;
            p.Age += dt;

            float progress = math.saturate(p.Age / p.Lifetime);
            t.Scale = math.max(0f, p.InitialSize * (1f - progress));
        }
    }
}