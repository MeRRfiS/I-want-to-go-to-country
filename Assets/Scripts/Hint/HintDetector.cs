using System.Collections.Generic;
using UnityEngine;

public sealed class HintDetector : MonoBehaviour
{
    [SerializeField] private HintDisplay _hintDisplay;

    private Transform _cameraTransform;
    private List<HintBase> _hints = new();

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out var hit, PlayerConstants.DISTANCE_TO_OBJECT))
        {
            hit.transform.GetComponentsInChildren(_hints);
        }

        foreach (var hint in _hints)
        {
            if (hint.IsActive())
            {
                _hintDisplay.Show(hint);
                _hints.Clear();
                return;
            }
        }

        _hintDisplay.Hide();
    }
}