using UnityEngine;

namespace Game.UI
{
    public class PageToggle : MonoBehaviour
    {
        [SerializeField] GameObject[] pages;
        [SerializeField] int _active = 0;

        public int active {
            get => _active;
            set {
                if (value == _active) return;
                
                _active = Mathf.Clamp(value, 0, pages.Length-1);
                if (pages.Length > 0)
                {
                    for (int i = 0; i < pages.Length; i++)
                    {
                        pages[i].gameObject.SetActive(i == _active);
                    }
                }
            }
        }

        void OnValidate()
        {
            #if UNITY_EDITOR
            void DirtyApply() 
            {
                if (pages.Length > 0)
                {
                    active = _active;
                }
                UnityEditor.EditorApplication.delayCall -= DirtyApply;
            }
            UnityEditor.EditorApplication.delayCall += DirtyApply;
            #else
            #endif
        }
    }
}