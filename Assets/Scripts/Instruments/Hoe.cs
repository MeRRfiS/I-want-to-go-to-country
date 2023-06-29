using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoe : Instrument
{
    private GameObject patchObj;
    private Patch patch;

    private bool IsPatchObjNull() => patchObj == null;

    public override GameObject CreateObj(GameObject obj = null, GameObject prefab = null)
    {
        patchObj = obj;
        if(!IsPatchObjNull()) patch = obj.GetComponent<Patch>();

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;
        Physics.Raycast(startPoint.position, startPoint.forward, out hit, InstrumentConstants.MAX_DISTANCE_TO_EARTH);

        if (hit.collider != null && hit.collider.tag == TagConstants.EARTH)
        {
            if (IsPatchObjNull()) patchObj = MonoBehaviour.Instantiate(prefab);

            patchObj.transform.position = new Vector3(hit.point.x,
                                                      0,
                                                      hit.point.z);

            if (patch && patch.IsOnObject)
            {
                patchObj.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.5f);
            }
            else if(patch)
            {
                patchObj.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {
            patchObj = DestroyObj(patchObj);
        }

        return patchObj;
    }

    public override GameObject DestroyObj(GameObject obj = null)
    {
        if (!IsPatchObjNull()) MonoBehaviour.Destroy(patchObj);
        patchObj = null;

        return patchObj;
    }

    public override void Use()
    {
        if (IsPatchObjNull()) return;
        if (patch.IsOnObject) return;

        patchObj.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
        patchObj.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
        MonoBehaviour.Instantiate(patchObj);
        patchObj = null;
    }
}
