using UnityEngine;
using UnityEngine.UI;

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
        _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        _sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        _fovSlider.onValueChanged.AddListener(OnFovChanged);

        if (PlayerPrefs.HasKey(VOLUME_KEY))
        {
            _volumeSlider.value = PlayerPrefs.GetFloat(VOLUME_KEY);
        }
        else
        {
            PlayerPrefs.SetFloat(VOLUME_KEY, 1);
            _volumeSlider.value = 1;
        }

        if(PlayerPrefs.HasKey(SENSITIVITY_KEY))
        {
            _sensitivitySlider.value = PlayerPrefs.GetFloat(SENSITIVITY_KEY);
        }
        else 
        {
            PlayerPrefs.SetFloat(SENSITIVITY_KEY, 0.25f);
            _sensitivitySlider.value = 0.25f;
        }

        if(PlayerPrefs.HasKey(FOV_KEY))
        {
            _fovSlider.value = PlayerPrefs.GetFloat(FOV_KEY);
        }
        else
        {
            PlayerPrefs.SetFloat(SENSITIVITY_KEY, 75);
            _sensitivitySlider.value = 75;
        }
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
        foreach (var camera in Camera.main.GetComponentsInChildren<Camera>())
        {
            camera.fieldOfView = value;
        }
    }

    public void Apply()
    {
        PlayerPrefs.Save();
        if (_isInMenu == false)
            AudioController.instance.SetMasterVolume(PlayerPrefs.GetFloat(VOLUME_KEY));
        float sentivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY);
        PlayerConstants.SENSITIVITY_HOR = sentivity;
        PlayerConstants.SENSITIVITY_VERT = sentivity;
        foreach (var camera in Camera.main.GetComponentsInChildren<Camera>())
        {
            camera.fieldOfView = PlayerPrefs.GetFloat(FOV_KEY);
        }
    }
}