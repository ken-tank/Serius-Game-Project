using System;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace KenTank.Systems.UI
{
    [AddComponentMenu("KenTank/UI/Input/Button")]
    [RequireComponent(typeof(Navigator))]
    public class Button : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Attribute")]
        [SerializeField] RectTransform a_root;
        [SerializeField] RectTransform[] a_background = new RectTransform[0];

        [Header("Properties")]
        public bool p_enable = true;
        [SerializeField] bool _showBackground = true;
        [SerializeField] EventReference p_clickSound;

        [Header("Events")]
        public UnityEvent onClick;

        public bool showBackground {
            get => _showBackground;
            set {
                _showBackground = value;
                if (a_background.Length == 0) return;
                foreach (var item in a_background)
                {
                    if (!item) continue;
                    item.gameObject.SetActive(_showBackground);
                }
            }
        }

        public void Click(bool value) 
        {
            if (!a_root) return;

            var duration = 0.1f;
            var id = gameObject.GetInstanceID() + "_click";
            DOTween.Kill(id);
            a_root.DOScale(value ? new Vector3(0.9f, 1.1f, 1) : Vector3.one, duration)
            .SetEase(value ? Ease.OutSine : Ease.OutBack)
            .SetId(id)
            .SetUpdate(true);
        }

        public void Submit() 
        {   
            onClick.Invoke();
            if (!p_clickSound.IsNull) RuntimeManager.PlayOneShot(p_clickSound);
        }

        void OnValidate()
        {
            void UpdatePropertiesDirty() 
            {
                showBackground = _showBackground;

                #if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall -= UpdatePropertiesDirty;
                #endif
            }

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += UpdatePropertiesDirty;
            #else
            UpdatePropertiesDirty();
            #endif
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (!p_enable) return;
            Click(true);
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (!p_enable) return;
            Click(false);
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (!p_enable) return;
            Submit();
        }
    }
}