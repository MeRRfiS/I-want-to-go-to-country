using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(SignalReceiver))]
[RequireComponent(typeof(BoxCollider))]
public class PlantController : MonoBehaviour
{
    private const string WAIT_WATER_COR = "WaitWater";
    private const string SHADER_COLOR_VAR = "BaseColor";

    private bool _isNeedWater = false;
    private bool _isPlantDry = false;
    private bool _isPlantGrow = false;
    private bool _isFertilized = false;
    private int _harvestAmount = 1;

    [SerializeField] private Item _harvest;

    [Header("Materials")]
    [SerializeField] private Material _earthWithFertilize;

    [Header("Setting")]
    [SerializeField] private PlantTypeEnum _type;

    [Header("Components")]
    [SerializeField] private List<GameObject> _plantsLevel;
    [SerializeField] private GameObject _icon;
    [SerializeField] private PlayableDirector _player;

    public Item ResultItem => _harvest;
    public bool IsComplate() => _isPlantGrow;
    public bool IsPlantNeedWater() => _isNeedWater;
    public bool IsFertilized { get { return  _isFertilized; } }

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
            RuntimeManager.PlayOneShot(FMODEvents.instance.Harvest, transform.position);
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

    public void Fertilizering(int level, Material material)
    {
        if (_isFertilized) return;
        if (_isPlantGrow) return;

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

        transform.GetComponentInParent<Seedbed>().ChangeMaterial(material);
        _isFertilized = true;
    }

    public void PlantDead()
    {
        Destroy(_player);

        GameObject activeLevel = null;

        foreach (GameObject tree in _plantsLevel)
        {
            if (tree.activeSelf)
            {
                activeLevel = tree;
                break;
            }
        }

        LODGroup lodGroup = activeLevel.GetComponent<LODGroup>();
        Transform lodTransform = lodGroup.transform;

        foreach (Transform child in lodTransform)
        {
            var renderer = child.GetComponent<Renderer>();
            foreach (Material material in renderer.materials)
            {
                material.color = Color.black;
                material.SetColor(SHADER_COLOR_VAR, Color.black);
            }
        }

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

    private IEnumerator WaitWater()
    {
        yield return new WaitForSeconds(MechConstants.WAIT_TIME_FOR_WATER);
        PlantDead();
    }
}
