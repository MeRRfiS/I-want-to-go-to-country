using TMPro;
using UnityEngine;

public sealed class HintDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _text;

    private HintBase _currentHint;
    private bool _visible = false;

    public void Show(HintBase hint)
    {
        if (hint == _currentHint && _visible == true && hint.IsAlwaysUpdate() == false) return;
        
        _text.text = hint.GetText();
        _panel.SetActive(true);
        _visible = true;
        _currentHint = hint;
    }

    public void Hide()
    {
        if (_visible == false) return;

        _panel.SetActive(false);
        _visible = false;
    }
}