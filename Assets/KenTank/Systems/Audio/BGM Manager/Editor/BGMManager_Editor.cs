using UnityEngine;
using UnityEditor;

namespace KenTank.Systems.Audio.Editor
{
    public class BGMManager_Editor : UnityEditor.Editor
    {
        [MenuItem("GameObject/Audio/BGM Controller", false)]
        public static void CreateBGMController() 
        {
            var instance = new GameObject("BGM Controller");
            Undo.RegisterCreatedObjectUndo(instance, "Create BGM Controller");
            instance.AddComponent<BGMController>();
            Selection.activeGameObject = instance;
        }
    }
}