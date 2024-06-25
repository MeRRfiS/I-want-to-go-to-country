using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instrument Object", menuName = "Inventory System/Items/Hoe")]
public class Hoe : Instrument
{
    [field: SerializeField] public int TimeWork { get; private set; }

    [Header("Hoe's prefabs")]
    [SerializeField] private GameObject _seedbedPrefab;
    [SerializeField] private GameObject _seedbedVisualization;

    private const string OPACITY = "_OPACITY";
    private const string BASE_COLOR = "_Color";

    private GameObject _seedbedObj;
    private SeedbedChecking _seedbedCheck;
    private Renderer _seedbedRend;

    private Terrain _terrain;

    private bool IsPatchObjNull() => _seedbedObj == null;

    public override void Init()
    {
        _durability = MaxDurability;
        Amount = 1;
    }

    private void MakePath()
    {
        _durability--;
        var seedbed = Instantiate(_seedbedPrefab);
        seedbed.transform.position = _seedbedObj.transform.position;
        _terrain = FindObjectOfType<Terrain>();
        RemoveGrassAt(_seedbedObj.transform.position);
        RuntimeManager.PlayOneShot(FMODEvents.instance.SeedbedDig, seedbed.transform.position);
        Destroy(_seedbedObj);

        UIController.GetInstance().StopProgressBar();
        _seedbedCheck = null;
        _seedbedObj = null;
    }

    private void RemoveGrassAt(Vector3 worldPosition)
    {
        if (_terrain == null)
        {
            Debug.LogError("Terrain not assigned.");
            return;
        }

        // Перетворити світові координати у координати терену
        Vector3 terrainPosition = worldPosition - _terrain.transform.position;

        // Отримати розмір терену
        TerrainData terrainData = _terrain.terrainData;
        int terrainWidth = terrainData.detailWidth;
        int terrainHeight = terrainData.detailHeight;

        // Перетворити координати терену в координати масиву трави
        float relativeX = terrainPosition.x / terrainData.size.x;
        float relativeZ = terrainPosition.z / terrainData.size.z;
        int detailX = Mathf.FloorToInt(relativeX * terrainWidth);
        int detailZ = Mathf.FloorToInt(relativeZ * terrainHeight);

        // Радіус в координатах масиву трави
        int detailRadius = Mathf.FloorToInt(1 * (terrainWidth / terrainData.size.x));

        // Пройтися по всіх шарах трави
        int detailLayerCount = terrainData.detailPrototypes.Length;
        for (int layer = 0; layer < detailLayerCount; layer++)
        {
            // Отримати дані про траву для поточного шару
            int[,] details = terrainData.GetDetailLayer(0, 0, terrainWidth, terrainHeight, layer);

            // Очистити область навколо заданої позиції
            for (int y = -detailRadius; y <= detailRadius; y++)
            {
                for (int x = -detailRadius; x <= detailRadius; x++)
                {
                    int posX = detailX + x;
                    int posY = detailZ + y;

                    // Перевірка, чи координати не виходять за межі масиву
                    if (posX >= 0 && posX < terrainWidth && posY >= 0 && posY < terrainHeight)
                    {
                        details[posY, posX] = 0;
                    }
                }
            }

            // Застосувати зміни до шару трави
            terrainData.SetDetailLayer(0, 0, layer, details);
        }

        Debug.Log("Grass removed at specified position.");
    }

    public override void UseItem()
    {
        if (IsPatchObjNull()) return;
        if (_seedbedCheck.IsOnObject) return;

        UIController.GetInstance().ProgressBar(TimeWork, MakePath);
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
                            ItemConstants.MAX_DISTANCE_TO_EARTH) && hit.collider.CompareTag(TagConstants.EARTH))
        {
            InstantiateVisual();
            SetPosition(hit);
            CheckPosition();
        }
        else
        {
            _seedbedObj = this.StopUpdating();
            UIController.GetInstance().StopProgressBar();
        }
    }

    private void CheckPosition()
    {
        if (_seedbedCheck && _seedbedCheck.IsOnObject)
        {
            _seedbedRend.material.SetColor(BASE_COLOR, new Color(1, 0, 0, 0.95f));
        }
        else if (_seedbedCheck)
        {
            _seedbedRend.material.SetColor(BASE_COLOR, new Color(1, 1, 1, 0.95f));
        }

        if (_seedbedCheck.IsOnObject)
        {
            UIController.GetInstance().StopProgressBar();
        }
    }

    private void SetPosition(RaycastHit hit)
    {
        Vector3 point = Vector3.zero;
        point = new Vector3(_seedbedCheck.IsVerFasten ? _seedbedCheck.FastenPos.x : hit.point.x,
                            hit.point.y,
                            _seedbedCheck.IsHorFasten ? _seedbedCheck.FastenPos.z : hit.point.z);
        if (Vector3.Distance(point, hit.point) >= MechConstants.MAX_DISTANCE_FOR_FASTEN_PATCH)
        {
            _seedbedCheck.ResetFastenChecking();
            point = hit.point;
        }
        _seedbedObj.transform.position = new Vector3(point.x,
                                                     hit.point.y,
                                                     point.z);
    }

    private void InstantiateVisual()
    {
        if (IsPatchObjNull())
        {
            _seedbedObj = Instantiate(_seedbedVisualization);
            _seedbedRend = _seedbedObj.transform.GetComponentInChildren<Renderer>();
            _seedbedCheck = _seedbedObj.GetComponent<SeedbedChecking>();
        }
    }

    public override GameObject StopUpdating()
    {
        if (!IsPatchObjNull()) MonoBehaviour.Destroy(_seedbedObj);
        _seedbedObj = null;

        return _seedbedObj;
    }

    public override void Destruct()
    {
        _durability = 0;
    }
}
