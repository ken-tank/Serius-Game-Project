using UnityEngine;

namespace Game.UI
{
    public class PageToggle : MonoBehaviour
    {
        [SerializeField] GameObject[] pages;
        [SerializeField] int _active = 0;

        static bool initialized;
        static int saved;

        public int active {
            get => _active;
            set {
                if (value == _active) return;
                
                _active = Mathf.Clamp(value, 0, pages.Length-1);
                saved = _active;
                if (pages.Length > 0)
                {
                    for (int i = 0; i < pages.Length; i++)
                    {
                        pages[i].gameObject.SetActive(i == _active);
                    }
                }
            }
        }

        void Awake()
        {
            if (initialized)
            {
                active = saved;
            }
            else
            {
                initialized = true;
            }
        }

        void OnValidate()
        {
            #if UNITY_EDITOR
            void DirtyApply() 
            {
                active = _active;
                UnityEditor.EditorApplication.delayCall -= DirtyApply;
            }
            UnityEditor.EditorApplication.delayCall += DirtyApply;
            #endif
        }
    }
}