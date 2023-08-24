using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Instrument Object", menuName = "Inventory System/Items/Axe")]
public class Axe : Instrument 
{
    private PlantController tree = null;

    public int _hitCount;
    public int _timeChop;

    public override void Init()
    {
        if (_durability == 0)
        {
            _durability = _maxDurability;
        }
        _amount = 1;
    }

    private void ChopTree()
    {
        tree.ChoppingTree(_hitCount);
        _durability--;
    }

    public override void UseItem()
    {
        if (tree == null) return;

        if (tree.IsCanChoppingTree())
        {
            UIController.GetInstance().ProgressBar(_timeChop, ChopTree);
        }
    }

    public override GameObject Updating(GameObject obj, GameObject prefab)
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position,
                           startPoint.forward,
                           out hit,
                           MechConstants.MAX_DISTANCE_FOR_USING_ITEM))
        {
            Transform hitObj = hit.transform;

            if (hitObj.CompareTag(TagConstants.TREE))
            {
                tree = hitObj.GetComponent<PlantController>();
            }
            else
            {
                tree = null;
                UIController.GetInstance().StopProgressBar();
            }
        }
        else
        {
            tree = null;
            UIController.GetInstance().StopProgressBar();
        }

        return base.Updating(obj, prefab);
    }

    public override void Destruct()
    {
        _durability = 0;
    }
}
