using UnityEngine;

public class TreeHint : ItemUseHint
{
    [SerializeField] private HintData _chopData;
    [SerializeField] private HintData _harvestData;
    [SerializeField] private HintData _deadData;
    [SerializeField] private TreesController _tree;
    [SerializeField] private Item[] _choppingRequiredItem;

    public override bool IsActive()
    {
        if (_tree.IsDead)
        {
            return true;
        }
        if (_tree.IsPlantNeedWater())
        {
            return true;
        }
        else
        {
            return _tree.IsCanChoppingTree();
        }
    }

    public override bool IsAlwaysUpdate()
    {
        return true;
    }

    public override string GetText()
    {
        if (_tree.IsDead)
        {
            return CombineWithApple(_deadData.GetText());
        }
        if (_tree.IsPlantNeedWater())
        {
            if (base.IsActive())
            {
                return CombineWithApple(base.GetText());
            }
            else
            {
                return CombineWithApple("Need to water");
            }
        }
        else
        {
            if (IsItemInHand(_choppingRequiredItem))
            {
                
                return CombineWithApple(_chopData.GetText(_tree.ResultItem.name));
            }
            else
            {
                return CombineWithApple("Need axe");
            }
        }
    }

    public string CombineWithApple(string firstText)
    {
        if(_tree.IsCanHarvestApple())
        {
            string nextText = _harvestData.GetText(_tree.ResultItem.name);
            return $"{firstText}\n{nextText}";
        }
        return firstText;
    }
}