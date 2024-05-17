using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

[ExecuteAlways]
public class WorldController : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField, Range(GlobalConstants.MIN_TIME_VALUE, GlobalConstants.MAX_TIME_VALUE)] 
    private float _timeOfDay;
    [SerializeField] private Light _directionalLight;
    [SerializeField] private LightingPreset _preset;

    private static WorldController instance;

    public static WorldController GetInstance() => instance;

    public const float MINUTES_MULTIPLIER = 60f;

    public float TimeOfDay
    {
        get => _timeOfDay;
    }

    public int HourOfDay
    {
        get => (int)Math.Truncate(_timeOfDay);
    }

    public int MinuteOfDay
    {
        get => (int)Math.Floor((_timeOfDay - HourOfDay) * MINUTES_MULTIPLIER);
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = _preset._ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = _preset._forColor.Evaluate(timePercent);

        if(_directionalLight != null)
        {
            _directionalLight.color = _preset._directionalColor.Evaluate(timePercent);
            _directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 100f,
                                                                                     170,
                                                                                     0));
        }
    }

    private void UpdateShops()
    {
        ShopController[] shops = FindObjectsOfType<ShopController>();
        foreach (ShopController shop in shops)
        {
            if (!shop.IsEverydayUpdating) continue;
            shop.InitializeGoodsForDay();
            shop.LoadGoodsForDayToUI();
        }
    }

    private void UpdateDayQuest()
    {
        QuestSystemController.GetInstance().InitializeDayQuests();
        QuestSystemController.GetInstance().LoadDayQuestListToUI();
        UIController.GetInstance().CloseQuestInformation();
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        ApplyTimeChanging();
    }

    private void ApplyTimeChanging()
    {
        if (_preset == null) return;

        if (Application.isPlaying)
        {
            _timeOfDay += (Time.deltaTime * GlobalConstants.TIME_MULTIPLIER);

            if(_timeOfDay >= GlobalConstants.MAX_TIME_VALUE)
            {
                UpdateShops();
                UpdateDayQuest();
            }

            _timeOfDay %= GlobalConstants.MAX_TIME_VALUE;
            UpdateLighting(_timeOfDay / GlobalConstants.MAX_TIME_VALUE);
        }
        else
        {
            UpdateLighting(_timeOfDay / GlobalConstants.MAX_TIME_VALUE);
        }
    }

    private void OnValidate()
    {
        if (_directionalLight != null)
            return;

        if(RenderSettings.sun != null)
        {
            _directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights) 
            { 
                if(light.type == LightType.Directional)
                {
                    _directionalLight = light;
                    return;
                }
            }
        }
    }
}
