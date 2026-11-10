using System;
using UnityEngine;
using Minecraft.Application.World;
using Minecraft.Application.Survival;
using Minecraft.Domain.Blocks;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Entities;
using Minecraft.Domain.Entities.Mobs;
using Minecraft.Domain.Items;
using Minecraft.Domain.World;
using Minecraft.Presentation.Mobs;

namespace Minecraft.Presentation.Player
{
    /// <summary>
    /// Điều khiển các tương tác nhắm tia, khung viền chọn khối 3D, tay cầm vật phẩm góc nhìn thứ nhất, phá/đặt block, đánh Mob và ăn thức ăn (Presentation Layer).
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc phân tầng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Ủy quyền nghiệp vụ (Delegation):</b> Controller chỉ nhận tín hiệu chuột từ người dùng và ủy quyền xử lý nghiệp vụ cho <see cref="BlockInteractionService"/>, <see cref="MobCombatService"/> và <see cref="SurvivalService"/>.</item>
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
        private MobCombatService _combatService;
        private ItemRegistry _itemRegistry;
        private FirstPersonPlayerController _fpsController;

        private VoxelRaycastHit _currentHit;
        private LineRenderer _highlightLine;
        private Transform _handPivot;
        private Renderer _heldItemRenderer;
        private float _swingTimer;

        /// <summary>Kết quả dò tia Voxel hiện tại để HUD hiển thị thông tin khối đang nhắm.</summary>
        public VoxelRaycastHit CurrentHit => _currentHit;

        private void Awake()
        {
            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }

