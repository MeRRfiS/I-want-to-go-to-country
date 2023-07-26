using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Item
{
    private GameObject saplingObj;
    private TreeChecking treeCheck;
    private SeedTypeEnum _type;

    private bool IsSaplingObjNull() => saplingObj == null;

    public Seed(SeedTypeEnum type)
    {
        _type = type;
        Id = (int)Enum.Parse(typeof(ItemIdsEnum),
                             $"Seed_{type.ToString()}");
    }

    public override void Use()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.DISTANCE_FOR_PLANT))
        {
            Transform hitTransform = hit.collider.gameObject.transform;
            if (!hitTransform.CompareTag(TagConstants.PATCH)) return;
            if (hitTransform.childCount > 0) return;

            GameObject plant = Resources.Load(ResourceConstants.PLANTS + ((int)_type).ToString()) as GameObject;
            GameObject plantObj = MonoBehaviour.Instantiate(plant, hitTransform);
            plantObj.GetComponent<PlantController>().SetSeedType(_type);
            plantObj.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
    }
}
