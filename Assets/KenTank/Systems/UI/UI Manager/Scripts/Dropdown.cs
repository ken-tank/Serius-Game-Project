using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KenTank.Systems.UI
{
    [AddComponentMenu("KenTank/UI/Input/Dropdown")]
    public class Dropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Attribute")]
        [SerializeField] CanvasGroup[] a_outline;
        [SerializeField] TMP_Dropdown a_dropdown;

        [Header("Properties")]
        [SerializeField] bool p_enable = true;

        public bool g_enable {
            get => p_enable;
            set {
                p_enable = value;
                if (a_dropdown) a_dropdown.interactable = p_enable;
            }
        }

        public TMP_Dropdown g_dropdown => a_dropdown;

        public void SetList(string[] values)
        {
            var options = values.Select(x => new TMP_Dropdown.OptionData(x)).ToList();
            a_dropdown.ClearOptions();
            a_dropdown.AddOptions(options);
        }

        public void Hover(bool value)
        {
            if (a_outline.Length == 0) return;
            if (a_dropdown.IsExpanded) value = false;

            var id = gameObject.GetInstanceID() + "_hover";
            DOTween.Kill(id);
            var sequance = DOTween.Sequence();
            sequance.SetId(id);
            sequance.SetUpdate(true);

            foreach (var item in a_outline)
            {
                sequance.Insert(0, item.DOFade(value ? 1f : 0.0f, 0.2f));
            }

            sequance.Play();
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (!p_enable) return;
            Hover(true);
        }   

        public void OnPointerExit(PointerEventData data)
        {
            if (!p_enable) return;
            Hover(false);
        }

        void OnValidate()
        {
            g_enable = p_enable;
        }
    }
}