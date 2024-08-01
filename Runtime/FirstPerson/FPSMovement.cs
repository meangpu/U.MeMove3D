using UnityEngine;

namespace Meangpu.Move3D.FPS
{
    public class FPSMovement : MonoBehaviour
    {
        [Header("Mouse Input")]
        [SerializeField] float mouseSensitivity = 2f;
        [SerializeField] Vector2 verticalRotationClamp = new(-80, 80);
        private float rotationX = 0f;
        private float rotationY = 0f;

        [SerializeField] Transform cameraTransform;

        [Header("Movement")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float sprintMultiplier = 1.5f;
        [SerializeField] float crouchMultiplier = 0.5f;
        [SerializeField] float groundDrag = 5f;
        [SerializeField] float jumpForce = 5f;
        [SerializeField] float jumpCooldown = 0.25f;
        [SerializeField] float airMultiplier = 0.5f;
        private bool readyToJump = true;

        [Header("KeyBinds")]
        [SerializeField] KeyCode jumpKey = KeyCode.Space;
        [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;

        [Header("Ground Check")]
        [SerializeField] float playerHeight = 2f;
        [SerializeField] LayerMask whatIsGround;
        private bool isGrounded;

        [Header("Slope Handling")]
        [SerializeField] float maxSlopeAngle = 45f;
        private RaycastHit slopeHit;
        private bool exitingSlope;

        private float horizontalInput;
        private float verticalInput;
        private Vector3 moveDirection;
        private Rigidbody rb;

        private bool isSprinting = false;
        private bool isCrouching = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            readyToJump = true;
            LockCursor();

            if (cameraTransform == null)
                cameraTransform = Camera.main.transform;
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

            GetInput();
            ControlDrag();
            ControlSpeed();
            RotateCamera();

            if (Input.GetKeyDown(jumpKey) && readyToJump && isGrounded)
                Jump();

            if (Input.GetKeyDown(sprintKey))
                isSprinting = true;
            if (Input.GetKeyUp(sprintKey))
                isSprinting = false;

            if (Input.GetKeyDown(crouchKey))
                isCrouching = true;
            if (Input.GetKeyUp(crouchKey))
                isCrouching = false;
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void GetInput()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }

        private void RotateCamera()
        {
            rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
            rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationY = Mathf.Clamp(rotationY, verticalRotationClamp.x, verticalRotationClamp.y);

            transform.rotation = Quaternion.Euler(0, rotationX, 0);
            cameraTransform.localRotation = Quaternion.Euler(rotationY, 0, 0);
        }

        private void MovePlayer()
        {
            moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

                if (rb.linearVelocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else if (isGrounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            else if (!isGrounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

            rb.useGravity = !OnSlope();
        }

        private void ControlSpeed()
        {
            float targetSpeed = moveSpeed;

            if (isSprinting)
                targetSpeed *= sprintMultiplier;
            else if (isCrouching)
                targetSpeed *= crouchMultiplier;

            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > targetSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * targetSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

        private void ControlDrag()
        {
            if (isGrounded)
                rb.linearDamping = groundDrag;
            else
                rb.linearDamping = 0;
        }

        private void Jump()
        {
            exitingSlope = true;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        private void ResetJump()
        {
            readyToJump = true;
            exitingSlope = false;
        }

        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        private Vector3 GetSlopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
        }
    }
}