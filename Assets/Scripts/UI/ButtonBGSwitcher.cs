using KenTank.Systems.UI;
using UnityEngine;

namespace Game.UI
{
    public class ButtonBGSwitcher : MonoBehaviour
    {
        [SerializeField] Button[] buttons;
        [SerializeField] int _active;

        static bool initialized;
        static int saved;

        public int active {
            get => _active;
            set {
                _active = Mathf.Clamp(value, 0, buttons.Length-1);
                saved = _active;
                if (buttons.Length <= 0) return;
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].showBackground = _active == i;
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