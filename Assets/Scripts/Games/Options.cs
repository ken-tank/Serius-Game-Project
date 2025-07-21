using System.Collections;
using FMODUnity;
using KenTank.Core.SettingsManager;
using KenTank.Systems.UI;
using UnityEngine;

using SettingManager = KenTank.Core.SettingsManager.Manager;

public class Options : MonoBehaviour
{
    [SerializeField] Toggle music;
    [SerializeField] Toggle sfx;

    SettingManager manager => SettingManager.instance;

    IEnumerator DirtyInit() 
    {
        while (!SettingManager.isInitialized)
        {
            yield return null;
        }

        music.value = manager.config.Get("audio-music").getBool;
        sfx.value = manager.config.Get("audio-sfx").getBool;

        music.onValueChanged.AddListener(value => {
            manager.config.Set("audio-music", value);
            RuntimeManager.StudioSystem.setParameterByName("Music", value ? 1 : 0);
            manager.DirtySave();
        });
        sfx.onValueChanged.AddListener(value => {
            manager.config.Set("audio-sfx", value);
            RuntimeManager.StudioSystem.setParameterByName("UI", value ? 1 : 0);
            manager.DirtySave();
        });
    }   

    void Awake() 
    {
        StartCoroutine(DirtyInit());
    }
}
