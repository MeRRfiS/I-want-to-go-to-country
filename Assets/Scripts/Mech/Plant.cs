using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Plant : MonoBehaviour
{
    private bool _isPlantGrow = false;
    private bool _isPlantDry = false;
    private bool _isNeedWater = false;
    private const string WAIT_WATER_COR = "WaitWater";
    private const string SHADER_COLOR_VAR = "_Color";
    private PlantTypeEnum _type;
    private Animator _animator;
    private Renderer _renderer;

    public bool IsPlantGrow
    {
        get => _isPlantGrow;
    }

    public bool IsPlantDry
    {
        get => _isPlantDry;
    }

    public bool IsNeedWater
    {
        get => _isNeedWater;
    }

    public PlantTypeEnum PlantType
    {
        set => _type = value;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<Renderer>();
    }

    public void GetWater()
    {
        if (_isPlantDry) return;

        StopCoroutine(WAIT_WATER_COR);
        _isNeedWater = false;
        _animator.enabled = true;
    }

    public void PlantGrew()
    {
        _isPlantGrow = true;
    }

    public void NeedWater()
    {
        _isNeedWater = true;
        _animator.enabled = false;
        StartCoroutine(WAIT_WATER_COR);
    }

    private IEnumerator WaitWater()
    {
        yield return new WaitForSeconds(MechConstants.WAIT_TIME_FOR_WATER);

        Destroy(_animator);
        _renderer.material.SetColor(SHADER_COLOR_VAR, Color.black);
        _isNeedWater = false;
        _isPlantDry = true;
    }
}