            _fpsController = GetComponent<FirstPersonPlayerController>();
            CreateBlockHighlightWireframe();
            CreateFirstPersonHeldItemVisual();
        }

        /// <summary>
        /// Khởi tạo phụ thuộc của Controller từ GameBootstrap.
        /// </summary>
        public void Initialize(
            Domain.Entities.Player player,
            VoxelRaycaster raycaster,
            BlockInteractionService blockInteractionService,
            SurvivalService survivalService,
            MobCombatService combatService = null,
            ItemRegistry itemRegistry = null)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _raycaster = raycaster ?? throw new ArgumentNullException(nameof(raycaster));
            _blockInteractionService = blockInteractionService ?? throw new ArgumentNullException(nameof(blockInteractionService));
            _survivalService = survivalService ?? throw new ArgumentNullException(nameof(survivalService));
            _combatService = combatService;
            _itemRegistry = itemRegistry;
        }

        private void CreateBlockHighlightWireframe()
        {
            GameObject highlightGo = new GameObject("VoxelBlockSelectionWireframe");
            _highlightLine = highlightGo.AddComponent<LineRenderer>();
            _highlightLine.useWorldSpace = true;
            _highlightLine.loop = false;
            _highlightLine.startWidth = 0.025f;
            _highlightLine.endWidth = 0.025f;
            _highlightLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _highlightLine.receiveShadows = false;

            Shader lineShader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color") ?? Shader.Find("Standard");
            Material lineMat = new Material(lineShader)
            {
                color = new Color(0.05f, 0.05f, 0.05f, 0.9f)
            };
            _highlightLine.sharedMaterial = lineMat;
            _highlightLine.enabled = false;
        }

        private void CreateFirstPersonHeldItemVisual()
        {
            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }

            if (_cameraTransform == null)
            {
                return;
            }

            GameObject pivotGo = new GameObject("FirstPersonHandPivot");
            pivotGo.transform.SetParent(_cameraTransform, worldPositionStays: false);
            pivotGo.transform.localPosition = new Vector3(0.46f, -0.36f, 0.65f);
            pivotGo.transform.localRotation = Quaternion.Euler(15f, -35f, 10f);
            _handPivot = pivotGo.transform;

            GameObject itemCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            itemCube.name = "HeldItemMesh";
            itemCube.transform.SetParent(_handPivot, worldPositionStays: false);
            itemCube.transform.localPosition = Vector3.zero;
            itemCube.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);

            Collider col = itemCube.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }

            _heldItemRenderer = itemCube.GetComponent<Renderer>();
            if (_heldItemRenderer != null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                Material mat = new Material(shader) { color = new Color(0.88f, 0.72f, 0.56f) };
                if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
                if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0f);
                _heldItemRenderer.sharedMaterial = mat;
                _heldItemRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        private void Update()
        {
            if (_player == null || !_player.IsAlive)
            {
                return;
            }

            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
                if (_handPivot == null)
                {
                    CreateFirstPersonHeldItemVisual();
                }
            }

            if (_cameraTransform == null)
            {
                return;
            }

            // Đồng bộ tọa độ Position trong Domain với vị trí thực tế trong Unity
            _player.MoveTo(new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z));

            // Cập nhật hiển thị vật phẩm trên tay phải và hiệu ứng vung tay
            UpdateHeldItemVisual();

            // Dò tia từ Camera nhìn ra trước
            System.Numerics.Vector3 rayOrigin = new System.Numerics.Vector3(_cameraTransform.position.x, _cameraTransform.position.y, _cameraTransform.position.z);
            System.Numerics.Vector3 rayDir = new System.Numerics.Vector3(_cameraTransform.forward.x, _cameraTransform.forward.y, _cameraTransform.forward.z);

            _currentHit = _raycaster.Raycast(rayOrigin, rayDir, _reachDistance);
            UpdateHighlightWireframe(_currentHit);

            // Không thực hiện đập/đặt khối khi đang mở bảng UI Chế tạo
            if (_fpsController != null && _fpsController.IsUiOpen)
            {
                return;
            }

            // Chuột trái: Đánh sinh vật (UC08) hoặc Phá khối (UC03)
            if (Input.GetMouseButtonDown(0))
            {
                _swingTimer = 0.22f;

                if (TryAttackTargetedMob())
                {
                    return;
                }

                if (_currentHit.IsHit)
                {
                    _blockInteractionService.TryBreakBlock(_currentHit.BlockPosition, _player.Inventory, _player.CurrentGameMode);
                }
            }

            // Chuột phải: Ăn thức ăn (UC10) hoặc Đặt khối (UC04)
            if (Input.GetMouseButtonDown(1))
            {
                _swingTimer = 0.22f;

                bool ateFood = _survivalService.TryEatSelectedFood(_player);
                if (!ateFood && _currentHit.IsHit)
                {
                    BlockPos playerFootPos = new BlockPos(
                        (int)Math.Floor(transform.position.x),
                        (int)Math.Floor(transform.position.y),
                        (int)Math.Floor(transform.position.z));
                    BlockPos playerHeadPos = playerFootPos.Offset(0, 1, 0);

                    _blockInteractionService.TryPlaceBlock(_currentHit.AdjacentPosition, playerFootPos, playerHeadPos, _player.Inventory, _player.CurrentGameMode);
                }
            }
        }

        private bool TryAttackTargetedMob()
        {
            if (_combatService == null || _cameraTransform == null)
            {
                return false;
            }

            Ray unityRay = new Ray(_cameraTransform.position, _cameraTransform.forward);
            if (Physics.Raycast(unityRay, out RaycastHit hitInfo, 3.5f))
            {
                MobView mobView = hitInfo.collider.GetComponentInParent<MobView>();
                if (mobView != null && mobView.MobModel != null && mobView.MobModel.IsAlive)
                {
                    int damage = 2;
                    ItemStack held = _player.Inventory.GetSlot(_player.Inventory.SelectedHotbarIndex);
                    if (!held.IsEmpty && held.Definition is ToolItem tool)
                    {
                        damage = tool.AttackDamage;
                    }

                    bool attacked = _combatService.TryPlayerAttackMob(_player, mobView.MobModel, damage);
                    if (attacked && !mobView.MobModel.IsAlive && mobView.MobModel is Cow && _itemRegistry != null)
                    {
                        _player.Inventory.AddItem(_itemRegistry.Get(ItemId.RawBeef), 2);
                    }

                    return attacked;
                }
            }

            return false;
        }

        private void UpdateHighlightWireframe(VoxelRaycastHit hit)
        {
            if (_highlightLine == null)
            {
                return;
            }

            if (!hit.IsHit)
            {
                _highlightLine.enabled = false;
                return;
            }

            _highlightLine.enabled = true;
            const float pad = 0.004f;
            float x0 = hit.BlockPosition.X - pad;
            float y0 = hit.BlockPosition.Y - pad;
            float z0 = hit.BlockPosition.Z - pad;
            float x1 = hit.BlockPosition.X + 1f + pad;
            float y1 = hit.BlockPosition.Y + 1f + pad;
            float z1 = hit.BlockPosition.Z + 1f + pad;

            Vector3[] pts =
            {
                new Vector3(x0, y0, z0), new Vector3(x1, y0, z0), new Vector3(x1, y0, z1), new Vector3(x0, y0, z1), new Vector3(x0, y0, z0),
                new Vector3(x0, y1, z0), new Vector3(x1, y1, z0), new Vector3(x1, y0, z0), new Vector3(x1, y1, z0),
                new Vector3(x1, y1, z1), new Vector3(x1, y0, z1), new Vector3(x1, y1, z1),
                new Vector3(x0, y1, z1), new Vector3(x0, y0, z1), new Vector3(x0, y1, z1), new Vector3(x0, y1, z0)
            };

            _highlightLine.positionCount = pts.Length;
            _highlightLine.SetPositions(pts);
        }

        private void UpdateHeldItemVisual()
        {
            if (_handPivot == null || _heldItemRenderer == null || _player == null)
            {
                return;
            }

            ItemStack selected = _player.Inventory.GetSlot(_player.Inventory.SelectedHotbarIndex);
            if (selected.IsEmpty)
            {
                // Hiển thị cánh tay nhân vật khi ô Hotbar trống
                _heldItemRenderer.transform.localScale = new Vector3(0.16f, 0.18f, 0.42f);
                SetHeldColor(new Color(0.88f, 0.70f, 0.56f));
            }
            else if (selected.Definition is ToolItem)
            {
                // Hiển thị hình dáng công cụ/vũ khí thon dài
                _heldItemRenderer.transform.localScale = new Vector3(0.10f, 0.38f, 0.12f);
                SetHeldColor(GetItemPreviewColor(selected.Definition.Id));
            }
            else
            {
                // Hiển thị khối 3D thu nhỏ trên tay
                _heldItemRenderer.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);
                SetHeldColor(GetItemPreviewColor(selected.Definition.Id));
            }

            if (_swingTimer > 0f)
            {
                _swingTimer -= Time.deltaTime;
                float phase = Mathf.Sin((_swingTimer / 0.22f) * Mathf.PI);
                _handPivot.localRotation = Quaternion.Euler(15f + phase * 48f, -35f - phase * 25f, 10f);
                _handPivot.localPosition = new Vector3(0.46f - phase * 0.08f, -0.36f + phase * 0.06f, 0.65f + phase * 0.08f);
            }
            else
            {
                float idleBob = Mathf.Sin(Time.time * 2.5f) * 0.012f;
                _handPivot.localRotation = Quaternion.Euler(15f, -35f, 10f);
                _handPivot.localPosition = new Vector3(0.46f, -0.36f + idleBob, 0.65f);
            }
        }

        private void SetHeldColor(Color c)
        {
            if (_heldItemRenderer != null && _heldItemRenderer.sharedMaterial != null)
            {
                _heldItemRenderer.sharedMaterial.color = c;
                if (_heldItemRenderer.sharedMaterial.HasProperty("_BaseColor"))
                {
                    _heldItemRenderer.sharedMaterial.SetColor("_BaseColor", c);
                }
            }
        }

        private static Color GetItemPreviewColor(ItemId id)
        {
            switch (id)
            {
                case ItemId.Grass: return new Color(0.36f, 0.72f, 0.24f);
                case ItemId.Dirt: return new Color(0.54f, 0.37f, 0.23f);
                case ItemId.Stone:
                case ItemId.Cobblestone: return new Color(0.52f, 0.53f, 0.55f);
                case ItemId.Wood: return new Color(0.46f, 0.32f, 0.18f);
                case ItemId.Planks:
                case ItemId.CraftingTable: return new Color(0.68f, 0.52f, 0.32f);
                case ItemId.Sand: return new Color(0.88f, 0.84f, 0.58f);
                case ItemId.Bricks: return new Color(0.72f, 0.32f, 0.24f);
                case ItemId.Glass: return new Color(0.78f, 0.94f, 1.0f);
                case ItemId.Glowstone: return new Color(0.98f, 0.86f, 0.35f);
                case ItemId.DiamondOre:
                case ItemId.DiamondPickaxe: return new Color(0.25f, 0.88f, 0.92f);
                case ItemId.GoldOre: return new Color(0.95f, 0.80f, 0.25f);
                case ItemId.Apple:
                case ItemId.RawBeef: return new Color(0.88f, 0.22f, 0.20f);
                default: return new Color(0.65f, 0.55f, 0.42f);
            }
        }
    }
}

