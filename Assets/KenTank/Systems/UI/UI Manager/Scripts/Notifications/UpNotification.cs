using DG.Tweening;
using TMPro;
using UnityEngine;

namespace KenTank.Systems.UI.Notifications
{
    public class UpNotification : Notification
    {
        [Header("Animation Attribute")]
        [SerializeField] CanvasGroup alpha;
        [SerializeField] RectTransform root;
        [SerializeField] TextMeshProUGUI label;

        [Header("Animation Properties")]
        [SerializeField] float duration;
        [SerializeField] Ease showEase;
        [SerializeField] Ease hideEase;
        [SerializeField] float offset = 200;

        Sequence Display(bool value)
        {
            if (value)
            {
                alpha.alpha = 0;
                root.anchoredPosition = new(0, offset);
                if (!gameObject.activeSelf) gameObject.SetActive(true);
            }

            var sequance = DOTween.Sequence();

            sequance.SetUpdate(true);
            sequance.onComplete += () => {
                if (!value) gameObject.SetActive(false);
            };

            sequance.Insert(0, alpha.DOFade(value ? 1 : 0, duration));
            sequance.Insert(0, root.DOAnchorPosY(value ? 0 : offset, duration).SetEase(value ? showEase : hideEase));
            
            return sequance;
        }

        public override Sequence Show(string msg)
        {
            var sequance = Display(true);
            sequance.OnStart(() => label.SetText(msg));
            return sequance;
        }

        public override Sequence Hide()
        {
            var sequance = Display(false);
            return sequance;
        }

        void Awake()
        {
            alpha.alpha = 0;
            root.anchoredPosition = new(0, offset);
            gameObject.SetActive(false);
        }
    }
}