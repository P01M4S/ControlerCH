using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
     public CharacterController _controller;
    public InputAction _moveAction;
    public Vector2 _moveInput;
    public InputAction _jumpAction;
    public Animator _animator;
    public InputAction _aimAction;
    public float _movementSpeed = 5;
    public float _jumpHeigth = 2;
    public float _gravity = -9;
    [SerializeField] private Vector3 _playerGravity;
    public Transform _sensor;
    public LayerMask _groundLayer;
    public float _sensorRadius;

    public InputAction _lookAction;
    public Vector2 _lookImput;
    public float _cameraSenivility = 10;
    [SerializeField] private Transform _lookAtSky;
    float _xRotation;
    Transform _mainCamara;
    
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _aimAction = InputSystem.actions["Aim"];
        _lookAction = InputSystem.actions["Look"];
        _animator = GetComponentInChildren<Animator>();
        _mainCamara = Camera.main.transform;
    }

    void Start()
    {
        _jumpTimeOutDelta = jumpTimeOut;
        _fallTimeOutDelta = fallTimeOut;
    }

    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookImput = _lookAction.ReadValue<Vector2>();
        Gravity();

        Movement();

        if (_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }
    }
    
    float _speedChangeRate = 10;
    float _speed;
    float _animationSpeed;
    bool isSprinting = false;
    float _sprintSpeed;
    float targetAngle;
    float jumpTimeOut = 0.5f;
    float fallTimeOut = 0.15f;
    float _jumpTimeOutDelta;
    float _fallTimeOutDelta;
    void Movement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        float mouseX = _lookImput.x * _cameraSenivility * Time.deltaTime;
        float mouseY = _lookImput.y * _cameraSenivility * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);


        //_animator.SetFloat("horizontal", _moveInput.x);
        //_animator.SetFloat("vertical", _moveInput.y);
        /*float targetSpeed;
        if(isSprinting)
        {
            targetSpeed = _sprintSpeed;
        }
        else
        {
            targetSpeed = _movementSpeed;
        }*/
        float targetSpeed = _movementSpeed;

        if(direction == Vector3.zero)
        {
            targetSpeed = 0;
        }
        float currentSpeed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        if(currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentSpeed, targetSpeed * direction.magnitude, Time.deltaTime * _speedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationSpeed = Mathf.Lerp(_animationSpeed, targetSpeed, Time.deltaTime * _speedChangeRate);
        if(_animationSpeed < 0.1f)
        {
            _animationSpeed = 0;
        }
        _animator.SetFloat("Speed", _animationSpeed);


        transform.Rotate(Vector3.up, mouseX);
        _lookAtSky.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        //_lookAtSky.Rotate(Vector3.right, mouseY);

        if(direction != Vector3.zero)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamara.eulerAngles.y;
        }
        Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        _controller.Move(_speed * Time.deltaTime * moveDirection.normalized + _playerGravity * Time.deltaTime);
    }
    
     void Gravity()
    {
        /*if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if (IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = _gravity;
            _animator.SetBool("IsJumping", false);
        }*/
        //_controller.Move(_playerGravity * Time.deltaTime);
        
        _animator.SetBool("Land", IsGrounded());
        if(IsGrounded())
        {
            _fallTimeOutDelta = fallTimeOut;

            _animator.SetBool("Jump", false);
            _animator.SetBool("Fall", false);
            if(_playerGravity.y < 0)
            {
                _playerGravity.y = -2;
            }
            if(_jumpTimeOutDelta >= 0)
            {
                _jumpTimeOutDelta -= Time.deltaTime;
            }
        }
        else
            {
                _jumpTimeOutDelta = jumpTimeOut;
                if(_fallTimeOutDelta >= 0)
                {
                    _fallTimeOutDelta -= Time.deltaTime;
                }
                else
                {
                    _animator.SetBool("Fall", true);
                }
                _playerGravity.y += _gravity * Time.deltaTime;
            }
    }

    void Jump()
    {
        if(_jumpTimeOutDelta <= 0)
        {
            _animator.SetBool("Jump", true);
            _playerGravity.y = Mathf.Sqrt(_jumpHeigth * -2 * _gravity);
        }

        //_controller.Move(_playerGravity * Time.deltaTime);
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
