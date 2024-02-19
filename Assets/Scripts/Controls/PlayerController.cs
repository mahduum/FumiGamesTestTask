using System;
using AbilitySystem.Scripts.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controls
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput), typeof(InventorySystem.InventorySystem))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Vector3 _playerVelocity;
        [SerializeField] private bool _groundedPlayer;
        private float _playerSpeed = 2.0f;
        private float _jumpHeight = 1.0f;
        private float _gravityValue = -9.81f;

        private PlayerInput _playerInput;
        private CharacterController _controller;
        private InventorySystem.InventorySystem _inventorySystem;
        
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;

        private Transform _cameraTransform;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            _inventorySystem = GetComponent<InventorySystem.InventorySystem>();
            _moveAction =_playerInput.actions["Move"];
            _jumpAction =_playerInput.actions["Jump"];
            _cameraTransform = Camera.main.transform;
        }

        void Update()
        {
            _groundedPlayer = _controller.isGrounded;
            if (_groundedPlayer && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0f;
            }

            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

            move = move.x * _cameraTransform.right.normalized + move.z * _cameraTransform.forward.normalized;
            move.y = 0f;
            
            _controller.Move(move * Time.deltaTime * _playerSpeed);

            if (_jumpAction.triggered && _groundedPlayer)
            {
                _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
            }

            _playerVelocity.y += _gravityValue * Time.deltaTime;
            _controller.Move(_playerVelocity * Time.deltaTime);

            var targetRotation = Quaternion.Euler(_cameraTransform.eulerAngles.x, _cameraTransform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
}
