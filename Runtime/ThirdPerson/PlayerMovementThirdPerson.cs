using System;
using UnityEngine;

namespace Meangpu.Move3D.ThirdPerson
{
    // learn from [Character Controller Tutorial in Unity | AshDev - YouTube](https://www.youtube.com/watch?v=i5NVbu7rQJE)
    // give up half way because this script is not much useful / I already have asset that do this better
    public class PlayerMovementThirdPerson : MonoBehaviour
    {
        [Header("Ref")]
        [SerializeField] CharacterController _controller;
        [SerializeField] Camera _camera;

        [Header("Speed")]
        [SerializeField] float _moveSpeed = 5f;
        [SerializeField] float _turnSpeed = 2f;

        float _moveInputValue;
        float _turnInputValue;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            UpdateInput();

            GroundMovement();
            Turn();
        }

        void UpdateInput()
        {
            _moveInputValue = Input.GetAxis("Vertical");
            _turnInputValue = Input.GetAxis("Horizontal");
        }

        void Turn()
        {
            if (Math.Abs(_turnInputValue) <= 0 && Math.Abs(_moveInputValue) <= 0) return;
            Vector3 _currentLookRotation = _controller.velocity.normalized;
            _currentLookRotation.y = 0;
            _currentLookRotation.Normalize();

            Quaternion _targetRotation = Quaternion.LookRotation(_currentLookRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _turnSpeed);
        }

        void GroundMovement()
        {
            Vector3 moveVector = new(_turnInputValue, 0, _moveInputValue);

            moveVector = _camera.transform.TransformDirection(moveVector);

            moveVector *= _moveSpeed;
            _controller.Move(moveVector * Time.deltaTime);
        }
    }
}