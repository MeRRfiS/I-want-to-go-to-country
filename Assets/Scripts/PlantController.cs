using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    private int collectTimes = 0;
    private int chopTreeTime = 0;
    [SerializeField] private Plant _plant;
    [SerializeField] private GameObject _icon;

    public bool IsCanChoppingTree() => _plant.IsPlantGrow || _plant.IsPlantDry;

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
            Item item = new Item();
            item.Count = 1;
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
        if (!_plant.IsNeedWater) return;

        _plant.GetWater();
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
