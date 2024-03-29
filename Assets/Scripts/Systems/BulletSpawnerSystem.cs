using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial struct BulletSpawnerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var isPressedSpace = Input.GetKey(KeyCode.Space);

            foreach (var (tf, spawner)
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRW<BulletSpawnerComponent>>()
                    )
            {
                if (!isPressedSpace)
                {
                    spawner.ValueRW.lastSpawnedTime = 0;
                }
                else
                {
                    if (spawner.ValueRO.lastSpawnedTime <= 0)
                    {
                        //Spawn Bullet
                        var newBulletE = state.EntityManager.Instantiate(spawner.ValueRO.prefab);
                        state.EntityManager.SetComponentData(newBulletE, new LocalTransform
                        {
                            Position = tf.ValueRO.Position + spawner.ValueRO.offset,
                            Scale = .2f,
                            Rotation = Quaternion.identity,
                        });
                        state.EntityManager.SetComponentData(newBulletE, new BulletComponent
                        {
                            speed =  8f,
                            direction = calculateDirection(tf),
                        });
                        spawner.ValueRW.lastSpawnedTime = spawner.ValueRO.spawnSpeed;
                    }
                    else
                    {
                        spawner.ValueRW.lastSpawnedTime -= SystemAPI.Time.DeltaTime;
                    }
                }
            }
        }

        private float3 calculateDirection(RefRO<LocalTransform> tf)
        {
            //TODO:
            return tf.ValueRO.Forward();
        }
    }
}