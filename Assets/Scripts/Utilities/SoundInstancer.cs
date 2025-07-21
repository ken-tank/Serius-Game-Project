using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundInstancer : MonoBehaviour
{
    [SerializeField] EventReference[] sounds;

    EventInstance instance;

    public void PlayAt(int index)
    {
        var sound = sounds[index];
        if (instance.isValid())
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        instance = RuntimeManager.CreateInstance(sound);
        instance.start();
    }

    void OnDisable()
    {
        if (instance.isValid())
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
