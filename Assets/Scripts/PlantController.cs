using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    private int collectTimes = 0;
    private int chopTreeTime = 0;
    private int harvestCount = 1;
    [Header("Materials")]
    [SerializeField] private Material _earthWithFertilize;
    [Header("Setting")]
    [SerializeField] private PlantTypeEnum _type;
    [Header("Components")]
    [SerializeField] private Plant _plant;
    [SerializeField] private GameObject _icon;

    public bool IsCanChoppingTree() => _plant.IsPlantGrow || _plant.IsPlantDry;

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
            Item item = new Item();
            item.Count = harvestCount;
            item.Type = ItemTypeEnum.Harvest;
            switch (_plant.SeedType)
            {
                case SeedTypeEnum.None: break;
                case SeedTypeEnum.Default:
                    item.Id = (int)ItemIdsEnum.Harvest_Default;
                    break;
            }
            InventoryController.GetInstance().AddItem(item, item.Count);

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
            Item item = new Item();
            item.Count = Random.Range(MechConstants.MIN_TREE_HARVEST,
                                      MechConstants.MAX_TREE_HARVEST);
            item.Type = ItemTypeEnum.Harvest;
            switch (_plant.TreeType)
            {
                case TreeTypeEnum.None: break;
                case TreeTypeEnum.Default:
                    item.Id = (int)ItemIdsEnum.Harvest_Default;
                    collectTimes += _plant.CollectFruit();
                    if (collectTimes == MechConstants.MAX_COUNT_OF_HARVEST)
                    {
                        _plant.PlantDead();
                    }
                    return;
            }
            InventoryController.GetInstance().AddItem(item, item.Count);
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
                harvestCount = level + 1;
                break;
            case PlantTypeEnum.Special:
                if (level < 2) return;

                harvestCount = level;
                break;
            case PlantTypeEnum.Rare:
                if (level < 3) return;

                harvestCount = level - 1;
                break;
            case PlantTypeEnum.VeryRare:
                if (level != 4) return;

                harvestCount = level - 2;
                break;
            default:
                break;
        }

        _plant.FertilizeringPlant();
    }

    public void ChoppingTree(int hitCount)
    {
        chopTreeTime++;
        Debug.Log(chopTreeTime);
        if(chopTreeTime == hitCount)
        {
            Destroy(gameObject);
        }
    }
}
