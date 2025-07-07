using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace KenTank.Core.SettingsManager.Editor 
{
    public class SettingsManager_Editor : UnityEditor.Editor
    {
        public const string settingPath = "Assets/KenTank/Core/SettingsManager/Resources/GameConfig.asset";

        [InitializeOnLoadMethod]
        public static void Init()
        {
            EditorApplication.delayCall += AssetChecker;
        }

        static void AssetChecker() 
        {
            EditorApplication.delayCall -= AssetChecker;

            if (!Resources.Load<GameConfig>("GameConfig"))
            {
                var instance = CreateInstance<GameConfig>();
                var path = settingPath;
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                AssetDatabase.CreateAsset(instance, path);
                AssetDatabase.SaveAssets();

                Debug.Log("Create GameConfig Assets in Resourcess");
            }
        }

        [MenuItem(itemName:"Tools/KenTank/SettingsManager/GameConfig")]
        public static void OpenSettings() 
        {
            Init();
            var item = AssetDatabase.LoadAssetAtPath<Object>(settingPath);
            Selection.activeObject = item;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem(itemName:"Tools/KenTank/SettingsManager/Open Config Directory")]
        public static void OpenSaveDirectory() 
        {
            var path = Path.Join(Application.persistentDataPath, Application.productName);
            path = Path.GetDirectoryName(path).Replace(@"/", @"\");

            // Windows
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                Process.Start("explorer.exe", path);
            }
            // macOS
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                Process.Start("open", path);
            }
            // Linux
            else if (Application.platform == RuntimePlatform.LinuxEditor)
            {
                Process.Start("xdg-open", path);
            }
        }
    }
}