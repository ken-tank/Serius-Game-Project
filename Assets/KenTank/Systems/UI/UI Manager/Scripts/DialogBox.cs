using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KenTank.Systems.UI
{
    [AddComponentMenu("KenTank/UI/Window/DialogBox")]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(CanvasGroup))]
    public class DialogBox : MonoBehaviour
    {
        [Header("Attribute")]
        [SerializeField] CanvasGroup a_background;
        [SerializeField] RectTransform a_root;

        [Header("Properties")]
        public float p_duration = 0.3f;

        [Header("Transitions")]
        [SerializeField] Transition showTransition; 
        [SerializeField] Transition hideTransition; 

        [System.Serializable]
        class Transition {
            public Vector2 offset;
            public Vector2 size = Vector2.one;
            public Ease ease;
        }

        public void Show(bool value) 
        {
            var id = "dialogbox-" + gameObject.GetInstanceID();

            if (value)
            {
                a_root.anchoredPosition = showTransition.offset;
                a_root.localScale = showTransition.size;
                a_background.alpha = 0;
                if (!gameObject.activeSelf) gameObject.SetActive(true);
            }

            DOTween.Kill(id);
            
            var tweenParameter = new TweenParams()
            .SetEase(value ? showTransition.ease : hideTransition.ease);

            var tween = DOTween.Sequence();

            tween
            .SetId(id)
            .SetUpdate(true)
            .OnComplete(() => {
                if (!value)
                {
                    gameObject.SetActive(false);
                }
            });

            tween.Insert(0, a_background.DOFade(value ? 1 : 0, p_duration));
            tween.Insert(0, a_root.DOScale(value ? Vector2.one : hideTransition.size, p_duration).SetAs(tweenParameter));
            tween.Insert(0, a_root.DOAnchorPos(value ? Vector2.one : hideTransition.offset, p_duration).SetAs(tweenParameter));

            tween.Play();
        }

        void OnEnable()
        {
            Show(true);
        }
    }
}