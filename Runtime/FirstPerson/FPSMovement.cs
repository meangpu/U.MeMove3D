using UnityEngine;

namespace Meangpu.Move3D.FPS
{
    public class FPSMovement : MonoBehaviour
    {
        [SerializeField] Vector3Reference _moveInput;
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float mouseSensitivity = 2f;
        [SerializeField] float jumpForce = 5f;
        [SerializeField] float gravity = -9.81f;

        [SerializeField] Transform playerCamera;
        [SerializeField] CharacterController controller;

        private float verticalRotation = 0f;
        private Vector3 playerVelocity;
        private bool isGrounded;

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

        private void Start() => LockCursor();

        private void Update()
        {
            isGrounded = controller.isGrounded;

            Vector3 movement = transform.right * _moveInput.Value.x + transform.forward * _moveInput.Value.z;
            controller.Move(movement * moveSpeed * Time.deltaTime);

            // Handle player rotation (looking around)
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

            playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);

            // Handle jumping and gravity
            if (isGrounded && playerVelocity.y < 0)
            {
                playerVelocity.y = -2f;
            }

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }

            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }
}
