using UnityEngine;

public class HintBase : MonoBehaviour
{
    [SerializeField] protected HintData _data;

    public virtual bool IsActive() => true;
    public virtual bool IsAlwaysUpdate() => false;

    public virtual string GetText()
    {
        return _data.GetText();
    }
}