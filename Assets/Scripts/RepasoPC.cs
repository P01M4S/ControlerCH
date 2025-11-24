using UnityEngine;
using UnityEngine.InputSystem;

public class RepasoPC : MonoBehaviour
{
    Animator _animator;
    CharacterController _controller;
    InputAction _jumpAction;
    InputAction _moveAction;
    Vector2 _moveValue;
    float _movementSpeed = 5;
    float _jumpHeigth = 3;
    float _gravity = -9.81f;
    Transform _sensorPosition;
    float _sensorRadiys;
    LayerMask _groundLayer;
    Vector3 playerGravity;

    void Awake()
    {
        _animator.GetComponent<Animator>();
        _controller.GetComponent<CharacterController>();
        _jumpAction = InputSystem.actions["Jump"];
        _moveAction = InputSystem.actions["Move"];
    }

    void Start()
    {
        
    }

    void Update()
    {
        _moveValue = _moveAction.ReadValue<Vector2>();
        Movement();
        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }
        Gravity();
    }

    void Movement()
    {
        Vector3 moveDirection = new Vector3(_moveValue.x, 0, _moveValue.y);
        _controller.Move(moveDirection * _movementSpeed * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadiys, _groundLayer);
    }

    void Jump()
    {
        playerGravity.y = Mathf.Sqrt(_jumpHeigth * _gravity * -2);
        _controller.Move(playerGravity * Time.deltaTime);
    }

    void Gravity()
    {
        if(!IsGrounded())
        {
            playerGravity.y += _gravity * Time.deltaTime;
        }
        else if(IsGrounded() && playerGravity.y < 0)
        {
            playerGravity.y = -2;
        }
        _controller.Move(playerGravity * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_sensorPosition.position, _sensorRadiys);
    }
}
