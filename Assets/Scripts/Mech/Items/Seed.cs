using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Item
{
    private PlantTypeEnum _type;

    public Seed(PlantTypeEnum type)
    {
        _type = type;
    }

    public override void Use()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.DISTANCE_FOR_PLANT))
        {
            GameObject gObject = hit.collider.gameObject;
            if (gObject.CompareTag(TagConstants.PATCH))
            {
                GameObject plant = Resources.Load(ResourceConstants.PLANTS + ((int)_type).ToString()) as GameObject;
                GameObject plantObj = MonoBehaviour.Instantiate(plant, gObject.transform);
                plantObj.GetComponent<Plant>().PlantType = _type;
                plantObj.transform.localPosition = new Vector3(0, 0.5f, 0);
            }
        }
        else
        {
            Debug.Log("Patch not found or it so far!");
        }
    }
}
