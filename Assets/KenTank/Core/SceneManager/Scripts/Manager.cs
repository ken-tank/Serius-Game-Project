using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using USM = UnityEngine.SceneManagement.SceneManager;

namespace KenTank.Core.SceneManager
{
    public class Manager : MonoBehaviour
    {
        public static Manager instance;

        [SerializeField] Transform transitionRoot;
        public Transition transition;

        public Transition currentTransition {get;set;} = null;
        public AsyncOperation currentTask {get;set;} = null;

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            if (instance) return;

            var reference = Resources.Load<Manager>("SceneManager");
            var prefab = Instantiate(reference);
            prefab.gameObject.name = $"[{reference.name}]";
        }

        void Awake()
        {
            if (instance) 
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async void LoadScene(int buildIndex, bool withLoadingBar = false)
        {
            if (currentTask != null) return;

            var scene = USM.LoadSceneAsync(buildIndex);
            currentTask = scene;

            if (transition)
            {
                scene.allowSceneActivation = false;
                currentTransition = Instantiate(transition, transitionRoot);
                if (currentTransition.loadingSlider)
                {
                    currentTransition.loadingSlider.gameObject.SetActive(withLoadingBar);
                    currentTransition.loadingSlider.value = 0;
                }
                currentTransition.Show(true);

                while (scene.progress < 0.9f || currentTransition.isAnimating) 
                {
                    if (currentTransition.loadingSlider)
                    {
                        currentTransition.loadingSlider.value = scene.progress;
                    }
                    await Task.Yield();
                }
                
                scene.allowSceneActivation = true;
                while (!scene.isDone) await Task.Yield();

                if (currentTransition.loadingSlider)
                {
                    currentTransition.loadingSlider.value = 1;
                }
                currentTransition.Show(false);
                
                await currentTransition.currentSequance.AsyncWaitForCompletion();

                Destroy(currentTransition.gameObject);
                currentTransition = null;
                currentTask = null;
            }
            else
            {
                while (scene.progress != 0.9f) await Task.Yield();
                
                scene.allowSceneActivation = true;
                while (!scene.isDone) await Task.Yield();
                currentTask = null;
            }
        }
    }
}