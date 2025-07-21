using FMODUnity;
using UnityEngine;

namespace KenTank.Core.SettingsManager
{
    public class SettingsActions : MonoBehaviour
    {
        public static void ApplyEffect() 
        {
            var data = Manager.instance.config;
            
            // Put Your Effect Here
            Screen.sleepTimeout = data.Get("display-AllowScreenSleep").getBool ? SleepTimeout.SystemSetting : SleepTimeout.NeverSleep;
            var framerate = Mathf.Abs(Mathf.RoundToInt(data.Get("display-framerates").getFloat));
            Application.targetFrameRate = framerate < 1 ? 999 : Mathf.Clamp(framerate, 1, 1000);
            QualitySettings.vSyncCount = data.Get("display-vsync").getBool ? 1 : 0;
            bool music = data.Get("audio-music").getBool;
            bool sfx = data.Get("audio-sfx").getBool;
            RuntimeManager.StudioSystem.setParameterByName("Music", music ? 1 : 0);
            RuntimeManager.StudioSystem.setParameterByName("SFX", sfx ? 1 : 0);
            RuntimeManager.StudioSystem.setParameterByName("UI", sfx ? 1 : 0);
        }

        public static void LoadSettings() 
        {
            Manager.instance.Load();
        }

        public static void SaveSettings() 
        {
            Manager.instance.Save();
            ApplyEffect();
        }

        public static void RestoreToDefault() 
        {
            Manager.instance.ResetToDefault();
        }
    }
}