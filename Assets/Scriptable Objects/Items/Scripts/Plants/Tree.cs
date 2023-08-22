using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "New Seed Object", menuName = "Inventory System/Items/Tree")]
public class Tree : Item
{
    private GameObject saplingObj;
    private GameObject treeObj;
    private List<Material> materials = new List<Material>();
    private TreeChecking treeCheck;
    public TreeTypeEnum _treeType;

    private bool IsSaplingObjNull() => saplingObj == null;

    public override void UseItem()
    {
        if (IsSaplingObjNull()) return;
        if (treeCheck.IsOnObject || treeCheck.IsNearTree) return;

        _amount--;
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
        var tree = Instantiate(saplingObj);
        tree.GetComponent<PlantController>().SetTreeType(_treeType);
        Destroy(tree.GetComponent<TreeChecking>());
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
                saplingObj = Instantiate(prefab);
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
        if (!IsSaplingObjNull()) Destroy(saplingObj);
        saplingObj = null;

        return saplingObj;
    }
}
