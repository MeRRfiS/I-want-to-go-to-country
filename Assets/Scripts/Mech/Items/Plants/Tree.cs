using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Tree : Item
{
    private GameObject saplingObj;
    private GameObject treeObj;
    private List<Material> materials = new List<Material>();
    private TreeChecking treeCheck;
    private TreeTypeEnum _type;

    private bool IsSaplingObjNull() => saplingObj == null;

    public Tree(TreeTypeEnum type)
    {
        _type = type;
        Id = (int)Enum.Parse(typeof(ItemIdsEnum),
                             $"Tree_{type.ToString()}");
    }

    public override void Use()
    {
        if (IsSaplingObjNull()) return;
        if (treeCheck.IsOnObject || treeCheck.IsNearTree) return;

        saplingObj.layer = LayerMask.NameToLayer(LayerConstants.TREE);
        for (int i = 0; i < treeObj.GetComponent<Renderer>().materials.Length; i++)
        {
            treeObj.GetComponent<Renderer>().materials[i].color = new Color(
                                       materials[i].color.r,
                                       materials[i].color.g,
                                       materials[i].color.b,
                                       1);
        }
        treeObj.GetComponent<Animator>().enabled = true;
        var tree = MonoBehaviour.Instantiate(saplingObj);
        tree.GetComponent<PlantController>().SetTreeType(_type);
        MonoBehaviour.Destroy(tree.GetComponent<TreeChecking>());
        treeCheck = null;
        treeObj = null;
        saplingObj = null;
    }

    public override GameObject Updating(GameObject obj, GameObject prefab)
    {
        saplingObj = obj;
        if (!IsSaplingObjNull())
        {
            treeCheck = obj.GetComponent<TreeChecking>();
            treeObj = obj.transform.GetChild(0).gameObject;
        }

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, 
                            startPoint.forward, 
                            out hit, 
                            InstrumentConstants.MAX_DISTANCE_TO_EARTH) &&
            hit.collider.CompareTag(TagConstants.EARTH))
        {
            if (IsSaplingObjNull())
            {
                saplingObj = MonoBehaviour.Instantiate(prefab);
                treeCheck = saplingObj.GetComponent<TreeChecking>();
                treeObj = saplingObj.transform.GetChild(0).gameObject;
                for (int i = 0; i < treeObj.GetComponent<Renderer>().materials.Length; i++)
                {
                    materials.Add(new Material(treeObj.GetComponent<Renderer>().materials[i]));
                }
            }

            saplingObj.transform.position = new Vector3(hit.point.x,
                                                        1,
                                                        hit.point.z);

            if (treeCheck && (treeCheck.IsOnObject || treeCheck.IsNearTree))
            {
                for (int i = 0; i < treeObj.GetComponent<Renderer>().materials.Length; i++)
                {
                    treeObj.GetComponent<Renderer>().materials[i].color = new Color(
                                               1,
                                               treeObj.GetComponent<Renderer>().materials[i].color.g,
                                               treeObj.GetComponent<Renderer>().materials[i].color.b,
                                               0.5f);
                }
            }
            else if (treeCheck)
            {
                for (int i = 0; i < treeObj.GetComponent<Renderer>().materials.Length; i++)
                {
                    treeObj.GetComponent<Renderer>().materials[i].color = new Color(
                                               materials[i].color.r,
                                               materials[i].color.g,
                                               materials[i].color.b,
                                               0.5f);
                }
            }
        }
        else
        {
            saplingObj = this.StopUpdating();
        }

        return saplingObj;
    }

    public override GameObject StopUpdating()
    {
        if (!IsSaplingObjNull()) MonoBehaviour.Destroy(saplingObj);
        saplingObj = null;

        return saplingObj;
    }
}
