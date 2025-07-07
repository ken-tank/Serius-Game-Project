using DG.Tweening;
using FMODUnity;
using UnityEngine;

namespace KenTank.Systems.UI
{
    [AddComponentMenu("KenTank/UI/Animation/Transition")]
    public class UITransition : MonoBehaviour
    {
        [Header("Attribute")]
        [SerializeField] CanvasGroup alpha;
        [SerializeField] RectTransform root;

        public bool autoAnimate = false;

        [Header("Time Properties")]
        public float delay;
        [SerializeField] bool enableFade = true;
        [SerializeField] float fadeDuration = 0.3f;
        
        public Animate show;
        public Animate hide;

        public bool isShow {get;set;}

        [System.Serializable]
        public class Animate {
            [Header("Transition Properties")]
            public float duration = 0.3f;
            public Ease ease;
            [Header("Animation Properties")]
            public Vector2 offset;
            public Vector3 rotate;
            public Vector2 scale = Vector2.one;
            [Header("Audio Properties")]
            public EventReference sound;
            public float soundDelay;
        }

        void OnEnable()
        {
            if (autoAnimate)
            {
                Show();
            }
        }

        void OnDisable()
        {
            isShow = false;
        }

        public void Prepare(Animate value) 
        {
            alpha.alpha = enableFade ? 0 : 1;
            root.anchoredPosition = value.offset;
            root.localRotation = Quaternion.Euler(value.rotate);
            root.localScale = value.scale;
        }

        public void Prepare(bool value)
        {
            Prepare(value ? show : hide);
        }

        public Sequence Play(bool showState)
        {
            if (isShow == showState) return null;

            var value = showState;
            isShow = value;
            var transition = value ? show : hide;
            var id = gameObject.GetInstanceID() + "-UI Transition";
            if (value)
            {
                Prepare(transition);
                gameObject.SetActive(true);
            }

            DOTween.Kill(id);
            var sequance = DOTween.Sequence();

            sequance
            .SetId(id)
            .SetUpdate(true)
            .OnComplete(() => {
                if (!value)
                {
                    gameObject.SetActive(false);
                }
            });

            if (enableFade) sequance.Insert(delay, alpha.DOFade(value ? 1 : 0, fadeDuration).SetId(id));
            sequance.Insert(
                delay,
                root.DOLocalRotateQuaternion(Quaternion.Euler(transition.rotate), transition.duration)
                .SetEase(transition.ease)
            );
            sequance.Insert(
                delay, 
                root.DOAnchorPos(value ? Vector2.zero : transition.offset, transition.duration)
                .SetEase(value ? show.ease : hide.ease)
            );
            sequance.Insert(
                delay, 
                root.DOScale(value ? Vector2.one : transition.scale, transition.duration)
                .SetEase(value ? show.ease : hide.ease)
                .OnStart(() => {
                        DOVirtual.DelayedCall(transition.soundDelay, () => {
                            if (transition.sound.IsNull) return;
                            RuntimeManager.PlayOneShot(transition.sound);
                        }
                    );
                })
            );

            sequance.Play();

            return sequance;
        }

        public void Show() 
        {
            Play(true);
        }

        public void Hide()
        {
            Play(false);
        }
    }  
}