using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private float _velocity;
    private Vector2 _inputMovement;
    private Vector3 _direction;
    private Vector3 _rotationCamera;
    private Vector3 _rotationPlayer;
    private GameObject _heldObject;
    private GameObject _heldInstrument;
    private Rigidbody _heldRigidbody;
    private Rigidbody _heldRigidbodyInstrument;
    private CharacterController _chController;

    [Header("Player object")]
    [SerializeField] private Transform _holdArea;
    [SerializeField] private Transform _hand;

    public static PlayerController GetInstance() => instance;
    public bool HoldingObject() => _heldObject != null;
    public bool HoldingInstrument() => _heldInstrument != null;
    private bool IsGrounded() => _chController.isGrounded;

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

    public void PickupInstrument(GameObject heldInstrument)
    {
        if (_heldInstrument != null) return;

        _heldRigidbodyInstrument = heldInstrument.GetComponent<Rigidbody>();

        _heldRigidbodyInstrument.isKinematic = true;
        _heldRigidbodyInstrument.transform.parent = _hand;
        _heldInstrument = heldInstrument;
        _heldInstrument.transform.localPosition = Vector3.zero;
        _heldInstrument.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void DropInstrument()
    {
        if (_heldInstrument == null) return;

        _heldRigidbodyInstrument = _heldInstrument.GetComponent<Rigidbody>();
        _heldRigidbodyInstrument.isKinematic = false;
        _heldInstrument.transform.parent = null;
        _heldRigidbodyInstrument = null;
        _heldInstrument = null;
    }
}
