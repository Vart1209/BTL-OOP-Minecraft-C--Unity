using System;
using UnityEngine;
using Minecraft.Application.World;
using Minecraft.Application.Survival;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Entities;
using Minecraft.Domain.World;

namespace Minecraft.Presentation.Player
{
    /// <summary>
    /// Điều khiển các tương tác nhắm tia, phá block, đặt block và ăn thức ăn của người chơi (Presentation Layer).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc phân tầng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Ủy quyền nghiệp vụ (Delegation):</b> Controller chỉ nhận tín hiệu chuột từ người dùng và ủy quyền xử lý nghiệp vụ cho <see cref="BlockInteractionService"/> và <see cref="SurvivalService"/>.</item>
    ///   <item><b>Quy tắc R7:</b> Sử dụng <see cref="VoxelRaycaster"/> với tầm với tối đa 5 block.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class PlayerInteractionController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _reachDistance = 5.0f;
        [SerializeField] private Transform _cameraTransform;

        private Domain.Entities.Player _player;
        private VoxelRaycaster _raycaster;
        private BlockInteractionService _blockInteractionService;
        private SurvivalService _survivalService;

        private void Awake()
        {
            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        /// <summary>
        /// Khởi tạo phụ thuộc của Controller từ GameBootstrap.
        /// </summary>
        public void Initialize(
            Domain.Entities.Player player,
            VoxelRaycaster raycaster,
            BlockInteractionService blockInteractionService,
            SurvivalService survivalService)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _raycaster = raycaster ?? throw new ArgumentNullException(nameof(raycaster));
            _blockInteractionService = blockInteractionService ?? throw new ArgumentNullException(nameof(blockInteractionService));
            _survivalService = survivalService ?? throw new ArgumentNullException(nameof(survivalService));
        }

        private void Update()
        {
            if (_player == null || !_player.IsAlive || _cameraTransform == null)
            {
                return;
            }

            // Đồng bộ tọa độ Position trong Domain với vị trí thực tế trong Unity
            _player.MoveTo(new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z));

            // Dò tia từ Camera nhìn ra trước
            System.Numerics.Vector3 rayOrigin = new System.Numerics.Vector3(_cameraTransform.position.x, _cameraTransform.position.y, _cameraTransform.position.z);
            System.Numerics.Vector3 rayDir = new System.Numerics.Vector3(_cameraTransform.forward.x, _cameraTransform.forward.y, _cameraTransform.forward.z);

            VoxelRaycastHit hit = _raycaster.Raycast(rayOrigin, rayDir, _reachDistance);

            // Chuột trái: Phá khối (Use Case UC03)
            if (Input.GetMouseButtonDown(0) && hit.IsHit)
            {
                _blockInteractionService.TryBreakBlock(hit.BlockPosition, _player.Inventory, _player.CurrentGameMode);
            }

            // Chuột phải: Ăn thức ăn (UC10) hoặc Đặt khối (UC04)
            if (Input.GetMouseButtonDown(1))
            {
                // Ưu tiên ăn thức ăn nếu đang cầm FoodItem
                bool ateFood = _survivalService.TryEatSelectedFood(_player);
                if (!ateFood && hit.IsHit)
                {
                    BlockPos playerFootPos = new BlockPos(
                        (int)Math.Floor(transform.position.x),
                        (int)Math.Floor(transform.position.y),
                        (int)Math.Floor(transform.position.z));
                    BlockPos playerHeadPos = playerFootPos.Offset(0, 1, 0);

                    _blockInteractionService.TryPlaceBlock(hit.AdjacentPosition, playerFootPos, playerHeadPos, _player.Inventory, _player.CurrentGameMode);
                }
            }
        }
    }
}
