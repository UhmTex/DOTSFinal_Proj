using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class FireworkParticleAuthoring : MonoBehaviour
{
    class Baker : Baker<FireworkParticleAuthoring>
    {
        public override void Bake(FireworkParticleAuthoring _)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new FireworkParticle
            {
                Velocity = float3.zero,
                Age = 0f,
                Lifetime = 1f,
                Gravity = -9.81f
            });

            AddComponent(entity, new URPMaterialPropertyBaseColor
            {
                Value = new float4(1f, 1f, 1f, 1f)
            });
        }
    }
}

public struct FireworkParticle : IComponentData
{
    public float3 Velocity;
    public float Age;
    public float Lifetime;
    public float Gravity;
    public float InitialSize;
}