using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlantController : MonoBehaviour
{
    private const string WAIT_WATER_COR = "WaitWater";
    private const string SHADER_PATH = "HDRP/Autodesk Interactive/AutodeskInteractive";
    private const string USE_COLOR_MAP = "_UseColorMap";

    private bool _isNeedWater = false;
    private bool _isPlantDry = false;
    private bool _isPlantGrow = false;
    private bool _isFertilized = false;
    private int _harvestAmount = 1;

    [Header("Materials")]
    [SerializeField] private Material _earthWithFertilize;

    [Header("Setting")]
    [SerializeField] private PlantTypeEnum _type;

    [Header("Components")]
    [SerializeField] private List<GameObject> _plantsLevel;
    [SerializeField] private Item _harvest;
    [SerializeField] private GameObject _icon;
    [SerializeField] private PlayableDirector _player;

    public bool IsPlantNeedWater() => _isNeedWater;

    private void Update()
    {
        ApplyWaterIcon();
        ApplyFertilize();
    }

    private void ApplyWaterIcon()
    {
        _icon.SetActive(_isNeedWater);
    }

    private void ApplyFertilize()
    {
        if (_isFertilized) return;

        //switch (gameObject.tag)
        //{
        //    case TagConstants.PLANT:
        //        Renderer earth = transform.GetComponentInParent<Renderer>();
        //        earth.material = _earthWithFertilize;
        //        break;
        //}
    }

    public void PatchHarvesting()
    {
        if (_isPlantGrow)
        {
            InventoryController.GetInstance().AddItemToMainInventory(_harvest.Copy(), _harvestAmount);

            Destroy(transform.parent.gameObject);
        }
        if (_isPlantDry)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void Watering()
    {
        if (!_isNeedWater) return;
        if (_isPlantDry) return;

        StopCoroutine(WAIT_WATER_COR);
        _isNeedWater = false;

        if (!_isPlantGrow)
        {
            _player.Play();
        }
    }

    public void Fertilizering(int level)
    {
        switch (_type)
        {
            case PlantTypeEnum.Normal:
                _harvestAmount = level + 1;
                break;
            case PlantTypeEnum.Special:
                if (level < 2) return;

                _harvestAmount = level;
                break;
            case PlantTypeEnum.Rare:
                if (level < 3) return;

                _harvestAmount = level - 1;
                break;
            case PlantTypeEnum.VeryRare:
                if (level != 4) return;

                _harvestAmount = level - 2;
                break;
            default:
                break;
        }

        FertilizeringPlant();
    }

    public void PlantDead()
    {
        Destroy(_player);

        var renderer = GetActiveLOD();
        renderer.material.shader = Shader.Find(SHADER_PATH);
        renderer.material.SetInt(USE_COLOR_MAP, 0);

        _isNeedWater = false;
        _isPlantDry = true;
    }

    public void NeedWater()
    {
        _isNeedWater = true;
        _player.Pause();
        StartCoroutine(WAIT_WATER_COR);
    }

    public void PlantGrew()
    {
        _isPlantGrow = true;
        _player.Pause();
    }

    private Renderer GetActiveLOD()
    {
        Renderer result = null;
        GameObject activeLevel = null;

        foreach (GameObject plant in _plantsLevel)
        {
            if (plant.activeSelf)
            {
                activeLevel = plant;
                break;
            }
        }

        LODGroup lodGroup = activeLevel.GetComponent<LODGroup>();
        Transform lodTransform = lodGroup.transform;

        foreach (Transform child in lodTransform)
        {
            result = child.GetComponent<Renderer>();
            if (result != null && result.isVisible) break;
        }

        return result;
    }

    private void FertilizeringPlant()
    {
        if (_isFertilized) return;
        if (_isPlantGrow) return;

        _isFertilized = true;
    }

    private IEnumerator WaitWater()
    {
        yield return new WaitForSeconds(MechConstants.WAIT_TIME_FOR_WATER);
        PlantDead();
    }
}
