using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 1.5f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 10f;

        // Components
        private CharacterController _characterController;
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _jumpAction;

        // State
        private Vector3 _velocity;
        private bool _isGrounded;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            
            // Setup Input Actions
            // Note: Assuming 'Player' map and 'Move', 'Jump' actions exist in your InputActions asset
            _moveAction = _playerInput.actions["Move"];
            _jumpAction = _playerInput.actions["Jump"];
        }

        private void Update()
        {
            HandleGravity();
            HandleMovement();
            HandleJump();
        }

        private void HandleGravity()
        {
            _isGrounded = _characterController.isGrounded;

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f; // Ensure we stay grounded
            }
        }

        private void HandleMovement()
        {
            Vector2 input = _moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, input.y);

            // Convert to world space relative to camera if needed, or just world space
            // For now, let's assume Top-Down where Up is World Forward
            // If camera rotates, we need to transform direction based on camera Y rotation

             if (move != Vector3.zero)
            {
                // Move the character
                _characterController.Move(move * (moveSpeed * Time.deltaTime));

                // Rotate character towards move direction
                Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private void HandleJump()
        {
            if (_jumpAction.triggered && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Apply gravity
            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}
