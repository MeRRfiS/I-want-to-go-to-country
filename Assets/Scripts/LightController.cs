using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightController : MonoBehaviour
{
    [SerializeField, Range(GlobalConstants.MIN_TIME_VALUE, GlobalConstants.MAX_TIME_VALUE)] 
    private float _timeOfDay;
    [SerializeField] private Light _directionalLight;
    [SerializeField] private LightingPreset _preset;

    private static LightController instance;

    public static LightController GetInstance() => instance;

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

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(_preset == null) return;

        if (Application.isPlaying)
        {
            _timeOfDay += (Time.deltaTime * GlobalConstants.TIME_MULTIPLIER);
            _timeOfDay %= 24;
            UpdateLighting(_timeOfDay / 24f);
        }
        else
        {
            UpdateLighting(_timeOfDay / 24f);
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
