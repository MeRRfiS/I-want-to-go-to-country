using TMPro;
using UnityEngine;

public class HintDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _text;

    private Hint _currentHint;
    private bool _visible = false;

    public void Show(Hint hint)
    {
        if (hint == _currentHint || _visible == true) return;
        
        _text.text = hint.GetText();
        _panel.SetActive(true);
        _visible = true;
        _currentHint = hint;
    }

    public void Hide()
    {
        if (_visible) return;

        _panel.SetActive(false);
        _visible = false;
    }
}