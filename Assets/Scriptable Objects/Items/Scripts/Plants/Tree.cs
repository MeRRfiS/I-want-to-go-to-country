using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "New Seed Object", menuName = "Inventory System/Items/Tree")]
public class Tree : Plant
{
    [Header("Tree Prefab")]
    [SerializeField] private GameObject _treeVisualizationObj;
    [SerializeField] private GameObject _treePrefab;

    private GameObject _saplingObj;
    private Renderer _saplingRenderer;
    private List<Material> _materials;
    private TreeChecking _treeCheck;

    private bool IsSaplingObjNull() => _saplingObj == null;

    public override void Init()
    {
        _materials = new List<Material>();
    }

    public override void UseItem()
    {
        if (IsSaplingObjNull()) return;
        if (_treeCheck.IsOnObject || _treeCheck.IsNearTree) return;

        Amount--;
        var tree = Instantiate(_treePrefab);
        tree.transform.position = new Vector3(_saplingObj.transform.position.x,
                                              0.55f,
                                              _saplingObj.transform.position.z);

        Destroy(_saplingObj);
        _treeCheck = null;
        _saplingObj = null;
    }

    public override GameObject Updating(GameObject obj, GameObject prefab)
    {
        if (!IsSaplingObjNull())
        {
            _treeCheck = _saplingObj.GetComponent<TreeChecking>();
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
                _saplingObj = Instantiate(_treeVisualizationObj);
                _saplingRenderer = _saplingObj.GetComponent<Renderer>();
                _treeCheck = _saplingObj.GetComponent<TreeChecking>();
                for (int i = 0; i < _saplingRenderer.materials.Length; i++)
                {
                    _materials.Add(new Material(_saplingRenderer.materials[i]));
                }
            }

            _saplingObj.transform.position = new Vector3(hit.point.x,
                                                        1.1f,
                                                        hit.point.z);

            if (_treeCheck && (_treeCheck.IsOnObject || _treeCheck.IsNearTree))
            {
                for (int i = 0; i < _saplingRenderer.materials.Length; i++)
                {
                    _saplingRenderer.materials[i].color = new Color(
                                               1,
                                               _saplingObj.GetComponent<Renderer>().materials[i].color.g,
                                               _saplingObj.GetComponent<Renderer>().materials[i].color.b,
                                               0.8f);
                }
            }
            else if (_treeCheck)
            {
                for (int i = 0; i < _saplingRenderer.materials.Length; i++)
                {
                    _saplingRenderer.materials[i].color = new Color(
                                               _materials[i].color.r,
                                               _materials[i].color.g,
                                               _materials[i].color.b,
                                               0.8f);
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
