using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace KenTank.Systems.UI
{
    [AddComponentMenu("KenTank/UI/Input/Toggle")]
    public class Toggle : MonoBehaviour
    {
        [Header("Attribute")]
        [SerializeField] RectTransform a_root;
        [SerializeField] GameObject a_markTrue;
        [SerializeField] GameObject a_markfalse;

        [Header("Properties")]
        [SerializeField] float transitionDuration = 0.2f;
        [SerializeField] bool _value = true;

        [Header("Events")]
        public UnityEvent<bool> onValueChanged;

        public bool value {
            get => _value;
            set {
                _value = value;
                ChangeSprite(_value);
                onValueChanged.Invoke(_value);
            }
        }

        public void ToggleValue() 
        {
            value = !value;
        }

        void ChangeSprite(bool value)
        {
            var id = gameObject.GetInstanceID() + "-toggle";
            DOTween.Kill(id);

            a_root.localScale = Vector2.one;

            var sequance = DOTween.Sequence()
                .SetId(id)
                .SetUpdate(true);

            sequance.Insert(
                0,
                a_root.DOPunchScale(Vector2.one * 0.5f, transitionDuration, Random.Range(5, 11))
            );
            if (a_markTrue) a_markTrue.SetActive(value);
            if (a_markfalse) a_markfalse.SetActive(!value);

            sequance.Play();
        }

        void OnValidate()
        {
            if (a_markTrue) a_markTrue.SetActive(value);
            if (a_markfalse) a_markfalse.SetActive(!value);
        }
    }
}