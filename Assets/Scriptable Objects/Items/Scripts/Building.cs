using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Build Object", menuName = "Inventory System/Items/Build")]
public class Building: Item
{
    [Header("Building Prefabs")]
    [SerializeField] private GameObject _buildingVisualizationObj;
    [SerializeField] private GameObject _buildingPrefab;

    private GameObject _itemObj;
    private Renderer _buildingRenderer;
    private List<Material> _materials;
    private BuildChecking _buildingCheck;

    private bool IsItemObjNull() => _itemObj == null;

    public override void Init()
    {
        Amount = 1;
        _materials = new List<Material>();
    }

    public override void UseItem()
    {
        if (IsItemObjNull()) return;
        if (_buildingCheck.IsOnObject) return;

        Amount = 0;
        var build = Instantiate(_buildingPrefab);
        build.transform.position = new Vector3(_itemObj.transform.position.x,
                                               1.1f,
                                               _itemObj.transform.position.z);
        Destroy(_itemObj);

        _buildingCheck = null;
        _itemObj = null;
    }

    public override GameObject Updating(GameObject obj, GameObject prefab)
    {
        if (!IsItemObjNull())
        {
            _buildingCheck = _itemObj.GetComponent<BuildChecking>();
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
                _itemObj = Instantiate(_buildingVisualizationObj);
                _buildingCheck = _itemObj.GetComponent<BuildChecking>();
                _buildingRenderer = _itemObj.transform.GetChild(0).GetComponent<Renderer>();
                for (int i = 0; i < _buildingRenderer.materials.Length; i++)
                {
                    _materials.Add(new Material(_buildingRenderer.materials[i]));
                }
            }

            Vector3 point = Vector3.zero;
            point = new Vector3(_buildingCheck.IsVerFasten ? _buildingCheck.FastenPos.x : hit.point.x,
                                1,
                                _buildingCheck.IsHorFasten ? _buildingCheck.FastenPos.z : hit.point.z);
            if (Vector3.Distance(point, hit.point) >= MechConstants.MAX_DISTANCE_FOR_FASTEN_BUILDING)
            {
                _buildingCheck.ResetFastenChecking();
                point = hit.point;
            }
            _itemObj.transform.position = new Vector3(point.x,
                                                      1.1f,
                                                      point.z);

            if (_buildingCheck && _buildingCheck.IsOnObject)
            {
                for (int i = 0; i < _buildingRenderer.materials.Length; i++)
                {
                    _buildingRenderer.materials[i].color = new Color(1,
                                                                     _materials[i].color.g,
                                                                     _materials[i].color.b,
                                                                     0.5f);
                }
            }
            else if (_buildingCheck)
            {
                for (int i = 0; i < _buildingRenderer.materials.Length; i++)
                {
                    _buildingRenderer.materials[i].color = new Color(_materials[i].color.r,
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
