using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public const string MASTER_VOLUME_KEY = "Master Volume";
    public const string MUSIC_VOLUME_KEY = "Music Volume";
    public const string SFX_VOLUME_KEY = "SFX Volume";
    public const string SENSITIVITY_KEY = "Sensitivity";
    public const string FOV_KEY = "Fov";

    [SerializeField] private bool _isInMenu = false;
    [Header("Audio")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [Header("Gameplay")]
    [SerializeField] private Slider _sensitivitySlider;
    [SerializeField] private Slider _fovSlider;
    [SerializeField] private GameObject _settingsUI;

    private void Awake()
    {
        // Default master volume
        if (PlayerPrefs.HasKey(MASTER_VOLUME_KEY))
        {
            float volume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY);
            Debug.Log($"Volume: {_masterVolumeSlider.value}");
            _masterVolumeSlider.value = volume; // <- тут помилка
        }
        else
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, 1);
            _masterVolumeSlider.value = 1;
        }

        // Default music volume
        if (PlayerPrefs.HasKey(MUSIC_VOLUME_KEY))
        {
            _musicVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
        }
        else
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, 1);
            _musicVolumeSlider.value = 1;
        }

        // Default SFX volume
        if (PlayerPrefs.HasKey(SFX_VOLUME_KEY))
        {
            _sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
        }
        else
        {
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, 1);
            _sfxVolumeSlider.value = 1;
        }

        // Default sensitivity
        if (PlayerPrefs.HasKey(SENSITIVITY_KEY))
        {
            _sensitivitySlider.value = PlayerPrefs.GetFloat(SENSITIVITY_KEY);
        }
        else 
        {
            PlayerPrefs.SetFloat(SENSITIVITY_KEY, 0.25f);
            _sensitivitySlider.value = 0.25f;
        }

        // Default FOV
        if(PlayerPrefs.HasKey(FOV_KEY))
        {
            _fovSlider.value = PlayerPrefs.GetFloat(FOV_KEY);
        }
        else
        {
            PlayerPrefs.SetFloat(SENSITIVITY_KEY, 75);
            _fovSlider.value = 75;
        }

        // Audio
        _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        // Gameplay
        _sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        _fovSlider.onValueChanged.AddListener(OnFovChanged);
    }

    private void Start()
    {
        Debug.Log("fds");
        Apply();
        Debug.Log("fdsfdsf");
    }

    private void OnDestroy()
    {
        // Audio
        _masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        _musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        _sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        // Gameplay
        _sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
        _fovSlider.onValueChanged.RemoveListener(OnFovChanged);
    }

    public void OnMasterVolumeChanged(float value)
    {
        value = Mathf.Clamp(value, _masterVolumeSlider.minValue, _masterVolumeSlider.maxValue);
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, value);
        UpdateMasterVolume(value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        value = Mathf.Clamp(value, _musicVolumeSlider.minValue, _musicVolumeSlider.maxValue);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        UpdateMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        value = Mathf.Clamp(value, _sfxVolumeSlider.minValue, _sfxVolumeSlider.maxValue);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        UpdateSFXVolume(value);
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
        UpdateMasterVolume();
        UpdateMusicVolume();
        UpdateSFXVolume();
        float sentivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY);
        PlayerConstants.SENSITIVITY_HOR = sentivity;
        PlayerConstants.SENSITIVITY_VERT = sentivity;
        foreach (var camera in Camera.main.GetComponentsInChildren<Camera>())
        {
            camera.fieldOfView = PlayerPrefs.GetFloat(FOV_KEY);
        }
    }

    private void UpdateMasterVolume(float? value = null)
    {
        if (_isInMenu == false)
            AudioController.instance.SetMasterVolume(value ?? PlayerPrefs.GetFloat(MASTER_VOLUME_KEY));
    }

    private void UpdateMusicVolume(float? value = null)
    {
        if (_isInMenu == false)
            AudioController.instance.SetMusicVolume(value ?? PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY));
    }

    private void UpdateSFXVolume(float? value = null)
    {
        if (_isInMenu == false)
            AudioController.instance.SetSFXVolume(value ?? PlayerPrefs.GetFloat(SFX_VOLUME_KEY));
    }
}