using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Seed Object", menuName = "Inventory System/Items/Seed")]
public class Seed : Item
{
    public SeedTypeEnum _seedType;

    public override void UseItem()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.MAX_DISTANCE_FOR_USING_ITEM))
        {
            Transform hitTransform = hit.collider.gameObject.transform;
            if (!hitTransform.CompareTag(TagConstants.PATCH)) return;
            if (hitTransform.childCount > 0) return;

            _amount--;
            GameObject plant = Resources.Load(ResourceConstants.PLANTS + ((int)_seedType).ToString()) as GameObject;
            GameObject plantObj = MonoBehaviour.Instantiate(plant, hitTransform);
            plantObj.GetComponent<PlantController>().SetSeedType(_seedType);
            plantObj.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
    }
}
