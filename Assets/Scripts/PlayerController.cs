using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private int _money = 300;
    private Rigidbody _heldRigidbodyItem;
    private ItemController _heldItem;

    [Header("Player object")]
    [SerializeField] private Transform _holdArea;
    [SerializeField] private Transform _hand;

    [Header("Animator")]
    public Animator _hands;

    public ItemController HeldItem { get { return _heldItem; } }

    public static PlayerController GetInstance() => instance;

    public int Money
    {
        get => _money; 
        set => _money = value;
    }

    private IPlayerService _playerService;
    private IItemFactory _itemFactory;

    [Inject]
    private void Construct(IPlayerService playerManager,
                           IItemFactory itemFactory)
    {
        _playerService = playerManager;
        _itemFactory = itemFactory;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        _playerService.ApplyGravity();
        _playerService.ApplyMovement();
        _playerService.ApplyRotation();
        _playerService.ApplyMovementObject();
    }

    public void PickupItem(GameObject heldItem)
    {
        Item item = heldItem.GetComponent<ItemController>().Item;
        if(!InventoryController.GetInstance().AddItemToMainInventory(item.Copy(), item.Amount)) return;

        heldItem.GetComponent<ItemController>().DestroyItem();
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
        _heldItem.transform.parent = null;
        _heldRigidbodyItem = null;
        _heldItem = null;

        StartCoroutine(HideInstrument());
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
            _heldItem.DestroyItem();
            _heldItem = null;
        }
    }

    public void ClearHeldInstrument()
    {
        if (_heldItem != null)
        {
            _heldItem.DestroyItem();
            _heldItem = null;
        }
    }

    private IEnumerator ChangingInstrument(Item newItem)
    {
        if (newItem is Funnel)
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
            _heldItem.DestroyItem();
            _heldItem = null;
        }

        GameObject heldItem = _itemFactory.Create(newItem.Object.gameObject);

        _heldRigidbodyItem = heldItem.GetComponent<Rigidbody>();
        _heldRigidbodyItem.isKinematic = true;
        _heldRigidbodyItem.transform.parent = _hand;

        _heldItem = heldItem.GetComponent<ItemController>();
        _heldItem.Item = newItem;
        _heldItem.IsUpdating = true;
        _heldItem.gameObject.layer = LayerMask.NameToLayer(LayerConstants.IGNORE_REYCAST);
        _heldItem.transform.localPosition = newItem.Object.transform.localPosition;
        _heldItem.transform.localRotation = newItem.Object.transform.localRotation;
        //if(_heldItem.Item is Funnel)
        //{
        //    _hands.SetBool("_IsHoldFunnel", true);
        //    _hands.SetBool("_IsHoldInst", false);
        //}
        //else
        //{
        //    _hands.SetBool("_IsHoldInst", true);
        //    _hands.SetBool("_IsHoldFunnel", false);
        //}

        yield return new WaitForSeconds(0.1f);

        _hands.SetBool("_IsChangingInst", false);
    }
}
