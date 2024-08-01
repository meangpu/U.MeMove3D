using UnityEngine;

namespace Meangpu.Move3D.FPS
{
    public class FPSMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] float _moveSpeed;
        [SerializeField] float _groundDrag;

        [SerializeField] float _jumpForce;
        [SerializeField] float _jumpCooldown;
        [SerializeField] float _airMultiplier;
        bool _readyToJump;

        [Header("KeyBinds")]
        [SerializeField] KeyCode _jumpKey = KeyCode.Space;

        [Header("Ground Check")]
        [SerializeField] float _playerHeight;
        [SerializeField] LayerMask _whatIsGround;
        bool _grounded;

        [SerializeField] Transform _orientation;

        float _horizontalInput;
        float _verticalInput;
        Vector3 _moveDirection;
        Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _readyToJump = true;
        }

        private void Update()
        {
            _grounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.3f, _whatIsGround);

            GetInput();
            ControlSpeed();

            if (_grounded)
                _rb.linearDamping = _groundDrag;
            else
                _rb.linearDamping = 0;
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
            _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

            if (_grounded)
                _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);

            else if (!_grounded)
                _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * _airMultiplier, ForceMode.Force);
        }

        private void ControlSpeed()
        {
            Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            // limit velocity
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