using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    private int collectTimes = 0;
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

    

    public void SetSeedType(SeedTypeEnum seedType)
    {
        _plant.SeedType = seedType;
    }

    public void SetTreeType(TreeTypeEnum treeType)
    {
        _plant.TreeType = treeType;
    }

    public void PatchHarvesting()
    {
        if (_plant.IsPlantGrow)
        {
            switch (_plant.SeedType)
            {
                case SeedTypeEnum.None: break;
                case SeedTypeEnum.Default:
                    break;
            }

            Destroy(gameObject);
        }
        if (_plant.IsPlantDry)
        {
            Destroy(gameObject);
        }
    }

    public void TreeHarvesting()
    {
        if (_plant.IsFruitsGrow)
        {
            switch (_plant.TreeType)
            {
                case TreeTypeEnum.None: break;
                case TreeTypeEnum.Default:
                    collectTimes += _plant.CollectFruit();
                    if (collectTimes == MechConstants.MAX_COUNT_OF_HARVEST)
                    {
                        _plant.PlantDead();
                    }
                    return;
            }
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
