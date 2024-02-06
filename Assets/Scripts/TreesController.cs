using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TreesController : MonoBehaviour
{
    private const string SHADER_COLOR_VAR = "BaseColor";
    private const string WAIT_WATER_COR = "WaitWater";

    private bool _isNeedWater = false;
    private bool _isTreeDry = false;
    private bool _isTreeGrow = false;
    private bool _isFruitGrow = false; 
    private int _collectTimes = 0;
    private int _chopTreeTime = 0;

    [SerializeField] private Item _harvest;
    [SerializeField] private Item _logs;
    [SerializeField] private List<Fruit> _fruit;

    [Header("Components")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private PlayableDirector _player; 
    [SerializeField] private GameObject _icon;

    public bool IsCanChoppingTree() => _isTreeGrow || _isTreeDry; 
    public bool IsPlantNeedWater() => _isNeedWater;

    private void FixedUpdate()
    {
        ApplyWaterIcon();

        if (!_isTreeGrow) return;
        if (_fruit.Count == 0) return;

        _isFruitGrow = CheckFruitGrowingStatus();
        CheckFruitWaterStatus();
    }

    public void TreeHarvesting()
    {
        if (_isFruitGrow)
        {
            int fruitCount = Random.Range(MechConstants.MIN_TREE_HARVEST,
                                          MechConstants.MAX_TREE_HARVEST);
            _collectTimes += CollectFruit();
            if (_collectTimes == MechConstants.MAX_COUNT_OF_HARVEST)
            {
                TreeDead();
            }
            InventoryController.GetInstance().AddItemToMainInventory(_harvest.Copy(), fruitCount);
        }
    }

    public void WateringTree()
    {
        if (!_isNeedWater) return;
        if (_isTreeDry) return;

        StopCoroutine(WAIT_WATER_COR);
        _isNeedWater = false;

        if (!_isTreeGrow)
        {
            _player.Play();
        }
        else
        {
            foreach (Fruit fruit in _fruit)
            {
                fruit.AnimatorEnabled(true);
                fruit.IsNeedWater = false;
            }
        }
    }

    //Method for animation's event
    public void TreeGrew()
    {
        _isTreeGrow = true;

        if (_fruit.Count != 0)
        {
            foreach (Fruit fruit in _fruit)
            {
                fruit.AnimatorEnabled(true);
            }
        }

        _player.Pause();
    }

    //Method for animation's event
    public void NeedWater()
    {
        _isNeedWater = true;
        _player.Pause();
        StartCoroutine(WAIT_WATER_COR);
    }

    public void ChoppingTree(int hitCount)
    {
        _chopTreeTime++;
        if (_chopTreeTime == hitCount)
        {
            int logCount = Random.Range(MechConstants.MIN_TREE_LOG,
                                        MechConstants.MAX_TREE_LOG);
            InventoryController.GetInstance().AddItemToMainInventory(_logs.Copy(), logCount);
            Destroy(gameObject);
        }
    }

    private void TreeDead()
    {
        Destroy(_player);
        foreach (Fruit fruit in _fruit)
        {
            Destroy(fruit.gameObject);
        }
        _fruit.Clear();

        foreach (Material material in _renderer.materials)
        {
            material.color = Color.black;
            material.SetColor(SHADER_COLOR_VAR, Color.black);
        }

        _isNeedWater = false;
        _isTreeDry = true;
    }

    private void ApplyWaterIcon()
    {
        _icon.SetActive(_isNeedWater);
    }

    private int CollectFruit()
    {
        foreach (Fruit fruit in _fruit)
        {
            fruit.ResetAnimation();
        }
        _isFruitGrow = false;

        return 1;
    }

    private bool CheckFruitGrowingStatus()
    {
        foreach (Fruit fruit in _fruit)
        {
            if (fruit.IsFruitGrow) continue;

            return false;
        }

        return true;
    }

    private void CheckFruitWaterStatus()
    {
        foreach (Fruit fruit in _fruit)
        {
            if (!fruit.IsNeedWater) continue;

            fruit.AnimatorEnabled(false);
            _isNeedWater = true;
            StartCoroutine(WAIT_WATER_COR);

            return;
        }
    }

    private IEnumerator WaitWater()
    {
        yield return new WaitForSeconds(MechConstants.WAIT_TIME_FOR_WATER);
        TreeDead();
    }
}
