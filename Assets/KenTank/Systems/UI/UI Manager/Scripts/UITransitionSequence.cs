using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace KenTank.Systems.UI
{
    [AddComponentMenu("KenTank/UI/Animation/Transition Sequance")]
    public class UITransitionSequence : MonoBehaviour
    {
        [SerializeField] bool autoAnimate = true;
        [SerializeField] float delay;
        [SerializeField] float interval;
        [SerializeField] bool reverseOnHide;
        [SerializeField] UITransition[] sequances;

        public bool isShow {get;set;}

        void Awake()
        {
            foreach (var item in sequances)
            {
                if (!item) continue;

                item.autoAnimate = false;
            }
        }

        void OnEnable()
        {
            if (autoAnimate)
            {
                Play(true);
            }
        }

        void OnDisable()
        {
            isShow = false;
        }

        public void Play(bool showState)
        {
            if (isShow == showState) return;
            
            isShow = showState;
            if (showState)
            {
                if (!gameObject.activeSelf) gameObject.SetActive(true);
            }

            var id = "[UI SEQUANCES] - " + gameObject.GetInstanceID();

            DOTween.Kill(id);

            var sequance = DOTween.Sequence();

            sequance
            .SetUpdate(true)
            .SetId(id)
            .OnComplete(() => {
                if (!showState)
                {
                    gameObject.SetActive(false);
                }
            });

            var time = 0f;
            var sequances = this.sequances;
            if (!showState && reverseOnHide) sequances = this.sequances.Reverse().ToArray();
            foreach (var item in sequances)
            {
                if (!item) continue;
                
                if (showState) item.Prepare(showState);
                sequance.Insert(delay + time, item.Play(showState));
                time += interval;
            }

            sequance.Play();
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