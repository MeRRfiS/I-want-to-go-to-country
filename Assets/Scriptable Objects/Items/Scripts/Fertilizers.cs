using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fertilizers Object", menuName = "Inventory System/Items/Fertilizers")]
public class Fertilizers: Item
{
    public int _level;
    private int _usings;

    public int Usings
    {
        get => _usings;
    }

    public override void Init()
    {
        _usings = MechConstants.MAX_USING_OF_FERTILIZER;
    }

    public Fertilizers(int level) 
    {
        _level = level;
        _price = 250;
        _id = (int)Enum.Parse(typeof(ItemIdsEnum),
                             $"Fertilizers_{level}");
        _isCanSold = false;
    }

    public override bool IsItemCountZero()
    {
        if (Usings == 0) return true;

        return false;
    }

    public override void UseItem()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.MAX_DISTANCE_FOR_USING_ITEM))
        {
            Transform hitTransform = hit.collider.gameObject.transform;
            if (!hitTransform.CompareTag(TagConstants.PLANT)) return;

            _usings--;
            PlantController plant = hitTransform.GetComponent<PlantController>();
            plant.Fertilizering(_level);
        }
    }
}
