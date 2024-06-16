using UnityEngine;
using UnityEngine.InputSystem;

namespace Meangpu.Move3D.TopDown
{
    public class PlayerMovementInput : MonoBehaviour
    {
        [SerializeField] InputActionReference _moveInputAction;
        [SerializeField] Vector3Reference _moveOutput;
        Vector3 _moveDirection = new();
        Vector2 _moveInput;

        void OnEnable() => _moveInputAction.action.Enable();
        void OnDisable() => _moveInputAction.action.Disable();

        public Vector3 GetMovementVectorNormalize()
        {
            _moveInput = _moveInputAction.action.ReadValue<Vector2>();
            _moveDirection.Set(_moveInput.x, 0, _moveInput.y);
            return _moveDirection.normalized;
        }
        private void Update() => _moveOutput.Variable.SetValue(GetMovementVectorNormalize());
    }
}
