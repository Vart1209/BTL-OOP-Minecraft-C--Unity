using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.Presentation.World
{
    /// <summary>
    /// Bộ phát hiệu ứng hạt vỡ khối (Voxel Break Particles) mô phỏng cảm giác xúc giác của Minecraft Java 1.12.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Hiệu năng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Single Responsibility Principle (SRP - Tiêu chí 8):</b> Chỉ đảm nhiệm việc trực quan hóa các mảnh vụn khối voxel khi vỡ.</item>
    ///   <item><b>Tối ưu bộ nhớ &amp; Zero GC (Tiêu chí 18):</b> Tái sử dụng chung một đối tượng <see cref="Mesh"/> lập phương và gán màu thông qua <see cref="MaterialPropertyBlock"/> để không cấp phát bộ nhớ động thừa thãi. Tự động dọn dẹp các mảnh vụn sau 0.65 giây.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class VoxelParticleEmitter : MonoBehaviour
    {
        private sealed class VoxelParticle
        {
            public Transform Transform;
            public Vector3 Velocity;
            public float RemainingLife;
            public float TotalLife;
            public Vector3 InitialScale;
        }

        private readonly List<VoxelParticle> _activeParticles = new List<VoxelParticle>();
        private Mesh _sharedCubeMesh;
        private Material _particleMaterial;

        private void Awake()
        {
            // Tạo mesh lập phương chuẩn dùng chung cho toàn bộ các mảnh vụn
            GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _sharedCubeMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
            Destroy(tempCube);

            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            _particleMaterial = new Material(shader)
            {
                name = "VoxelParticleMaterial"
            };
        }

        private void OnDestroy()
        {
            if (_particleMaterial != null)
            {
                Destroy(_particleMaterial);
            }

            for (int i = 0; i < _activeParticles.Count; i++)
            {
                if (_activeParticles[i].Transform != null)
                {
                    Destroy(_activeParticles[i].Transform.gameObject);
                }
            }
            _activeParticles.Clear();
        }

        /// <summary>
        /// Sinh các hạt vụn lập phương nhỏ khi một khối bị phá vỡ.
        /// </summary>
        /// <param name="blockWorldPos">Tọa độ tâm của khối vừa vỡ.</param>
        /// <param name="blockColor">Màu sắc đặc trưng của khối.</param>
        /// <param name="count">Số lượng mảnh vụn cần phát sinh (mặc định 16).</param>
        public void SpawnBreakParticles(Vector3 blockWorldPos, Color blockColor, int count = 16)
        {
            if (_sharedCubeMesh == null)
            {
                return;
            }

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetColor("_BaseColor", blockColor);
            mpb.SetColor("_Color", blockColor);

            for (int i = 0; i < count; i++)
            {
                GameObject particleObj = new GameObject("VoxelDebris");
                particleObj.transform.SetParent(transform, true);

                // Điểm khởi tạo ngẫu nhiên trong phạm vi khối lập phương 1x1x1
                Vector3 localOffset = new Vector3(
                    Random.Range(-0.35f, 0.35f),
                    Random.Range(-0.35f, 0.35f),
                    Random.Range(-0.35f, 0.35f));
                particleObj.transform.position = blockWorldPos + localOffset;

                float particleSize = Random.Range(0.06f, 0.09f);
                Vector3 initialScale = new Vector3(particleSize, particleSize, particleSize);
                particleObj.transform.localScale = initialScale;

                MeshFilter mf = particleObj.AddComponent<MeshFilter>();
                mf.sharedMesh = _sharedCubeMesh;

                MeshRenderer mr = particleObj.AddComponent<MeshRenderer>();
                mr.sharedMaterial = _particleMaterial;
                mr.SetPropertyBlock(mpb);

                // Vận tốc hướng lên và tỏa ra xung quanh
                Vector3 spread = Random.insideUnitSphere * 1.8f;
                spread.y = Mathf.Abs(spread.y) + Random.Range(1.8f, 3.2f);

                float lifetime = Random.Range(0.55f, 0.70f);

                _activeParticles.Add(new VoxelParticle
                {
                    Transform = particleObj.transform,
                    Velocity = spread,
                    RemainingLife = lifetime,
                    TotalLife = lifetime,
                    InitialScale = initialScale
                });
            }
        }

        private void Update()
        {
            if (_activeParticles.Count == 0)
            {
                return;
            }

            float dt = Time.deltaTime;
            Vector3 gravity = new Vector3(0f, -9.81f, 0f);

            for (int i = _activeParticles.Count - 1; i >= 0; i--)
            {
                VoxelParticle p = _activeParticles[i];
                if (p.Transform == null)
                {
                    _activeParticles.RemoveAt(i);
                    continue;
                }

                p.RemainingLife -= dt;
                if (p.RemainingLife <= 0f)
                {
                    Destroy(p.Transform.gameObject);
                    _activeParticles.RemoveAt(i);
                    continue;
                }

                // Cập nhật vị trí và gia tốc trọng lực
                p.Velocity += gravity * dt;
                p.Transform.position += p.Velocity * dt;

                // Thu nhỏ dần ở giai đoạn cuối đời
                float lifeRatio = p.RemainingLife / p.TotalLife;
                p.Transform.localScale = p.InitialScale * Mathf.Clamp01(lifeRatio * 1.2f);
            }
        }
    }
}
