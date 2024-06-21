using UnityEngine;

[CreateAssetMenu(fileName ="New Hint", menuName ="Hint")]
public class HintData : ScriptableObject
{
    [field: SerializeField] public string HintText { get; private set; }

    public string GetText(params object[] obj)
    {
        return string.Format(HintText, obj);
    }
}