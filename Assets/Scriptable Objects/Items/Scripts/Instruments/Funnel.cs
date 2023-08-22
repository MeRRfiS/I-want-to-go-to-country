using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instrument Object", menuName = "Inventory System/Items/Funnel")]
public class Funnel : Instrument
{
    public int _maxUsings;
    private int _usings;

    public int Usings
    {
        get => _usings;
    }

    public override void Init()
    {
        _usings = _maxUsings;
        _durability = _maxDurability;
        _amount = 1;
    }

    public Funnel(int level, int durability)
    {
        //_isCanSold = false;
        //_instrumentType = InstrumentTypeEnum.Funnel;
        //_level = level;
        //switch (level)
        //{
        //    case 1:
        //        _maxDurability = 100;
        //        _maxUnings = 2;
        //        _price = 50;
        //        break;
        //    case 2:
        //        _maxDurability = 150;
        //        _maxUnings = 4;
        //        _price = 150;
        //        break;
        //    case 3:
        //        _maxDurability = 200;
        //        _maxUnings = 6;
        //        _price = 400;
        //        break;
        //    case 4:
        //        _maxDurability = 300;
        //        _maxUnings = 8;
        //        _price = 800;
        //        break;
        //    case 5:
        //        _maxDurability = 400;
        //        _maxUnings = 10;
        //        _price = 1250;
        //        break;
        //}
    }

    public override void UseItem()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if(Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.MAX_DISTANCE_FOR_USING_ITEM)) 
        {
            GameObject hitObject = hit.collider.gameObject;
            switch (hitObject.tag)
            {
                case TagConstants.PLANT:
                case TagConstants.TREE:
                    if (Usings == 0) return;

                    _durability--;
                    _usings--;
                    hitObject.GetComponent<PlantController>().Watering();
                    break;
                case TagConstants.WELL:
                    _usings = _maxUsings;
                    break;
            }
        }
    }
}
