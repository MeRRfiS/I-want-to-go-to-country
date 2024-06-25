using UnityEngine;

public class RobotHint : HintBase
{
    [SerializeField] private HintData _dropData;
    [SerializeField] private NPCController _npcController;

    public override bool IsAlwaysUpdate() => true;

    public override string GetText()
    {
        if (_npcController.IsHold)
        {
            return _dropData.GetText();
        }
        else
        {
            return base.GetText();
        }
    }
}