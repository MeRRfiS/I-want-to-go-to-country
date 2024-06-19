using UnityEngine;

public class BuildingHint : HintBase
{
    [SerializeField] private GameObject _building;

    public override string GetText()
    {
        return _data.GetText(_building.name);
    }
}