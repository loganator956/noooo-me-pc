using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CharacterSystems.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement3D : MonoBehaviour
    {
        [Header("Movement Properties")]
        public float WalkSpeed = 3.0f;
        public float SprintSpeed = 6.0f;

        public float JumpForce = 10f;
        public float JumpForceDegradation = 10f;

        [Header("Physics Properties")]
        public float GravityAcceleration = 9.8f;
        public float TerminalFallVelocity = 10f;

        [Header("Detection Properties")]
        public float CharacterHeight = 1.0f;

        [Header("Events")]
        public UnityEvent<bool> IsMovingChangedEvent, OnIsSprintingChanged, OnIsGroundedChanged;

        private Vector3 _cameraForward = Vector3.forward;
        public Vector3 CameraForward
        {
            get { return _cameraForward; }
            set { _cameraForward = new Vector3(value.x, 0f, value.z).normalized; }
        }

        private bool _isMoving;
        public bool IsMoving
        {
            get { return _isMoving; }
            private set
            {
                _isMoving = value;
                IsMovingChangedEvent.Invoke(IsMoving);
            }
        }

        private bool _isSprinting;
        public bool IsSprinting
        {
            get { return _isSprinting; }
            private set
            {
                _isSprinting = value;
                OnIsSprintingChanged.Invoke(_isSprinting);
            }
        }

        private bool _isGrounded;
        public bool IsGrounded
        {
            get { return _isGrounded; }
            private set
            {
                _isGrounded = value;
            }
        }

        private bool _isJumpKeyPressed;

        private CharacterController _charController;

        public Vector3 CurrentVelocity { get; private set; }

        private void Awake()
        {
            _charController = GetComponent<CharacterController>();
        }

        private float _currentJumpForce = 0f;

        #region User Input
        private Vector2 _inputs;
        public void Move(Vector2 inputs)
        {
            _inputs = inputs.normalized;
        }

        public void OnMove(InputValue value)
        {
            Move(value.Get<Vector2>());
        }

        public void SetSprint(bool isSprint)
        {
            _isSprinting = isSprint;
        }

        public void OnSprint(InputValue value)
        {
            SetSprint(value.Get<float>() == 1f);
        }

        private void SetJump(bool isJump)
        {
            _isJumpKeyPressed = isJump;
            if (CheckCanJump() && isJump)
            {
                _currentJumpForce = JumpForce;
            }
        }

        public void OnJump(InputValue value)
        {
            SetJump(value.Get<float>() == 1f);
        }
        #endregion

        private void Update()
        {
            // ground detection
            IsGrounded = Physics.Raycast(transform.position, Vector3.down, CharacterHeight);

            // horizontal walking
            Vector3 moveVector = InputToWorldDirection(_inputs);
            moveVector *= IsSprinting ? SprintSpeed : WalkSpeed;

            moveVector.y = CurrentVelocity.y;

            // gravity
            if (!IsGrounded)
            {
                moveVector.y -= GravityAcceleration * Time.deltaTime;
                moveVector.y = Mathf.Clamp(moveVector.y, -TerminalFallVelocity, TerminalFallVelocity);
            }
            else
            {
                moveVector.y = 0f;
            }

            // jumping
            if (_currentJumpForce > 0f)
            {
                if (!_isJumpKeyPressed)
                {
                    // cancel jump, take away much quicker
                    _currentJumpForce -= JumpForceDegradation * 2 * Time.deltaTime;
                }
                else
                {
                    _currentJumpForce -= JumpForceDegradation * Time.deltaTime;
                }
                moveVector.y = _currentJumpForce;
            }

            CurrentVelocity = moveVector;
        }

        private void FixedUpdate()
        {
            _charController.Move(CurrentVelocity * Time.fixedDeltaTime);
        }

        private bool CheckCanJump()
        {
            // TODO: Could add multi-jumps here, or something
            return IsGrounded;
        }

        private Vector3 InputToWorldDirection(Vector2 input)
        {
            Vector3 forward = CameraForward * input.y;
            Vector3 right = Vector3.Cross(Vector3.up, CameraForward) * input.x;
            return forward + right;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue; // z
            Gizmos.DrawLine(transform.position, transform.position + InputToWorldDirection(Vector2.up));
            Gizmos.color = Color.red; // x
            Gizmos.DrawLine(transform.position, transform.position + InputToWorldDirection(Vector2.right));
            Gizmos.color = Color.green; // y
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + InputToWorldDirection(_inputs), 0.1f);

            Gizmos.DrawSphere(transform.position + Vector3.down * CharacterHeight, 0.2f);
        }
    }
}
