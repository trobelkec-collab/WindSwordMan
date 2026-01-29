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



        [Header("Combat Settings")]
        [SerializeField] private float comboWindow = 1.0f;
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackDuration = 0.4f; // Time to block movement
        [SerializeField] private Combat.Weapon weapon; // Reference to Weapon script

        // State
        private int _comboStep = 0;
        private float _lastAttackTime = -99f;
        private bool _isAttacking = false;
        private float _attackEndTime = 0f;

        private InputAction _attackAction;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            
            // Setup Input Actions
            _moveAction = _playerInput.actions["Move"];
            _jumpAction = _playerInput.actions["Jump"];
            
            // Try 'Attack' first, then 'Fire' (default)
            _attackAction = _playerInput.actions.FindAction("Attack");
            if (_attackAction == null)
                _attackAction = _playerInput.actions.FindAction("Fire");
        }

        private void Update()
        {
            HandleCombatState(); // Check if attack animation finished
            HandleGravity();
            HandleMovement();
            HandleJump();
            HandleAttackInput();
        }

        private void HandleCombatState()
        {
            if (_isAttacking && Time.time >= _attackEndTime)
            {
                _isAttacking = false;
            }

            // Reset combo if window passed
            if (_comboStep > 0 && Time.time > _lastAttackTime + comboWindow)
            {
                _comboStep = 0;
                Debug.Log("Combo Reset (Timeout)");
            }
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
            if (_moveAction == null) return;
            if (_isAttacking) return; // Block movement during attack

            Vector2 input = _moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, input.y);

            // World space movement (Top-Down, assuming camera doesn't rotate Y significantly)
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
            if (_jumpAction == null) return;
            if (_isAttacking) return; // Block jump during attack

            if (_jumpAction.triggered && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Apply gravity
            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void HandleAttackInput()
        {
            if (_attackAction == null) return;

            // Attack triggered
            bool debugAttack = Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame;
            if ((_attackAction != null && _attackAction.triggered) || debugAttack)
            {
                // Check cooldown (from last attack start)
                if (Time.time < _lastAttackTime + attackCooldown) return;
                
                // Allow attack if not currently attacking (or can chain?)
                // Simple implementation: wait for cooldown which is slightly longer than duration usually?
                // Or allow chaining before duration ends for smoothness? 
                // Let's rely on Cooldown.

                StartCoroutine(PerformAttackRoutine());
            }
        }

        private System.Collections.IEnumerator PerformAttackRoutine()
        {
             _comboStep++;
            if (_comboStep > 3) _comboStep = 1; // 3-hit combo loop

            _lastAttackTime = Time.time;
            _isAttacking = true;
            _attackEndTime = Time.time + attackDuration;

            Debug.Log($"Attack Combo {_comboStep}!");

            // Enable Hitbox & Visuals (Simulating Animation Event)
            if (weapon != null) 
            {
                // Play visual swing and handle hitbox internally in Weapon script
                weapon.Swing(0.3f); 
                
                // Wait for swing to finish
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                Debug.LogWarning("PlayerController: No Weapon Assigned!");
                 yield return null;
            }
        }
    }
}
