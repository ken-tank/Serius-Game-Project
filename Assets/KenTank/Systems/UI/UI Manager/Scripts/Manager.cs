using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KenTank.Systems.UI
{
    [AddComponentMenu("KenTank/UI/UI Manager")]
    public class Manager : MonoBehaviour
    {
        public static Manager instance;
        public static bool isInitialized;

        [Header("Navigation Input")]
        [SerializeField] InputAction input_navigation;
        [SerializeField] InputAction input_submit;

        Navigator _selected;

        public List<Navigator> registered {get;set;} = new();

        public Navigator selected {
            get => _selected;
            set {
                if (_selected) _selected.Selected(false);
                _selected = value;
                if (_selected) 
                {
                    _selected.Selected(true);
                    if (registered.Contains(_selected))
                    {
                        registered.Remove(_selected);
                        registered.Insert(0, _selected);
                    }
                }
            }
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Init() 
        {
            if (instance) return;
            
            var go = Resources.Load<Manager>("UI/UI Manager");
            instance = Instantiate(go);
            instance.gameObject.name = $"[{go.gameObject.name}]";
            DontDestroyOnLoad(instance.gameObject);
            isInitialized = true;
        }

        void Awake()
        {
            input_navigation.performed += callback => {
                var axis = callback.ReadValue<Vector2>();
                
                if (axis.magnitude > 0)
                {
                    if (!selected || !registered.Contains(selected))
                    {
                        selected = registered[0];
                    }
                    else
                    {
                        if (axis.y > 0 && selected.neighborhood.top) 
                        {
                            selected = selected.IsEnable() 
                            ? selected.neighborhood.top 
                            : selected.neighborhood.top.neighborhood.top;
                        }
                        if (axis.y < 0 && selected.neighborhood.bottom)
                        {
                            selected = selected.IsEnable() 
                            ? selected.neighborhood.bottom 
                            : selected.neighborhood.bottom.neighborhood.bottom;
                        }
                        if (axis.x < 0 && selected.neighborhood.left) 
                        {
                            selected = selected.IsEnable() 
                            ? selected.neighborhood.left 
                            : selected.neighborhood.left.neighborhood.left;
                        }
                        if (axis.x > 0 && selected.neighborhood.right) 
                        {
                            selected = selected.IsEnable() 
                            ? selected.neighborhood.right 
                            : selected.neighborhood.right.neighborhood.right;
                        }
                    }
                }
            };

            input_submit.performed += callback => {
                if (selected) selected.Submit(true);
            };

            input_submit.canceled += callback => {
                if (selected) selected.Submit(false);
            };
        }

        void OnEnable()
        {
            input_navigation.Enable();
            input_submit.Enable();
        }

        void OnDisable()
        {
            input_navigation.Disable();
            input_submit.Disable();
        }
    }
}