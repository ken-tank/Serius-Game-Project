using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using USM = UnityEngine.SceneManagement.SceneManager;

namespace KenTank.Core.SceneManager
{
    public class SceneActions : MonoBehaviour
    {
        static int GetBuildIndex(string sceneName)
        {
            int index = -1;
            for (int i = 0; i < USM.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (sceneNameFromPath == sceneName)
                {
                    index = i;
                }
            }
            return index;
        }

        public static void LoadScene(int buildIndex)
        {
            Manager.instance.LoadScene(buildIndex);
        }
        public static void LoadSceneWithLoading(int buildIndex)
        {
            Manager.instance.LoadScene(buildIndex, true);
        }

        public static void LoadScene(string sceneName)
        {
            var index = GetBuildIndex(sceneName);
            Manager.instance.LoadScene(index);
        }
        public static void LoadSceneWithLoading(string sceneName)
        {
            var index = GetBuildIndex(sceneName);
            Manager.instance.LoadScene(index, true);
        }

        public static void RestartScene()
        {
            var current = USM.GetActiveScene().buildIndex;
            LoadScene(current);
        }
        public static void RestartSceneWithLoading()
        {
            var current = USM.GetActiveScene().buildIndex;
            Manager.instance.LoadScene(current, true);
        }

        public static void NextScene() 
        {
            var index = USM.GetActiveScene().buildIndex + 1;
            LoadScene(index);
        }
        public static void NextSceneWithLoading() 
        {
            var index = USM.GetActiveScene().buildIndex + 1;
            LoadSceneWithLoading(index);
        }

        public static void BackScane() 
        {
            var index = USM.GetActiveScene().buildIndex - 1;
            LoadScene(index);
        }
        public static void BackScaneWithLoading() 
        {
            var index = USM.GetActiveScene().buildIndex - 1;
            LoadSceneWithLoading(index);
        }
    }
}