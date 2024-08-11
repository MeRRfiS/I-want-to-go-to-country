using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public sealed class AudioController : MonoBehaviour
{
    private FMOD.Studio.System studioSystem;
    private FMOD.System coreSystem;
    private FMOD.ChannelGroup masterChannelGroup;
    private Bus musicBus;
    private Bus sfxBus;

    public static AudioController instance;

    public static AudioController GetInstance() => instance;

    private EventInstance _music;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // ������������ ������� FMOD Studio
        studioSystem = RuntimeManager.StudioSystem;
        studioSystem.getCoreSystem(out coreSystem);

        // ��������� ������� ����� ������ (master channel group)
        coreSystem.getMasterChannelGroup(out masterChannelGroup);

        // ��������� ���� ������ �� �������� ������
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");

        _music = CreateInstance(FMODEvents.instance.BackroundMusic);
        _music.start();
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);

        return eventInstance;
    }

    public void SetMasterVolume(float volume)
    {
        // ������������ ������� ������� ����� ������
        masterChannelGroup.setVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        // ������������ ������� ����� ������ ��� ������
        musicBus.setVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        // ������������ ������� ����� ������ ��� �������� ������
        sfxBus.setVolume(volume);
    }

    private void OnDestroy()
    {
        _music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _music.release();
    }
}
