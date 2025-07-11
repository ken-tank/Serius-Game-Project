using System.Collections;
using Game.Core;
using UnityEngine;

namespace Game 
{
    public class GameManager : MonoBehaviour
    {
        public static void Quit() 
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
            #endif
        }
    }
}