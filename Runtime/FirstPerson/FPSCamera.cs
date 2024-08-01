using UnityEngine;

namespace Meangpu.Move3D.FPS
{
    public class FPSCamera : MonoBehaviour
    {
        [SerializeField] float _sensitivityX;
        [SerializeField] float _sensitivityY;
        [SerializeField] Transform _orientation;
        float _xRotation;
        float _yRotation;

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

        }

    }
}