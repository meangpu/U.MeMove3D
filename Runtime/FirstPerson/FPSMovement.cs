using UnityEngine;

namespace Meangpu.Move3D.FPS
{
    public class FPSMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float mouseSensitivity = 2f;
        public float jumpForce = 5f;
        public float gravity = -9.81f;

        public Transform playerCamera;
        public CharacterController controller;

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

            // Handle player movement
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
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
