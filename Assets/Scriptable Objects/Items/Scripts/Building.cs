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

    private const float ROTATION_SPEED = 20f;

    private static Quaternion _newRotation;
    private GameObject _itemObj;
    private Renderer _buildingRenderer;
    private List<Material> _materials;
    private BuildChecking _buildingCheck;

    private bool IsItemObjNull() => _itemObj == null;
    private bool IsPlacingBuilding() => true;

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
        build.transform.rotation = _itemObj.transform.GetChild(0).rotation;
        Destroy(_itemObj);

        UIController.GetInstance().ChangeBuildInfoActive(false);
        BuildInputSystem.OnPlacingBuilding -= IsPlacingBuilding;
        _buildingCheck = null;
        _itemObj = null;
    }

    public override void Updating()
    {
        if (!IsInHand)
        {
            StopUpdating();
            return;
        }

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position,
                            startPoint.forward,
                            out hit,
                            ItemConstants.MAX_DISTANCE_TO_EARTH_FOR_BUILDING) &&
            hit.collider.CompareTag(TagConstants.EARTH))
        {
            InstantiateVisual();
            SetPosition(hit);
            SetRotation();
            CheckBuildingPos();
        }
        else
        {
            _itemObj = this.StopUpdating();
        }
    }

    private void SetRotation()
    {
        if (BuildInputSystem.HoldingZButton)
        {
            _itemObj.transform.GetChild(0).Rotate(Vector3.up * ROTATION_SPEED * Time.deltaTime);
        }
        if (BuildInputSystem.HoldingXButton)
        {
            _itemObj.transform.GetChild(0).Rotate(-Vector3.up * ROTATION_SPEED * Time.deltaTime);
        }

        _newRotation = _itemObj.transform.GetChild(0).rotation;
    }

    private void CheckBuildingPos()
    {
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

    private void SetPosition(RaycastHit hit)
    {
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
    }

    private void InstantiateVisual()
    {
        if (IsItemObjNull())
        {
            BuildInputSystem.OnPlacingBuilding += IsPlacingBuilding;
            UIController.GetInstance().ChangeBuildInfoActive(true);

            _itemObj = Instantiate(_buildingVisualizationObj);
            _itemObj.transform.GetChild(0).rotation = _newRotation;
            _buildingCheck = _itemObj.transform.GetComponentInChildren<BuildChecking>();
            _buildingRenderer = _itemObj.transform.GetComponentInChildren<Renderer>();
            for (int i = 0; i < _buildingRenderer.materials.Length; i++)
            {
                _materials.Add(new Material(_buildingRenderer.materials[i]));
            }
        }
    }

    public override GameObject StopUpdating()
    {
        if (!IsItemObjNull()) Destroy(_itemObj);
        _itemObj = null;

        BuildInputSystem.OnPlacingBuilding -= IsPlacingBuilding;
        UIController.GetInstance().ChangeBuildInfoActive(false);

        return _itemObj;
    }
}
