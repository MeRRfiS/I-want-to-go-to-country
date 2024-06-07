using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public const string VOLUME_KEY = "Volume";
    public const string SENSITIVITY_KEY = "Sensitivity";

    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _sensitivitySlider;

    private void Awake()
    {
        _volumeSlider.value = PlayerPrefs.GetFloat(VOLUME_KEY);
        _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        _sensitivitySlider.value = PlayerPrefs.GetFloat(SENSITIVITY_KEY);
        _sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void Start()
    {
        Apply();
    }

    private void OnDestroy()
    {
        _volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        _sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
    }

    public void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
    }

    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, value);
    }

    public void Apply()
    {
        AudioController.instance.SetMasterVolume(PlayerPrefs.GetFloat(VOLUME_KEY));
        float sentivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY);
        PlayerConstants.SENSITIVITY_HOR = sentivity;
        PlayerConstants.SENSITIVITY_VERT = sentivity;
    }
}