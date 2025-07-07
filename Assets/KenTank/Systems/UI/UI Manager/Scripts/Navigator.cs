using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KenTank.Systems.UI
{
    [AddComponentMenu("KenTank/UI/Navigator")]
    public class Navigator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [System.Serializable]
        public class Neighborhood
        {
            public Navigator top, right, bottom, left;
        }

        [Header("Attribute")]
        [SerializeField] CanvasGroup[] a_selection;
        
        public Neighborhood neighborhood;
        public bool isFirst;

        Button button;
        Dropdown dropdown;

        public bool isSelected {get;set;}

        public void Hover(bool value)
        {
            if (a_selection.Length == 0) return;
            if (!IsEnable()) return;

            var id = gameObject.GetInstanceID() + "-hover";
            DOTween.Kill(id);
            var sequance = DOTween.Sequence();
            sequance.SetId(id);
            sequance.SetUpdate(true);

            foreach (var item in a_selection)
            {
                sequance.Insert(0, item.DOFade(value ? 1f : 0.0f, 0.2f));
            }

            sequance.Play();
        }

        public void Selected(bool value)
        {
            isSelected = value;
            Hover(value);
        }

        public bool IsEnable() 
        {
            var value = true;
            if (button && !button.p_enable) value = false;

            return value;
        }

        public void Submit(bool value = true) 
        {
            if (button) 
            {
                if (value) 
                {
                    button.Submit();
                }
                button.Click(value);
            }

            if (dropdown)
            {
                Debug.Log("Navigation for Dorpdown Still In Development");
            }
        }

        IEnumerator RegisterAsync() 
        {
            yield return new WaitUntil(() => Manager.isInitialized);
            if (Manager.instance.registered.Contains(this)) yield break;
            if (isFirst) 
            {
                Manager.instance.registered.Insert(0, this);
                Manager.instance.selected = this;
            }
            else Manager.instance.registered.Add(this);
        }

        public void OnPointerEnter(PointerEventData data)
        {
            Manager.instance.selected = this;
        }   

        public void OnPointerExit(PointerEventData data)
        {
            Manager.instance.selected = null;
        }

        void Awake()
        {
            TryGetComponent(out button);
            TryGetComponent(out dropdown);

            foreach (var item in a_selection)
            {
                if (!item) continue;
                item.alpha = 0;
            }
        }

        void OnEnable()
        {
            StartCoroutine(RegisterAsync());
        }

        void OnDisable()
        {
            foreach (var item in a_selection)
            {
                if (!item) continue;
                item.alpha = 0;
            }
            Manager.instance.registered.Remove(this);
        }

        void OnDrawGizmosSelected()
        {
            if (neighborhood.top)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, neighborhood.top.transform.position);
            }

            if (neighborhood.bottom)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, neighborhood.bottom.transform.position);
            }

            if (neighborhood.left)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, neighborhood.left.transform.position);
            }

            if (neighborhood.right)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, neighborhood.right.transform.position);
            }
        }

        void OnValidate()
        {
            if (neighborhood.top)
            {
                neighborhood.top.neighborhood.bottom = this;
            }

            if (neighborhood.bottom)
            {
                neighborhood.bottom.neighborhood.top = this;
            }

            if (neighborhood.left)
            {
                neighborhood.left.neighborhood.right = this;
            }

            if (neighborhood.right)
            {
                neighborhood.right.neighborhood.left = this;
            }
        }
    }
}