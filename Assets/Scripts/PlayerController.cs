using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private bool _isCanMoving = true;
    private bool _isCanRotation = true;
    private bool _isCanUsingItem = true;
    private int _money = 300;
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
    private EventInstance _eventInstance;

    [Header("Player object")]
    [SerializeField] private Transform _holdArea;
    [SerializeField] private Transform _hand;

    [Header("Animator")]
    public Animator _hands;

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

    public int Money
    {
        get => _money; 
        set => _money = value;
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
        _eventInstance = AudioController.GetInstance().CreateInstance(FMODEvents.GetInstance().WalkOnGrass);
    }

    private void Update()
    {
        ApplyGravity();
        ApplyMovement();
        UpdateSound();
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
        if (!_isCanMoving) 
        {
            _inputMovement = Vector2.zero;
            return;
        }

        Vector3 direction = (_inputMovement.y * transform.forward) + (_inputMovement.x * transform.right);
        _direction.x = direction.x;
        _direction.z = direction.z; 
        _chController.Move(_direction * PlayerConstants.MOVEMENT_SPEED * Time.deltaTime);

        _hands.SetBool(AnimPropConstants.IS_MOVING, _chController.velocity.x != 0 || _chController.velocity.y != 0);
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

    private void UpdateSound()
    {
        if((_chController.velocity.x != 0 || _chController.velocity.y != 0) && IsGrounded())
        {
            PLAYBACK_STATE playbackState;
            _eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            _eventInstance.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                _eventInstance.start();
            }
        }
        else
        {
            _eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
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
        if(!InventoryController.GetInstance().AddItemToMainInventory(item.Copy(), item.Amount)) return;
        Destroy(heldItem);
    }

    public void ChangeActiveItemInHand(Item item)
    {
        if (_heldItem != null)
        {
            _hands.SetBool("_IsChangingInst", true);
        }
        if (item == null) 
        {
            StartCoroutine(HideInstrument());
            return;
        }

        StartCoroutine(ChangingInstrument(item));
    }

    public void DropItem()
    {
        if (_heldItem == null) return;

        _hands.SetBool("_IsChangingInst", true);
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

        StartCoroutine(HideInstrument());
    }

    public void UseItemInPlayerHand()
    {
        if (!_isCanUsingItem) return;

        _heldItem.Item.UseItem();
    }

    public void SwitchActiveController(bool state)
    {
        _isCanMoving = state;
        _isCanRotation = state;
        _isCanUsingItem = state;
    }

    private IEnumerator HideInstrument()
    {
        yield return new WaitForSeconds(0.1f);

        _hands.SetBool("_IsChangingInst", false);
        if(_heldItem != null)
        {
            if (_heldItem.Item is Funnel)
            {
                _hands.SetBool("_IsHoldFunnel", false);
            }
            else
            {
                _hands.SetBool("_IsHoldInst", false);
            }
        }

        if (_heldItem != null)
        {
            yield return new WaitForSeconds(0.45f);
            Destroy(_heldItem.gameObject);
            _heldItem = null;
        }
    }

    private IEnumerator ChangingInstrument(Item item)
    {
        if (item is Funnel)
        {
            _hands.SetBool("_IsHoldFunnel", true);
            _hands.SetBool("_IsHoldInst", false);
        }
        else
        {
            _hands.SetBool("_IsHoldInst", true);
            _hands.SetBool("_IsHoldFunnel", false);
        }

        if (_heldItem != null)
        {
            yield return new WaitForSeconds(0.45f);
            Destroy(_heldItem.gameObject);
            _heldItem = null;
        }

        ItemController heldItem = Instantiate(item.Object);

        _heldRigidbodyItem = heldItem.GetComponent<Rigidbody>();
        _heldRigidbodyItem.isKinematic = true;
        _heldRigidbodyItem.transform.parent = _hand;

        _heldItem = heldItem;
        _heldItem.Item = item;
        _heldItem.IsUpdating = true;
        _heldItem.gameObject.layer = LayerMask.NameToLayer(LayerConstants.IGNORE_REYCAST);
        //ToDo: Remove after create whole objects (https://trello.com/c/d3sKzxu6/26-remove-cycle)
        for (int i = 0; i < _heldItem.transform.childCount; i++)
        {
            _heldItem.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(LayerConstants.IGNORE_REYCAST);
        }
        _heldItem.transform.localPosition = item.Object.transform.position;
        _heldItem.transform.localRotation = item.Object.transform.rotation;
        if(_heldItem.Item is Funnel)
        {
            _hands.SetBool("_IsHoldFunnel", true);
            _hands.SetBool("_IsHoldInst", false);
        }
        else
        {
            _hands.SetBool("_IsHoldInst", true);
            _hands.SetBool("_IsHoldFunnel", false);
        }

        yield return new WaitForSeconds(0.1f);

        _hands.SetBool("_IsChangingInst", false);
    }
}
