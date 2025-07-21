using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace KenTank.Core.SettingsManager
{
    public class Manager : MonoBehaviour
    {
        public static Manager instance;

        public GameConfig config;

        GameConfig defaultConfig;
        string path => Path.Join(Application.persistentDataPath, "config.ini");

        public static Action onInitialized;
        public static bool isInitialized;

        Coroutine dirtySaveTask = null;

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            var item = new GameObject("[SettingsManager]");
            item.AddComponent<Manager>();
        }

        async void Awake()
        {
            instance = this;
            defaultConfig = Resources.Load<GameConfig>("GameConfig");
            DontDestroyOnLoad(gameObject);
            await LoadAsync();
            onInitialized?.Invoke();
            SettingsActions.ApplyEffect();
            isInitialized = true;
        }

        string[] Deserialize(GameConfig config)
        {
            var text = new List<string>();
            foreach (var item in config.list)
            {
                text.Add($"{item.key.Trim()}={item.value.Trim()}");
            }
            return text.ToArray();
        }

        GameConfig Serialize(string[] lines, out bool foundError)
        {
            foundError = false;
            var data = Instantiate(defaultConfig);
            data.name = "GameConfig - Instance";
            Dictionary<string,string> local = new();
            foreach (var item in lines)
            {
                if (string.IsNullOrEmpty(item) || item.StartsWith(";") || item.StartsWith("#")) continue;
                if (item.Contains('='))
                {
                    var splited = item.Split('=');
                    local.Add(splited[0], splited[1]);
                }
            }
            foreach (var item in data.list)
            {
                if (local.ContainsKey(item.key))
                {
                    var value = local[item.key];
                    if (!item.IsValueValid(value))
                    {
                        value = item.value;
                        foundError = true;
                    }
                    item.value = value;
                }
                else
                {
                    foundError = true;
                }
            }
            return data;
        }

        public void ResetToDefault() 
        {
            for (int i = 0; i < config.list.Count; i++)
            {
                config.list[i] = defaultConfig.list[i];
            }
        }

        public async Task LoadAsync() 
        {
            if (!File.Exists(path))
            {
                await SaveAsync(true);
            }

            string[] lines = await File.ReadAllLinesAsync(path);
            config = Serialize(lines, out bool error);
            if (error)
            {
                await SaveAsync();
            }
        }

        public async Task SaveAsync(bool useDefault = false) 
        {
            var data = Deserialize(useDefault ? defaultConfig : config);
            await File.WriteAllLinesAsync(path, data);
        }

        public async void Load() 
        {
            await LoadAsync();
        }

        public async void Save(bool useDefault = false) 
        {
            await SaveAsync(useDefault);
        }

        public void DirtySave(float wait = 1) 
        {
            IEnumerator job()
            {
                yield return new WaitForSecondsRealtime(wait);
                Save();
                dirtySaveTask = null;
            }

            if (dirtySaveTask != null) StopCoroutine(dirtySaveTask);
            dirtySaveTask = StartCoroutine(job());
        }
    }
}