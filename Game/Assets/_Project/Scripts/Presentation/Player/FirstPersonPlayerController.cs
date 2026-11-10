using System;
using UnityEngine;
using Minecraft.Domain.Contracts;
using Minecraft.Domain.Items;

namespace Minecraft.Presentation.Player
{
    /// <summary>
    /// Điều khiển di chuyển nhân vật góc nhìn thứ nhất (FPS Controller) tích hợp quyền theo GameMode và túi đồ.
    /// <para>
    /// <b>Nguyên lý OOP &amp; Kiến trúc phân tầng (dùng khi vấn đáp):</b>
    /// <list type="bullet">
    ///   <item><b>Phân quyền tác nhân (Tiêu chí 4):</b> Điều chỉnh cơ chế di chuyển (bay tự do khi <see cref="IGameMode.CanFly"/> ở Creative, hoặc chịu trọng lực và nhảy ở Survival).</item>
    ///   <item><b>Dependency Injection (Tiêu chí 2):</b> Nhận tham chiếu <see cref="IGameMode"/> và <see cref="Inventory"/> từ tầng ngoài thông qua phương thức <see cref="Initialize"/>.</item>
    ///   <item><b>Lựa chọn Hotbar:</b> Bắt sự kiện phím 1..9 và con lăn chuột để đồng bộ ô vật phẩm cầm trên tay.</item>
    /// </list>
    /// </para>
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonPlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 4.5f;
        [SerializeField] private float _jumpForce = 6.0f;
        [SerializeField] private float _flySpeed = 10.0f;
        [SerializeField] private float _gravity = -9.81f;

        [Header("Camera Look Settings")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _mouseSensitivity = 2.0f;

        private CharacterController _characterController;
        private IGameMode _gameMode;
        private Inventory _inventory;

        private float _verticalVelocity;
        private float _cameraPitch;
        private bool _isUiOpen;

        /// <summary>Trạng thái đang mở bảng UI (Chế tạo / Túi đồ) để tạm dừng xoay chuột FPS.</summary>
        public bool IsUiOpen
        {
            get => _isUiOpen;
            set
            {
                _isUiOpen = value;
                UpdateCursorState();
            }
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        private void Start()
        {
            UpdateCursorState();
        }

        /// <summary>
        /// Khởi tạo phụ thuộc của Player từ Bootstrap.
        /// </summary>
        public void Initialize(IGameMode gameMode, Inventory inventory)
        {
            _gameMode = gameMode ?? throw new ArgumentNullException(nameof(gameMode));
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                IsUiOpen = !_isUiOpen;
            }

            if (!_isUiOpen)
            {
                HandleMouseLook();
            }

            HandleMovement();
            HandleHotbarInput();
        }

        private void UpdateCursorState()
        {
            Cursor.lockState = _isUiOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _isUiOpen;
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            // Xoay thân nhân vật theo trục Y (ngang)
            transform.Rotate(Vector3.up * mouseX);

            // Gật/ngửa camera theo trục X (dọc), giới hạn góc nhìn [-89..89] độ
            _cameraPitch -= mouseY;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -89f, 89f);

            if (_cameraTransform != null)
            {
                _cameraTransform.localEulerAngles = new Vector3(_cameraPitch, 0f, 0f);
            }
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxis("Horizontal"); // A, D
            float vertical = Input.GetAxis("Vertical");     // W, S

            Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

            // Kiểm tra phân quyền bay (Creative) hay đi bộ bình thường (Survival)
            if (_gameMode != null && _gameMode.CanFly)
            {
                float verticalFly = 0f;
                if (Input.GetKey(KeyCode.Space)) verticalFly += 1f;
                if (Input.GetKey(KeyCode.LeftShift)) verticalFly -= 1f;

                Vector3 flyVelocity = (moveDirection * _walkSpeed) + (Vector3.up * verticalFly * _flySpeed);
                _characterController.Move(flyVelocity * Time.deltaTime);
                _verticalVelocity = 0f;
            }
            else
            {
                if (_characterController.isGrounded)
                {
                    _verticalVelocity = -0.5f; // Giữ áp lực bám sàn
                    if (Input.GetButtonDown("Jump"))
                    {
                        _verticalVelocity = _jumpForce;
                    }
                }
                else
                {
                    _verticalVelocity += _gravity * Time.deltaTime;
                }

                Vector3 currentMove = (moveDirection * _walkSpeed) + (Vector3.up * _verticalVelocity);
                _characterController.Move(currentMove * Time.deltaTime);
            }
        }

        private void HandleHotbarInput()
        {
            if (_inventory == null)
            {
                return;
            }

            // Chọn ô 1..9 qua phím số
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    _inventory.SelectHotbarSlot(i);
                    return;
                }
            }

            // Chọn ô qua con lăn chuột
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                int current = _inventory.SelectedHotbarIndex;
                if (scroll > 0f)
                {
                    current = (current - 1 + 9) % 9;
                }
                else
                {
                    current = (current + 1) % 9;
                }

                _inventory.SelectHotbarSlot(current);
            }
        }
    }
}

