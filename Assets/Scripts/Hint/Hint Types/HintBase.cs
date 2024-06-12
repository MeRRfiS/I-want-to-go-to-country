using UnityEngine;

public class HintBase : MonoBehaviour
{
    [SerializeField] protected HintData _data;

    public virtual string GetText()
    {
        return _data.GetText();
    }
}