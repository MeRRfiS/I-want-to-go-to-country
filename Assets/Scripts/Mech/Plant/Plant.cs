using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class Plant : MonoBehaviour
{
    private bool _isPlantGrow = false;
    private bool _isPlantDry = false;
    private bool _isNeedWater = false;
    private bool _isFruitsGrow = false;
    private bool _isFertilized = false;
    private const string WAIT_WATER_COR = "WaitWater";
    private const string SHADER_COLOR_VAR = "_Color";
    [SerializeField] private List<Fruit> fruits;
    private Animator _animator;
    private Renderer _renderer;

    public bool IsFertilized
    {
        get => _isFertilized;
    }

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

    public bool IsFruitsGrow
    {
        get => _isFruitsGrow;
    }

    public SeedTypeEnum SeedType
    {
        get; set;
    }

    public TreeTypeEnum TreeType
    {
        get; set;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<Renderer>();
    }

    private void FixedUpdate()
    {
        if (!_isPlantGrow) return;
        if (fruits.Count == 0) return;

        _isFruitsGrow = CheckFruitGrowingStatus();
        CheckFruitWaterStatus();
    }

    private bool CheckFruitGrowingStatus()
    {
        foreach (Fruit fruit in fruits)
        {
            if (fruit.IsFruitGrow) continue;

            return false;
        }

        return true;
    }

    private void CheckFruitWaterStatus()
    {
        foreach (Fruit fruit in fruits)
        {
            if (!fruit.IsNeedWater) continue;

            fruit.AnimatorEnabled(false);
            _isNeedWater = true;
            StartCoroutine(WAIT_WATER_COR);

            return;
        }
    }

    public void PlantDead()
    {
        Destroy(_animator);
        foreach (Fruit fruit in fruits)
        {
            Destroy(fruit.gameObject);
        }
        fruits.Clear();

        foreach (Material material in _renderer.materials)
        {
            material.color = Color.black;
            material.SetColor(SHADER_COLOR_VAR, Color.black);
        }

        _isPlantDry = true;
    }

    public void WateringPlant()
    {
        if (!_isNeedWater) return;
        if (_isPlantDry) return;

        StopCoroutine(WAIT_WATER_COR);
        _isNeedWater = false;

        if (!_isPlantGrow)
        {
            _animator.enabled = true;
        }
        else
        {
            foreach (Fruit fruit in fruits)
            {
                fruit.AnimatorEnabled(true);
                fruit.IsNeedWater = false;
            }
        }
    }

    public int CollectFruit()
    {
        foreach (Fruit fruit in fruits)
        {
            fruit.ResetAnimation();
        }
        _isFruitsGrow = false;

        return 1;
    }

    public void FertilizeringPlant()
    {
        if (_isFertilized) return;
        if (_isPlantGrow) return;

        _isFertilized = true;
    }

    //Method for animation's event
    public void PlantGrew()
    {
        _isPlantGrow = true;
        
        if(fruits.Count != 0)
        {
            foreach (Fruit fruit in fruits)
            {
                fruit.AnimatorEnabled(true);
            }
        }
    }

    //Method for animation's event
    public void NeedWater()
    {
        _isNeedWater = true;
        _animator.enabled = false;
        StartCoroutine(WAIT_WATER_COR);
    }

    private IEnumerator WaitWater()
    {
        yield return new WaitForSeconds(MechConstants.WAIT_TIME_FOR_WATER);
        PlantDead();

        _isNeedWater = false;
    }
}
