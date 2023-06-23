using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private float _velocity;
    private Vector2 _inputMovement;
    private Vector3 _direction;
    private Vector3 _rotationCamera;
    private Vector3 _rotationPlayer;
    private CharacterController _chController;

    public static PlayerController GetInstance() => instance;
    public bool IsGrounded() => _chController.isGrounded;

    public float Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }
    public Vector2 InputMovement
    {
        set => _inputMovement = value;
    }
    public Vector3 RotationCamera
    {
        set => _rotationCamera = value;
    }
    public Vector3 RotationPlayer
    {
        set => _rotationPlayer = value;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _chController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity();
        ApplyMovement();
        ApplyRotation();
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += Physics.gravity.y * WorldConstants.GRAVITY_MULTIPLIER * Time.deltaTime;
        }

        _direction.y = _velocity;
    }

    private void ApplyMovement()
    {
        Vector3 direction = (_inputMovement.y * transform.forward) + (_inputMovement.x * transform.right);
        _direction.x = direction.x;
        _direction.z = direction.z; 
        _chController.Move(_direction * PlayerConstants.MOVEMENT_SPEED * Time.deltaTime);
    }

    private void ApplyRotation()
    {
        Camera.main.transform.localRotation = Quaternion.Euler(_rotationCamera);
        transform.rotation = Quaternion.Euler(_rotationPlayer);
    }
}
