using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public CharacterController _controller;
    public InputAction _moveAction;
    public Vector2 _moveInput;
    public float _movementSpeed = 6;
    public float _gravity = -9;
    [SerializeField] private Vector3 _playerGravity;
    public Transform _sensor;
    public LayerMask _groundLayer;
    public float _sensorRadius;
    public float _jumpHeigth = 2;
    public InputAction _jumpAction;
    public float _smoothTime = 0.2f;
    public float _turnSmooth;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MovimientoCutre();
        Gravity();
        if (_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }
    }

    void MovimientoCutre()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);
        if (direction != Vector3.zero)
        {
            _controller.Move(direction.normalized * _movementSpeed * Time.deltaTime);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmooth, _smoothTime);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        }
    }

    void Gravity()
    {
        if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if (IsGrounded() && _playerGravity.y < _gravity)
        {
            _playerGravity.y = _gravity;
        }
        _controller.Move(_playerGravity * Time.deltaTime);
    }

    void Jump()
    {
        _playerGravity.y = Mathf.Sqrt(_jumpHeigth * -2 * _gravity);
        _controller.Move(_playerGravity * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensor.position, _sensorRadius, _groundLayer);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_sensor.position, _sensorRadius);
    }
}
