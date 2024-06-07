using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameSettings : MonoBehaviour
{
    public const string VOLUME_KEY = "Volume";
    public const string SENSITIVITY_KEY = "Sensitivity";
    public const string FOV_KEY = "Fov";

    [SerializeField] private bool _isInMenu = false;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _sensitivitySlider;
    [SerializeField] private Slider _fovSlider;

    private void Awake()
    {
        _volumeSlider.value = PlayerPrefs.GetFloat(VOLUME_KEY);
        _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        _sensitivitySlider.value = PlayerPrefs.GetFloat(SENSITIVITY_KEY);
        _sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        _fovSlider.value = PlayerPrefs.GetFloat(FOV_KEY);
        _fovSlider.onValueChanged.AddListener(OnFovChanged);
    }

    private void Start()
    {
        Apply();
    }

    private void OnDestroy()
    {
        _volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        _sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
        _fovSlider.onValueChanged.RemoveListener(OnFovChanged);
    }

    public void OnVolumeChanged(float value)
    {
        value = Mathf.Clamp(value, _volumeSlider.minValue, _volumeSlider.maxValue);
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
    }

    public void OnSensitivityChanged(float value)
    {
        value = Mathf.Clamp(value, _sensitivitySlider.minValue, _sensitivitySlider.maxValue);
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, value);
    }

    public void OnFovChanged(float value)
    {
        value = Mathf.Clamp(value, _fovSlider.minValue, _fovSlider.maxValue);
        PlayerPrefs.SetFloat(FOV_KEY, value);
        Camera.main.fieldOfView = value;
    }

    public void Apply()
    {
        PlayerPrefs.Save();
        if (_isInMenu == false)
            AudioController.instance.SetMasterVolume(PlayerPrefs.GetFloat(VOLUME_KEY));
        float sentivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY);
        PlayerConstants.SENSITIVITY_HOR = sentivity;
        PlayerConstants.SENSITIVITY_VERT = sentivity;
        Camera.main.fieldOfView = PlayerPrefs.GetFloat(FOV_KEY);
    }
}