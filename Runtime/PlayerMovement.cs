using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Meangpu.Move3D.TopDown
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("MoveSpeedData")]
        [Expandable][SerializeField] SOPlayerMove3D _speedData;

        [Header("DashInput")]
        [SerializeField] InputActionReference _dashInputAction;
        bool _isDashing;
        float _nowDashCooldown;
        public static Action OnDash;

        [SerializeField] Vector3Reference _moveDirection;
        bool _isWalking;
        public bool IsWalking => _isWalking;

        void OnEnable()
        {
            if (_dashInputAction == null) return;
            _dashInputAction.action.Enable();
            _dashInputAction.action.performed += Dash;
        }

        void OnDisable()
        {
            if (_dashInputAction == null) return;
            _dashInputAction.action.performed -= Dash;
            _dashInputAction.action.Disable();
        }

        private void Dash(InputAction.CallbackContext context) => Dash();

        public void Dash()
        {
            if (_isDashing) return;
            if (_nowDashCooldown > 0) return;
            StopAllCoroutines();
            StartCoroutine(DashCoroutine());
        }

        private void Update()
        {
            _nowDashCooldown -= Time.deltaTime;
            Walking();
        }

        private void Walking()
        {
            if (_isDashing) return;
            float _moveDistance = _speedData.MoveSpeed * Time.deltaTime;
            if (_moveDirection == Vector3.zero) return;
            MovePlayer(_moveDistance, _moveDirection.Value);
        }

        private void MovePlayer(float _moveDistance, Vector3 _direction)
        {
            _isWalking = _direction != Vector3.zero;
            bool _canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * _speedData.PlayerHeight, _speedData.PlayerRadius, _direction, _moveDistance);

            // make it can still move on one axis when collide with wall
            if (!_canMove)
            {
                Vector3 _moveX = new Vector3(_direction.x, 0, 0).normalized;
                _canMove = (_direction.x < -.5f || _direction.x > .5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * _speedData.PlayerHeight, _speedData.PlayerRadius, _moveX, _moveDistance);

                if (_canMove)
                {
                    _direction = _moveX;
                }
                else
                {
                    Vector3 _moveZ = new Vector3(0, 0, _direction.z).normalized;
                    _canMove = (_direction.z < -.5f || _direction.z > .5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * _speedData.PlayerHeight, _speedData.PlayerRadius, _moveZ, _moveDistance);
                    if (_canMove)
                    {
                        _direction = _moveZ;
                    }
                }
            }

            if (_canMove)
            {
                transform.position += _direction * _moveDistance;
            }

            transform.forward = Vector3.Slerp(transform.forward, _direction, _speedData.RotateLerpSpeed * Time.deltaTime);
        }

        public IEnumerator DashCoroutine()
        {
            _isDashing = true;
            OnDash?.Invoke();
            _nowDashCooldown = _speedData.DashCooldown;
            float _dashTime = _speedData.DashDuration;

            while (_dashTime > 0)
            {
                _dashTime -= Time.deltaTime;
                float _moveDistance = _speedData.DashSpeed * Time.deltaTime;
                MovePlayer(_moveDistance, transform.forward);
                yield return null;
            }

            _isDashing = false;
        }
    }
}
