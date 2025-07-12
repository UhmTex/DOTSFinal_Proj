using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial struct FireworkSpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (emitter, xform) in
                 SystemAPI.Query<RefRW<FireworkEmitterData>, RefRO<LocalTransform>>())
        {
            emitter.ValueRW.TimeUntilNextBurst -= dt;
            if (emitter.ValueRW.TimeUntilNextBurst > 0f)
                continue;

            emitter.ValueRW.TimeUntilNextBurst = emitter.ValueRO.BurstInterval;

            var random = Unity.Mathematics.Random.CreateFromIndex(
                (uint)state.WorldUnmanaged.Time.ElapsedTime * 997u +
                (uint)emitter.GetHashCode());

            for (int i = 0; i < emitter.ValueRO.ParticlesPerBurst; i++)
            {
                Entity p = ecb.Instantiate(emitter.ValueRO.ParticlePrefab);

                float3 dir = math.normalize(random.NextFloat3Direction());
                float speed = random.NextFloat(emitter.ValueRO.MinSpeed,
                                                emitter.ValueRO.MaxSpeed);

                ecb.SetComponent(p, new FireworkParticle
                {
                    Velocity = dir * speed,
                    Age = 0f,
                    Lifetime = emitter.ValueRO.ParticleLifetime,
                    Gravity = emitter.ValueRO.Gravity,
                    InitialSize = emitter.ValueRO.InitialSize
                });

                ecb.SetComponent(p, LocalTransform.FromPositionRotationScale(
                    xform.ValueRO.Position,
                    quaternion.identity,
                    emitter.ValueRO.InitialSize));

                ecb.SetComponent(p, new URPMaterialPropertyBaseColor
                {
                    Value = emitter.ValueRO.BaseColor
                });
            }
        }

        ecb.Playback(state.EntityManager);
    }
}
