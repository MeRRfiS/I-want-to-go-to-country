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

        if (Physics.Raycast(ray, out var hit) && hit.transform.TryGetComponent<Hint>(out var hint))
        {
            _hintDisplay.Show(hint);
        }
        else
        {
            _hintDisplay.Hide();
        }
    }
}