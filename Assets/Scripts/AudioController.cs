using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioController : MonoBehaviour
{
    private FMOD.Studio.System studioSystem;
    private FMOD.System coreSystem;
    private FMOD.ChannelGroup masterChannelGroup;

    public static AudioController instance;

    public static AudioController GetInstance() => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // Ініціалізація системи FMOD Studio
        studioSystem = RuntimeManager.StudioSystem;
        studioSystem.getCoreSystem(out coreSystem);

        // Отримання головної групи каналів (master channel group)
        coreSystem.getMasterChannelGroup(out masterChannelGroup);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);

        return eventInstance;
    }

    public void SetMasterVolume(float volume)
    {
        // Встановлення гучності головної групи каналів
        masterChannelGroup.setVolume(volume);
    }
}
