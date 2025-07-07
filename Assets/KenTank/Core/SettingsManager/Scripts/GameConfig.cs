using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KenTank.Core.SettingsManager
{
    public class GameConfig : ScriptableObject
    {
        public enum ConfigValueType
        {
            String,
            Float,
            Bool
        }

        [System.Serializable]
        public class ConfigEntry
        {
            public string key;
            public ConfigValueType valueType;
            public string value;

            public bool IsValueValid()
            {
                return IsValueValid(value);
            }

            public bool IsValueValid(string value)
            {
                return valueType switch
                {
                    ConfigValueType.String => true,
                    ConfigValueType.Float => float.TryParse(value, out float floatValue),
                    ConfigValueType.Bool => bool.TryParse(value, out bool boolValue),
                    _ => true,
                };
            }

            public string getString => value;
            public float getFloat => float.Parse(value);
            public bool getBool => bool.Parse(value);
        }

        public List<ConfigEntry> list = new();
    }

    public static class GameConfigExtension 
    {
        public static GameConfig.ConfigEntry Get(this List<GameConfig.ConfigEntry> from, string key)
        {
            var value = from.FirstOrDefault(x => x.key.ToLower() == key.ToLower());
            return value;
        }

        public static GameConfig.ConfigEntry Get(this GameConfig from, string key)
        {
            var value = from.list.FirstOrDefault(x => x.key.ToLower() == key.ToLower());
            return value;
        }

        public static void Set<T>(this GameConfig from, string key, T value)
        {
            from.Get(key).value = value.ToString();
        }

        public static void Set<T>(this List<GameConfig.ConfigEntry> from, string key, T value)
        {
            from.Get(key).value = value.ToString();
        }
    }
}