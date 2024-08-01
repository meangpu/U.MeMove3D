using UnityEngine;

namespace Meangpu.Move3D.FPS
{
    public class FPSCamera : MonoBehaviour
    {
        // learn from https://www.youtube.com/watch?v=f473C43s8nE
        [SerializeField] float _sensitivityX;
        [SerializeField] float _sensitivityY;
        [Header("This is a child Gameobject inside player")]
        [SerializeField] Transform _orientationTarget;
        [SerializeField] Transform _playerTransform;
        [Header("Look up down lock")]
        [SerializeField] float _maxLookUp = 60;
        [SerializeField] float _maxLookDown = -60;
        float _xRotation;
        float _yRotation;

        void Start()
        {
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

        void Update()
        {
            float _mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _sensitivityX;
            float _mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _sensitivityY;
            _xRotation += _mouseX;
            _yRotation -= _mouseY;

            _xRotation = Mathf.Clamp(_xRotation, _maxLookDown, _maxLookUp);

            _playerTransform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            _orientationTarget.rotation = Quaternion.Euler(0, _yRotation, 0);
        }

    }
}