using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private bool _isCanMoving = true;
    private bool _isCanRotation = true;
    private bool _isCanUsingItem = true;
    private int _money = 10000;
    private float _velocity;
    private Vector2 _inputMovement;
    private Vector3 _direction;
    private Vector3 _rotationCamera;
    private Vector3 _rotationPlayer;
    private Item _newItem;
    private GameObject _heldObject;
    private NPCController _heldNPC;
    private Rigidbody _heldRigidbody;
    private Rigidbody _heldRigidbodyItem;
    private ItemController _heldItem;
    private CharacterController _chController;
    private EventInstance _eventInstance;

    [Header("Player object")]
    [SerializeField] private Transform _holdArea;
    [SerializeField] private Transform _hand;
    [SerializeField] private Transform _npcHand;

    public static PlayerController GetInstance() => instance;
    //TODO: Need to do better checking of holding
    public bool IsHoldingObject() => _heldObject != null;
    public bool IsHoldingNPC() => _heldNPC != null;
    public bool IsHoldingItem() 
    {
        var result = _heldItem != null;

        if (result) return result;
        else if (_hand.childCount != 0) return true;
        else return false;
    }
    private bool IsGrounded() => _chController.isGrounded;
    private ItemController HeldItem()
    {
        if (_heldItem != null) return _heldItem;
        else if (_hand.childCount != 0) return _hand.GetComponentInChildren<ItemController>();
        else return null;
    }

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
        HandsAnimationManager.GetInstance().OnHideItem += RemoveHeldItem;

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

        HandsAnimationManager.GetInstance().IsMoving(_chController.velocity.x != 0 || _chController.velocity.y != 0);
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
        if ((_chController.velocity.x != 0 || _chController.velocity.y != 0) && IsGrounded() && _isCanMoving)
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
            //_eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

    public void PickupNPC(NPCController npc)
    {
        npc.IsHold = true;
        Transform npcTransform = npc.transform;
        npcTransform.parent = _npcHand;
        //npcTransform.localPosition = new Vector3(0, -0.003f, -0.001f);
        //npcTransform.localRotation = Quaternion.identity;

        if (HeldItem() != null)
        {
            HeldItem().gameObject.SetActive(false);
        }

        InventoryController.GetInstance().IsCanChangeActiveItem = false;

        _heldNPC = npc;
    }

    public void DropNPC()
    {
        _heldNPC.DropFromHand();
        _heldNPC.transform.parent = null;

        if (HeldItem() != null)
        {
            HeldItem().gameObject.SetActive(true);
        }

        InventoryController.GetInstance().IsCanChangeActiveItem = true;

        _heldNPC = null;
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
        if (HeldItem() != null)
        {
            HeldItem().Item.IsInHand = false;
            HandsAnimationManager.GetInstance().IsChangeItem = item != null;
            HandsAnimationManager.GetInstance().IsChangingItem(true);
        }
        if (item == null)
        {
            HandsAnimationManager.GetInstance().OnChangeItem -= ChangingInstrument;
            HandsAnimationManager.GetInstance().OnNewItem -= GetNewItem;
            HandsAnimationManager.GetInstance().IsHoldInst(false);
            HandsAnimationManager.GetInstance().IsHoldFunnel(false);
            HandsAnimationManager.GetInstance().IsHoldStaf(false);
            _newItem = null;

            return;
        }

        _newItem = item;
        _newItem.GetItemInHand();

        HandsAnimationManager.GetInstance().OnNewItem += GetNewItem;
        HandsAnimationManager.GetInstance().OnChangeItem += ChangingInstrument;
    }

    public void DropItem()
    {
        if (HeldItem() == null) return;

        HandsAnimationManager.GetInstance().IsChangeItem = false;
        HandsAnimationManager.GetInstance().IsChangingItem(true);
        InventoryController.GetInstance().RemoveItem();
        UIController.GetInstance().StopProgressBar();
        _heldRigidbodyItem = HeldItem().GetComponent<Rigidbody>();
        _heldRigidbodyItem.isKinematic = false;
        HeldItem().IsUpdating = false;
        HeldItem().ApplyItemDisable();
        HeldItem().gameObject.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
        //ToDo: Remove after create whole objects (https://trello.com/c/d3sKzxu6/26-remove-cycle)
        for (int i = 0; i < _heldItem.transform.childCount; i++)
        {
            HeldItem().transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
        }
        HeldItem().transform.parent = null;
        _heldRigidbodyItem = null;
        _heldItem = null;
    }

    public void UseItemInPlayerHand()
    {
        if (!_isCanUsingItem) return;
        if(HeldItem() == null) return;
        
        HeldItem().Item.UseItem();
    }

    public void SwitchActiveController(bool state)
    {
        _isCanMoving = state;
        _isCanRotation = state;
        _isCanUsingItem = state;
        HandsAnimationManager.GetInstance().IsMoving(false);
    }

    private void ChangingInstrument()
    {
        if (_newItem == null) return;

        ItemController heldItem = Instantiate(_newItem.Object);

        SetUpItemRigidbody(heldItem);
        SetUpNewItem(heldItem);

        HandsAnimationManager.GetInstance().OnChangeItem -= ChangingInstrument;
        _newItem = null;
    }

    private void SetUpNewItem(ItemController heldItem)
    {
        _heldItem = heldItem;
        _heldItem.OnItemBroke += HoldItemBroken;
        _heldItem.Item = _newItem;
        _heldItem.IsUpdating = true;
        _heldItem.gameObject.layer = LayerMask.NameToLayer(LayerConstants.IGNORE_REYCAST);
        //ToDo: Remove after create whole objects (https://trello.com/c/d3sKzxu6/26-remove-cycle)
        for (int i = 0; i < HeldItem().transform.childCount; i++)
        {
            _heldItem.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(LayerConstants.IGNORE_REYCAST);
        }
        _heldItem.transform.localPosition = _newItem.Object.transform.position;
        _heldItem.transform.localRotation = _newItem.Object.transform.rotation;
    }

    private void SetUpItemRigidbody(ItemController heldItem)
    {
        _heldRigidbodyItem = heldItem.GetComponent<Rigidbody>();
        _heldRigidbodyItem.isKinematic = true;
        _heldRigidbodyItem.transform.parent = _hand;

        if(_hand.childCount > 1)
        {
            Destroy(_hand.GetChild(0).gameObject);
        }
    }

    private void RemoveHeldItem()
    {
        if (HeldItem() != null)
        {
            Destroy(HeldItem().gameObject);
            HeldItem().OnItemBroke -= HoldItemBroken;
            _heldItem = null;
        }
    }

    private void GetNewItem()
    {
        if (_newItem != null)
        {
            _newItem.GetItemInHand();
        }
    }

    private void HoldItemBroken()
    {
        HandsAnimationManager.GetInstance().IsChangeItem = false;
        HandsAnimationManager.GetInstance().IsChangingItem(true);
        HeldItem().OnItemBroke -= HoldItemBroken;
        _heldItem = null;
    }
}
