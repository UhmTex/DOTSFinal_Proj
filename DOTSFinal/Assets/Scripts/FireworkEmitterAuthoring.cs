using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FireworkEmitterAuthoring : MonoBehaviour
{
    [Header("Burst settings")]
    public float BurstInterval = 3f;
    public int ParticlesPerBurst = 50;
    public float ParticleLifetime = 4f;

    [Header("Initial speed")]
    public float MinSpeed = 5f;
    public float MaxSpeed = 10f;

    [Header("Physics")]
    public float Gravity = -9.81f;

    [Header("Visuals")]
    public Color BaseColor = Color.yellow;
    public GameObject ParticlePrefab;

    [Header("Size")]
    public float InitialSize = 0.15f;

    class Baker : Baker<FireworkEmitterAuthoring>
    {
        public override void Bake(FireworkEmitterAuthoring src)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FireworkEmitterData
            {
                BurstInterval = src.BurstInterval,
                TimeUntilNextBurst = src.BurstInterval,
                ParticlesPerBurst = src.ParticlesPerBurst,
                ParticleLifetime = src.ParticleLifetime,
                MinSpeed = src.MinSpeed,
                MaxSpeed = src.MaxSpeed,
                Gravity = src.Gravity,
                BaseColor = new float4(src.BaseColor.r,
                           src.BaseColor.g,
                           src.BaseColor.b,
                           src.BaseColor.a),
                ParticlePrefab = GetEntity(src.ParticlePrefab, TransformUsageFlags.Dynamic),
                InitialSize = src.InitialSize
            });
        }
    }
}

public struct FireworkEmitterData : IComponentData
{
    public float BurstInterval;
    public float TimeUntilNextBurst;
    public int ParticlesPerBurst;
    public float ParticleLifetime;
    public float MinSpeed;
    public float MaxSpeed;
    public float Gravity;
    public float4 BaseColor;
    public float InitialSize;
    public Entity ParticlePrefab;
}