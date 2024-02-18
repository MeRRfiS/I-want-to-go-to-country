using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerService: IPlayerService
{
    private GameObject _player;
    private Transform _holdArea;
    private Transform _hand;
    private CharacterController _chController;
    private Animator _handAnim;

    private bool _isCanRotation;
    private bool _isCanMoving;
    private bool _isCanUsingItem;

    private float _velocity;
    private Vector2 _inputMovement;
    private Vector3 _direction;
    private Vector3 _rotationCamera;
    private Vector3 _rotationPlayer;
    private GameObject _heldObject;
    private Rigidbody _heldRigidbody;
    private Rigidbody _heldRigidbodyItem;
    private ItemController _heldItem;

    private bool IsGrounded() => _chController.isGrounded;

    public bool IsCanRotation() => _isCanRotation;
    public bool IsCanMoving() => _isCanMoving;
    public bool IsCanUsingItem() => _isCanUsingItem;
    public bool HoldingObject() => _heldObject != null;
    public bool HoldingItem() => _heldItem != null;

    public PlayerService(GameObject player,
                         Transform holdArea,
                         Transform hand,
                         CharacterController chController,
                         Animator handAnim)
    {
        _player = player;
        _holdArea = holdArea;
        _hand = hand;
        _chController = chController;
        _handAnim = handAnim;

        SwitchActiveController(true);
    }

    public void ApplyGravity()
    {
        if (!_isCanMoving) return;

        if (IsGrounded() && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += Physics.gravity.y * GlobalConstants.GRAVITY_MULTIPLIER * Time.deltaTime;
        }

        _direction.y = _velocity;
    }

    public void ApplyMovement()
    {
        if (!_isCanMoving)
        {
            _inputMovement = Vector2.zero;
            return;
        }

        Vector3 direction = (_inputMovement.y * _player.transform.forward) + 
                            (_inputMovement.x * _player.transform.right);
        _direction.x = direction.x;
        _direction.z = direction.z;
        _chController.Move(_direction * PlayerConstants.MOVEMENT_SPEED * Time.deltaTime);

        _handAnim.SetBool(AnimPropConstants.IS_MOVING, 
                          _chController.velocity.x != 0 || _chController.velocity.y != 0);
    }

    public void ApplyMovementObject()
    {
        if (_heldObject == null) return;

        Vector3 moveDirectionObject = (_holdArea.position - _heldObject.transform.position);
        _heldRigidbody.AddForce(moveDirectionObject * PlayerConstants.PICKUP_FORCE);
    }

    public void ApplyRotation()
    {
        if (!_isCanRotation) return;

        Camera.main.transform.localRotation = Quaternion.Euler(_rotationCamera);
        _player.transform.rotation = Quaternion.Euler(_rotationPlayer);
    }

    public void SwitchActiveController(bool state)
    {
        _isCanMoving = state;
        _isCanRotation = state;
        _isCanUsingItem = state;
    }

    public void ChangeMovement(Vector2 movement)
    {
        _inputMovement = movement;
    }

    public void ChangeVelocity()
    {
        if (!IsGrounded()) return;

        _velocity = PlayerConstants.JUMP_SPEED;
    }

    public void ChangeRotation(float rotationCamera = 0,
                               float rotationPlayer = 0)
    {
        _rotationCamera = (Vector3)(rotationCamera != 0 ? new Vector3(rotationCamera, 0, 0) : _rotationCamera);
        _rotationPlayer = (Vector3)(rotationPlayer != 0 ? new Vector3(0, rotationPlayer, 0) : _rotationPlayer);
    }

    public void PickupObject(GameObject heldObject)
    {
        if (_heldObject != null) return;

        _heldRigidbody = heldObject.GetComponent<Rigidbody>();
        if (_heldRigidbody.mass > PlayerConstants.MAX_MASS_HELD_OBJECT) return;

        _heldRigidbody.useGravity = false;
        _heldRigidbody.drag = 10;
        _heldRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _heldRigidbody.transform.parent = _holdArea;
        _heldObject = heldObject;
    }

    public void DropObject()
    {
        if (_heldObject == null) return;

        _heldRigidbody.useGravity = true;
        _heldRigidbody.drag = 1;
        _heldRigidbody.constraints = RigidbodyConstraints.None;
        _heldObject.transform.parent = null;
        _heldRigidbody = null;
        _heldObject = null;
    }

    public void UseItemInPlayerHand()
    {
        if (!_isCanUsingItem) return;

        _heldItem.Item.UseItem();
    }
}
