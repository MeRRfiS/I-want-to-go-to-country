using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    [SerializeField] private Plant _plant;
    [SerializeField] private GameObject _icon;

    private void Update()
    {
        ApplyWaterIcon();
    }

    private void ApplyWaterIcon()
    {
        _icon.SetActive(_plant.IsNeedWater);
    }

    public void SetPlantType(PlantTypeEnum plantType)
    {
        _plant.PlantType = plantType;
    }

    public void Harvesting()
    {
        if (_plant.IsPlantGrow)
        {
            //...

            Destroy(gameObject);
        }
        if (_plant.IsPlantDry) 
        {
            Destroy(gameObject);
        }
    }

    public void Watering()
    {
        if (!_plant.IsNeedWater) return;

        _plant.GetWater();
    }
}
