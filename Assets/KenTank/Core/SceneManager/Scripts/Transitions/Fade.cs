using DG.Tweening;
using UnityEngine;

namespace KenTank.Core.SceneManager.Transitions 
{
    public class Fade : Transition
    {
        [Header("Attribute")]
        [SerializeField] CanvasGroup root;

        public override void Show(bool value)
        {
            var id = "scene-fade";
            if (value)
            {
                root.alpha = 0;
            }

            DOTween.Kill(id);
            var tween = DOTween.Sequence();
            tween.Insert(0, root.DOFade(value ? 1 : 0, duration));
            
            tween
            .SetId(id)
            .SetUpdate(true)
            .OnComplete(() => {
                isAnimating = false;
                currentSequance = null;
            });

            currentSequance = tween;
            isAnimating = true;

            tween.Play();
        }
    }
}