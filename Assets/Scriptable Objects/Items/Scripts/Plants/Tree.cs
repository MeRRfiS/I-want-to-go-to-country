using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "New Seed Object", menuName = "Inventory System/Items/Tree")]
public class Tree : Plant
{
    private GameObject _saplingObj;
    private GameObject _treeObj;
    private List<Material> _materials;
    private TreeChecking _treeCheck;
    public TreeTypeEnum _treeType;

    private bool IsSaplingObjNull() => _saplingObj == null;

    public override void Init()
    {
        _materials = new List<Material>();
    }

    public override void UseItem()
    {
        if (IsSaplingObjNull()) return;
        if (_treeCheck.IsOnObject || _treeCheck.IsNearTree) return;

        _amount--;
        _saplingObj.layer = LayerMask.NameToLayer(LayerConstants.TREE);
        for (int i = 0; i < _treeObj.GetComponent<Renderer>().materials.Length; i++)
        {
            _treeObj.GetComponent<Renderer>().materials[i].color = new Color(
                                       _materials[i].color.r,
                                       _materials[i].color.g,
                                       _materials[i].color.b,
                                       1);
        }
        _treeObj.GetComponent<Animator>().enabled = true;
        var tree = Instantiate(_saplingObj);
        Destroy(tree.GetComponent<TreeChecking>());
        _treeCheck = null;
        _treeObj = null;
        _saplingObj = null;
    }

    public override GameObject Updating(GameObject obj, GameObject prefab)
    {
        _saplingObj = obj;
        if (!IsSaplingObjNull())
        {
            _treeCheck = obj.GetComponent<TreeChecking>();
            _treeObj = obj.transform.GetChild(0).gameObject;
        }

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, 
                            startPoint.forward, 
                            out hit, 
                            ItemConstants.MAX_DISTANCE_TO_EARTH) &&
            hit.collider.CompareTag(TagConstants.EARTH))
        {
            if (IsSaplingObjNull())
            {
                _saplingObj = Instantiate(prefab);
                _treeCheck = _saplingObj.GetComponent<TreeChecking>();
                _treeObj = _saplingObj.transform.GetChild(0).gameObject;
                for (int i = 0; i < _treeObj.GetComponent<Renderer>().materials.Length; i++)
                {
                    _materials.Add(new Material(_treeObj.GetComponent<Renderer>().materials[i]));
                }
            }

            _saplingObj.transform.position = new Vector3(hit.point.x,
                                                        1,
                                                        hit.point.z);

            if (_treeCheck && (_treeCheck.IsOnObject || _treeCheck.IsNearTree))
            {
                for (int i = 0; i < _treeObj.GetComponent<Renderer>().materials.Length; i++)
                {
                    _treeObj.GetComponent<Renderer>().materials[i].color = new Color(
                                               1,
                                               _treeObj.GetComponent<Renderer>().materials[i].color.g,
                                               _treeObj.GetComponent<Renderer>().materials[i].color.b,
                                               0.5f);
                }
            }
            else if (_treeCheck)
            {
                for (int i = 0; i < _treeObj.GetComponent<Renderer>().materials.Length; i++)
                {
                    _treeObj.GetComponent<Renderer>().materials[i].color = new Color(
                                               _materials[i].color.r,
                                               _materials[i].color.g,
                                               _materials[i].color.b,
                                               0.5f);
                }
            }
        }
        else
        {
            _saplingObj = this.StopUpdating();
        }

        return _saplingObj;
    }

    public override GameObject StopUpdating()
    {
        if (!IsSaplingObjNull()) Destroy(_saplingObj);
        _saplingObj = null;

        return _saplingObj;
    }
}
