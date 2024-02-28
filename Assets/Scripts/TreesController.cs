using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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

    [Header("Components")]
    [SerializeField] private List<GameObject> _treeLevels;
    [SerializeField] private PlayableDirector _treePlayer;
    [SerializeField] private PlayableDirector _applePlayer;
    [SerializeField] private GameObject _icon;

    public bool IsCanChoppingTree() => _isTreeGrow || _isTreeDry; 
    public bool IsPlantNeedWater() => _isNeedWater;

    private void FixedUpdate()
    {
        ApplyWaterIcon();
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
            _treePlayer.Play();
        }
        else
        {
            _applePlayer.Play();
        }
    }

    //Method for animation's event
    public void TreeGrew()
    {
        _isTreeGrow = true;
        _applePlayer.Play();
        _treePlayer.Pause();
    }

    //Method for animation's event
    public void NeedWater()
    {
        _isNeedWater = true;
        _treePlayer.Pause();
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

    public void ApplesNeedWater()
    {
        _isNeedWater = true;
        _applePlayer.Pause();
        StartCoroutine(WAIT_WATER_COR);
    }

    public void ApplesGrew()
    {
        _isFruitGrow = true;
        _applePlayer.Stop();
    }

    private Renderer GetActiveLOD()
    {
        Renderer result = null;
        GameObject activeLevel = null;

        foreach (GameObject tree in _treeLevels)
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
            result = child.GetComponent<Renderer>();
            if (result != null && result.isVisible) break;
        }

        return result;
    }

    private void TreeDead()
    {
        Destroy(_treePlayer);
        Destroy(_applePlayer.gameObject);

        GameObject activeLevel = null;

        foreach (GameObject tree in _treeLevels)
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
        _isTreeDry = true;
    }

    private void ApplyWaterIcon()
    {
        _icon.SetActive(_isNeedWater);
    }

    private int CollectFruit()
    {
        _applePlayer.Play();
        _isFruitGrow = false;

        return 1;
    }

    private IEnumerator WaitWater()
    {
        yield return new WaitForSeconds(MechConstants.WAIT_TIME_FOR_WATER);

        TreeDead();
    }
}
