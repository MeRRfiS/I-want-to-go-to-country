using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Seed Object", menuName = "Inventory System/Items/Seed")]
public class Seed : Plant
{
    [field: SerializeField] public GameObject Plant { get; private set; }

    public override void UseItem()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, 
                            startPoint.forward, 
                            out hit, 
                            MechConstants.MAX_DISTANCE_FOR_USING_ITEM))
        {
            Transform hitTransform = hit.collider.gameObject.transform;
            if (!hitTransform.CompareTag(TagConstants.SEEDBED)) return;
            if (hitTransform.childCount > 3) return;

            Amount--;
            GameObject plantObj = MonoBehaviour.Instantiate(Plant, hitTransform);
            plantObj.transform.localPosition = new Vector3(0, 0.0f, 0);
        }
    }
}
