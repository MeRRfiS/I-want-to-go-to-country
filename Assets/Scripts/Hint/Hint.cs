using UnityEngine;

public class Hint : MonoBehaviour
{
    [SerializeField] protected HintData _data;

    public virtual string GetText()
    {
        return _data.GetText();
    }
}