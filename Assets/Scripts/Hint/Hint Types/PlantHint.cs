using UnityEngine;

public class PlantHint : ItemUseHint
{
    [SerializeField] private HintData _harvestData;
    [SerializeField] private HintData _deadData;
    [SerializeField] private PlantController _plant;

    public override bool IsActive()
    {
        if (_plant.IsDead)
        {
            return true;
        }
        if (_plant.IsPlantNeedWater())
        {
            return true;
        }
        else
        {
            return _plant.IsComplate();
        }
    }

    public override string GetText()
    {
        if (_plant.IsDead)
        {
            return _deadData.GetText();
        }
        if (_plant.IsPlantNeedWater())
        {
            if (base.IsActive())
            {
                return base.GetText();
            }
            else
            {
                return "Need to water";
            }
        }
        else
        {
            return _harvestData.GetText(_plant.ResultItem.name);
        }
    }
}