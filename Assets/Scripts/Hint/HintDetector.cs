using UnityEngine;

public class HintDetector : MonoBehaviour
{
    [SerializeField] private HintDisplay _hintDisplay;

    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);

        if (Physics.Raycast(ray, out var hit, PlayerConstants.DISTANCE_TO_OBJECT) && hit.transform.TryGetComponent<HintBase>(out var hint))
        {
            _hintDisplay.Show(hint);
        }
        else
        {
            _hintDisplay.Hide();
        }
    }
}