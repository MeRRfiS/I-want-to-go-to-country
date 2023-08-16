using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Item
{
    private GameObject saplingObj;
    private SeedTypeEnum _type;

    public Seed(SeedTypeEnum type)
    {
        _type = type;
        Count = 5;
        IsCanSold = false;
        switch (type)
        {
            case SeedTypeEnum.None:
                break;
            case SeedTypeEnum.Default:
                Price = 10;
                break;
        }
        Id = (int)Enum.Parse(typeof(ItemIdsEnum),
                             $"Seed_{type.ToString()}");
    }

    public override void UseItem()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.MAX_DISTANCE_FOR_USING_ITEM))
        {
            Transform hitTransform = hit.collider.gameObject.transform;
            if (!hitTransform.CompareTag(TagConstants.PATCH)) return;
            if (hitTransform.childCount > 0) return;

            Count--;
            GameObject plant = Resources.Load(ResourceConstants.PLANTS + ((int)_type).ToString()) as GameObject;
            GameObject plantObj = MonoBehaviour.Instantiate(plant, hitTransform);
            plantObj.GetComponent<PlantController>().SetSeedType(_type);
            plantObj.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
    }
}
