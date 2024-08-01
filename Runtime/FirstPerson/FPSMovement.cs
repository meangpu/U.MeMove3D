using UnityEngine;

namespace Meangpu.Move3D.FPS
{
    public class FPSMovement : MonoBehaviour
    {
        /// <summary>
        /// learn from https://www.youtube.com/watch?v=f473C43s8nE
        ///
        /// stick this script to main parent player transform
        /// </summary>

        [Header("Mouse Input")]
        [SerializeField] float _mouseSensitivity = 200f;
        [SerializeField] Vector2 _rotationClamp = new(-60, 60);
        private float _rotationX = 0f;
        private float _rotationY = 0f;

        [Header("Movement")]
        [SerializeField] float _moveSpeed = 10;
        [SerializeField] float _groundDrag = 5;

        [SerializeField] float _jumpForce;
        [SerializeField] float _jumpCooldown;
        [SerializeField] float _airMultiplier;
        bool _readyToJump;

        [Header("KeyBinds")]
        [SerializeField] KeyCode _jumpKey = KeyCode.Space;

        [Header("Ground Check")]
        [SerializeField] float _playerHeight = 10;
        [SerializeField] LayerMask _whatIsGround;
        bool _grounded;

        float _horizontalInput;
        float _verticalInput;
        Vector3 _moveDirection;
        Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _readyToJump = true;
            LockCursor();
        }


        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnLockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            _grounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.3f, _whatIsGround);

            GetInput();
            ControlSpeed();
            RotateCamera();

            if (_grounded)
                _rb.linearDamping = _groundDrag;
            else
                _rb.linearDamping = 0;
        }

        private void RotateCamera()
        {
            _rotationX += Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
            _rotationY += Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;
            _rotationY = Mathf.Clamp(_rotationY, _rotationClamp.x, _rotationClamp.y);
            transform.rotation = Quaternion.Euler(_rotationY, _rotationX, 0f);
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void GetInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKey(_jumpKey) && _readyToJump && _grounded)
            {
                _readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), _jumpCooldown);
            }
        }

        private void MovePlayer()
        {
            _moveDirection = transform.forward * _verticalInput + transform.right * _horizontalInput;
            if (_grounded)
                _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
            else if (!_grounded)
                _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * _airMultiplier, ForceMode.Force);
        }

        private void ControlSpeed()
        {
            Vector3 flatVel = new(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                _rb.linearVelocity = new Vector3(limitedVel.x, _rb.linearVelocity.y, limitedVel.z);
            }
        }

        private void Jump()
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        }

        private void ResetJump() => _readyToJump = true;
    }
}