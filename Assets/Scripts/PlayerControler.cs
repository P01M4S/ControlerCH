using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;
using UnityEngine.UIElements;

public class PlayerControler : MonoBehaviour
{
    public Animator _animator;
    public CharacterController _controller;
    public InputAction _moveAction;
    public Vector2 _moveInput;
    public float _movementSpeed = 5;
    public float _gravity = -9;
    [SerializeField] private Vector3 _playerGravity;
    public Transform _sensor;
    public LayerMask _groundLayer;
    public float _sensorRadius;
    public float _jumpHeigth = 2;
    public InputAction _jumpAction;
    public float _smoothTime = 0.2f;
    private float _turnSmooth;
    public InputAction _lookAction;
    public Vector2 _lookImput;
    public Transform _mainCamara;
    public InputAction _aimAction;
    public float _pushForce = 15;
    public Transform _manos;
    public Transform _grabObj;
    public Vector3 _manosSensor;
    public InputAction _grabAction;
    public InputAction _throwAction;
    public float _throwForce = 200;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _lookAction = InputSystem.actions["Look"];
        _mainCamara = Camera.main.transform;
        _aimAction = InputSystem.actions["Aim"];
        _animator = GetComponentInChildren<Animator>();
        _grabAction = InputSystem.actions["Interact"];
        _throwAction = InputSystem.actions["Throw"];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookImput = _lookAction.ReadValue<Vector2>();
        //MovimientoCutre();
        //Movement();
        //AimMovement();

        if (_aimAction.IsInProgress())
        {
            AimMovement();
        }
        else
        {
            Movement();
        }

        Gravity();
        if (_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        if (_aimAction.WasPerformedThisFrame())
        {
            Attack();
        }

        if (_grabAction.WasPerformedThisFrame())
        {
            GrabObject();
        }

        if (_throwAction.WasPerformedThisFrame())
        {
            //Throw();
            RayTest();
        }

        
    }

    void Attack()
    {
        Ray ray = Camera.main.ScreenPointToRay(_lookImput);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Enemy enemyScript = hit.transform.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                //enemyScript.TakeDamage();
            }
        }
    }

    void AimMovement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        _animator.SetFloat("horizontal", _moveInput.x);
        _animator.SetFloat("vertical", _moveInput.y);

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamara.eulerAngles.y;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamara.eulerAngles.y, ref _turnSmooth, _smoothTime);

        transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

        if (direction != Vector3.zero)
        {
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _controller.Move(moveDirection.normalized * _movementSpeed * Time.deltaTime);
        }
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        _animator.SetFloat("vertical", direction.magnitude);
        _animator.SetFloat("horizontal", 0);

        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamara.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmooth, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _controller.Move(moveDirection.normalized * _movementSpeed * Time.deltaTime);
        }
    }

    /*void Movimiento2()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);
        Ray ray = Camera.main.ScreenPointToRay(_lookImput);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 playerForeard = hit.point - transform.position;
            playerForeard.y = 0;
            transform.forward = playerForeard;
        }
        
        if (direction != Vector3.zero)
        {
            _controller.Move(direction.normalized * _movementSpeed * Time.deltaTime);
        }
    }

    void MovimientoCutre()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);
        if (direction != Vector3.zero)
        {
            _controller.Move(direction.normalized * _movementSpeed * Time.deltaTime);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmooth, _smoothTime);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        }
    }*/

    void Gravity()
    {
        if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if (IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = _gravity;
            _animator.SetBool("IsJumping", false);
        }
        _controller.Move(_playerGravity * Time.deltaTime);
    }

    void Jump()
    {
        _animator.SetBool("IsJumping", true);
        _playerGravity.y = Mathf.Sqrt(_jumpHeigth * -2 * _gravity);
        _controller.Move(_playerGravity * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensor.position, _sensorRadius, _groundLayer);
    }
    /*bool IsGrounded()
    {
        if (Physics.Raycast(_sensor.position, -transform.up, _sensorRadius, _groundLayer))
        {
            Debug.DrawLine(_sensor.position, -transform.up * _sensorRadius, Color.cyan);
            return true;
        }
        else
        {
            Debug.DrawLine(_sensor.position, -transform.up * _sensorRadius, Color.yellow);
            return false;
        }
    }*/
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_sensor.position, _sensorRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_manos.position, _manosSensor);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.gameObject.tag == "Empujable")
        {
            Rigidbody rBody = hit.collider.attachedRigidbody;
            //Rigidbody rBody = hit.transform.GetComponent<Rigidbody>(); 

            if (rBody == null || rBody.isKinematic)
            {
                return;
            }

            Vector3 puchDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            rBody.linearVelocity = puchDirection * _pushForce / rBody.mass;
        }
    }

    void GrabObject()
    {
        if (_grabObj == null)
        {
            Collider[] objectsToGrab = Physics.OverlapBox(_manos.position, _manosSensor);

            foreach (Collider item in objectsToGrab)
            {
                IGrabable grabableObject = item.GetComponent<IGrabable>();
                if (grabableObject != null)
                {
                    _grabObj = item.transform;
                    _grabObj.SetParent(_manos);
                    _grabObj.position = _manos.position;
                    _grabObj.rotation = _manos.rotation;
                    _grabObj.GetComponent<Rigidbody>().isKinematic = true;
                    return;
                }
            }
        }
        else
        {
            _grabObj.SetParent(null);
            _grabObj.GetComponent<Rigidbody>().isKinematic = false;
            _grabObj = null;
        }
    }

    void Throw()
    {
        if (_grabObj == null)
        {
            return;
        }
        Rigidbody grabedBody = _grabObj.GetComponent<Rigidbody>();
        _grabObj.SetParent(null);
        grabedBody.isKinematic = false;
        grabedBody.AddForce(_mainCamara.transform.forward * _throwForce, ForceMode.Impulse);
        _grabObj = null;
    }

    void RayTest()
    {
        if (Physics.Raycast(transform.position, transform.forward, 5))
        {
            Debug.Log("hit");
            Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 5, Color.blue);
        }


        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5))
        {
            Debug.Log(hit.transform.name);
            Debug.Log(hit.transform.position);
            Debug.Log(hit.transform.gameObject.layer);
            Debug.Log(hit.transform.tag);

            /*if (hit.transform.tag == "Empujable")
            {
                Box box = hit.transform.GetComponent<Box>();
                if (box != null)
                {
                    Debug.Log("Zapatos?!");
                }
            }*/
            IDamageEnable damageEnable = hit.transform.GetComponent<IDamageEnable>();
            if (damageEnable != null)
            {
                damageEnable.TakeDamage(5);
            }
        }
        Ray ray = Camera.main.ScreenPointToRay(_lookImput);
        RaycastHit hit2;
        if (Physics.Raycast(ray, out hit2, Mathf.Infinity))
        {
            Vector3 playerForeard = hit.point - transform.position;
            playerForeard.y = 0;
            transform.forward = playerForeard;
        }
    }
}

