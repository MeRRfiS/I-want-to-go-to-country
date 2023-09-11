using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Build Object", menuName = "Inventory System/Items/Build")]
public class Building: Item
{

    private GameObject _itemObj;
    private GameObject _buildingObj;
    private List<Material> _materials;
    private BuildChecking _buildingCheck;

    private bool IsItemObjNull() => _itemObj == null;

    public override void Init()
    {
        _amount = 1;
        _materials = new List<Material>();
    }

    public override void UseItem()
    {
        if (IsItemObjNull()) return;
        if (_buildingCheck.IsOnObject) return;

        _amount = 0;
        _itemObj.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
        for (int i = 0; i < _buildingObj.GetComponent<Renderer>().materials.Length; i++)
        {
            _buildingObj.GetComponent<Renderer>().materials[i].color = new Color(
                                       _materials[i].color.r,
                                       _materials[i].color.g,
                                       _materials[i].color.b,
                                       1);
        }
        var build = Instantiate(_itemObj);
        Destroy(build.GetComponent<TreeChecking>());
        _buildingCheck = null;
        _buildingObj = null;
        _itemObj = null;
    }

    public override GameObject Updating(GameObject obj, GameObject prefab)
    {
        _itemObj = obj;
        if (!IsItemObjNull())
        {
            _buildingCheck = obj.GetComponent<BuildChecking>();
            _buildingObj = obj.transform.GetChild(0).gameObject;
        }

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position,
                            startPoint.forward,
                            out hit,
                            ItemConstants.MAX_DISTANCE_TO_EARTH_FOR_BUILDING) &&
            hit.collider.CompareTag(TagConstants.EARTH))
        {
            if (IsItemObjNull())
            {
                _itemObj = Instantiate(prefab);
                _buildingCheck = _itemObj.GetComponent<BuildChecking>();
                _buildingObj = _itemObj.transform.GetChild(0).gameObject;
                for (int i = 0; i < _buildingObj.GetComponent<Renderer>().materials.Length; i++)
                {
                    _materials.Add(new Material(_buildingObj.GetComponent<Renderer>().materials[i]));
                }
            }

            _itemObj.transform.position = new Vector3(hit.point.x,
                                                        2,
                                                        hit.point.z);

            if (_buildingCheck && _buildingCheck.IsOnObject)
            {
                for (int i = 0; i < _buildingObj.GetComponent<Renderer>().materials.Length; i++)
                {
                    _buildingObj.GetComponent<Renderer>().materials[i].color = new Color(
                                               1,
                                               _buildingObj.GetComponent<Renderer>().materials[i].color.g,
                                               _buildingObj.GetComponent<Renderer>().materials[i].color.b,
                                               0.5f);
                }
            }
            else if (_buildingCheck)
            {
                for (int i = 0; i < _buildingObj.GetComponent<Renderer>().materials.Length; i++)
                {
                    _buildingObj.GetComponent<Renderer>().materials[i].color = new Color(
                                               _materials[i].color.r,
                                               _materials[i].color.g,
                                               _materials[i].color.b,
                                               0.5f);
                }
            }
        }
        else
        {
            _itemObj = this.StopUpdating();
        }

        return _itemObj;
    }

    public override GameObject StopUpdating()
    {
        if (!IsItemObjNull()) Destroy(_itemObj);
        _itemObj = null;

        return _itemObj;
    }
}
