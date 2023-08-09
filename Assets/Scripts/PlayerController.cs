using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private bool _isCanMoving = true;
    private bool _isCanRotation = true;
    private bool _isCanUsingItem = true;
    private float _velocity;
    private Vector2 _inputMovement;
    private Vector3 _direction;
    private Vector3 _rotationCamera;
    private Vector3 _rotationPlayer;
    private GameObject _heldObject;
    private Rigidbody _heldRigidbody;
    private Rigidbody _heldRigidbodyItem;
    private ItemController _heldItem;
    private CharacterController _chController;

    [Header("Player object")]
    [SerializeField] private Transform _holdArea;
    [SerializeField] private Transform _hand;

    public static PlayerController GetInstance() => instance;
    public bool HoldingObject() => _heldObject != null;
    public bool HoldingItem() => _heldItem != null;
    private bool IsGrounded() => _chController.isGrounded;

    public bool IsCanMoving
    {
        get => _isCanMoving;
        set => _isCanMoving = value;
    }

    public bool IsCanRotation
    {
        get => _isCanRotation;
        set => _isCanRotation = value;
    }

    public bool IsCanUsingItem
    {
        get => _isCanUsingItem;
        set => _isCanUsingItem = value;
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
        ApplyMovementObject();
    }

    private void ApplyGravity()
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

    private void ApplyMovement()
    {
        if (!_isCanMoving) return;

        Vector3 direction = (_inputMovement.y * transform.forward) + (_inputMovement.x * transform.right);
        _direction.x = direction.x;
        _direction.z = direction.z; 
        _chController.Move(_direction * PlayerConstants.MOVEMENT_SPEED * Time.deltaTime);
    }

    private void ApplyRotation()
    {
        if (!_isCanRotation) return;

        Camera.main.transform.localRotation = Quaternion.Euler(_rotationCamera);
        transform.rotation = Quaternion.Euler(_rotationPlayer);
    }

    private void ApplyMovementObject()
    {
        if (_heldObject == null) return;

        Vector3 moveDirectionObject = (_holdArea.position - _heldObject.transform.position);
        _heldRigidbody.AddForce(moveDirectionObject * PlayerConstants.PICKUP_FORCE);
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

    public void PickupItem(GameObject heldItem)
    {
        Item item = heldItem.GetComponent<ItemController>().Item;
        if(!InventoryController.GetInstance().AddItem(item, item.Count)) return;
        Destroy(heldItem);
    }

    public void ChangeActiveItemInHand(Item item)
    {
        if (_heldItem != null) Destroy(_heldItem.gameObject);
        if (item == null) return;

        GameObject heldItem = Instantiate(Resources.Load<GameObject>(ResourceConstants.ITEMS + (ItemIdsEnum)item.Id));
        _heldRigidbodyItem = heldItem.GetComponent<Rigidbody>();
        _heldRigidbodyItem.isKinematic = true;
        _heldRigidbodyItem.transform.parent = _hand;
        _heldItem = heldItem.GetComponent<ItemController>();
        _heldItem.Item = item;
        _heldItem.IsUpdating = true;
        _heldItem.gameObject.layer = LayerMask.NameToLayer(LayerConstants.ITEM);
        //ToDo: Remove after create whole objects (https://trello.com/c/d3sKzxu6/26-remove-cycle)
        for (int i = 0; i < _heldItem.transform.childCount; i++)
        {
            _heldItem.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(LayerConstants.ITEM);
        }
        _heldItem.transform.localPosition = Vector3.zero;
        _heldItem.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void DropItem()
    {
        if (_heldItem == null) return;

        InventoryController.GetInstance().RemoveItem();
        _heldRigidbodyItem = _heldItem.GetComponent<Rigidbody>();
        _heldRigidbodyItem.isKinematic = false;
        _heldItem.IsUpdating = false;
        _heldItem.ApplyItemDisable();
        _heldItem.gameObject.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
        //ToDo: Remove after create whole objects (https://trello.com/c/d3sKzxu6/26-remove-cycle)
        for (int i = 0; i < _heldItem.transform.childCount; i++)
        {
            _heldItem.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
        }
        _heldItem.transform.parent = null;
        _heldRigidbodyItem = null;
        _heldItem = null;
    }

    public void UseItemInPlayerHand()
    {
        if (!_isCanUsingItem) return;

        _heldItem.Item.UseItem();
    }
}
