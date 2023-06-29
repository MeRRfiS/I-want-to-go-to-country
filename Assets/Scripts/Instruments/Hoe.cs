using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoe : Instrument
{
    private GameObject patch;

    public override GameObject CreateObj(GameObject obj = null, GameObject prefab = null)
    {
        patch = obj;
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;
        Physics.Raycast(startPoint.position, startPoint.forward, out hit, InstrumentConstants.MAX_DISTANCE_TO_EARTH);

        if (hit.collider != null && hit.collider.tag == TagConstants.EARTH)
        {
            if (patch == null) patch = MonoBehaviour.Instantiate(prefab);

            patch.transform.position = new Vector3(hit.point.x,
                                                    0,
                                                    hit.point.z);
        }
        else
        {
            patch = DestroyObj(patch);
        }

        return patch;
    }

    public override GameObject DestroyObj(GameObject obj = null)
    {
        if (patch != null) MonoBehaviour.Destroy(patch);
        patch = null;

        return patch;
    }

    public override void Use()
    {
        if (patch == null) return;

        patch.layer = LayerMask.NameToLayer("Default");
        patch.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
        MonoBehaviour.Instantiate(patch);
    }
}
