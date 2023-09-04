using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    private int collectTimes = 0;
    private int chopTreeTime = 0;
    private int harvestAmount = 1;
    [Header("Materials")]
    [SerializeField] private Material _earthWithFertilize;
    [Header("Setting")]
    [SerializeField] private PlantTypeEnum _type;
    [Header("Components")]
    [SerializeField] private Item _harvest;
    [SerializeField] private Item _logs;
    [SerializeField] private PlantsHandler _plant;
    [SerializeField] private GameObject _icon;

    public bool IsCanChoppingTree() => _plant.IsPlantGrow || _plant.IsPlantDry;
    public bool IsPlantNeedWater() => _plant.IsNeedWater;

    private void Update()
    {
        ApplyWaterIcon();
        ApplyFertilize();
    }

    private void ApplyWaterIcon()
    {
        _icon.SetActive(_plant.IsNeedWater);
    }

    private void ApplyFertilize()
    {
        if (!_plant.IsFertilized) return;

        switch (gameObject.tag)
        {
            case TagConstants.PLANT:
                Renderer earth = transform.GetComponentInParent<Renderer>();
                earth.material = _earthWithFertilize;
                break;
        }
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
            InventoryController.GetInstance().AddItemToMainInventory(_harvest.Copy(), harvestAmount);

            Destroy(transform.parent.gameObject);
        }
        if (_plant.IsPlantDry)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void TreeHarvesting()
    {
        if (_plant.IsFruitsGrow)
        {
            int fruitCount = Random.Range(MechConstants.MIN_TREE_HARVEST,
                                          MechConstants.MAX_TREE_HARVEST);
            switch (_plant.TreeType)
            {
                case TreeTypeEnum.None: break;
                case TreeTypeEnum.Default:
                    collectTimes += _plant.CollectFruit();
                    if (collectTimes == MechConstants.MAX_COUNT_OF_HARVEST)
                    {
                        _plant.PlantDead();
                    }
                    break;
            }
            InventoryController.GetInstance().AddItemToMainInventory(_harvest.Copy(), fruitCount);
        }
    }

    public void Watering()
    {
        _plant.WateringPlant();
    }

    public void Fertilizering(int level)
    {
        switch (_type)
        {
            case PlantTypeEnum.Normal:
                harvestAmount = level + 1;
                break;
            case PlantTypeEnum.Special:
                if (level < 2) return;

                harvestAmount = level;
                break;
            case PlantTypeEnum.Rare:
                if (level < 3) return;

                harvestAmount = level - 1;
                break;
            case PlantTypeEnum.VeryRare:
                if (level != 4) return;

                harvestAmount = level - 2;
                break;
            default:
                break;
        }

        _plant.FertilizeringPlant();
    }

    public void ChoppingTree(int hitCount)
    {
        chopTreeTime++;
        if(chopTreeTime == hitCount)
        {
            int logCount = Random.Range(MechConstants.MIN_TREE_LOG,
                                        MechConstants.MAX_TREE_LOG);
            InventoryController.GetInstance().AddItemToMainInventory(_logs.Copy(), logCount);
            Destroy(gameObject);
        }
    }
}
