using System;
using System.Linq;
using DG.Tweening;
using Games.DragNDrop;
using KenTank.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Utilities
{
    public class FlipPositionElement : MonoBehaviour
    {
        [SerializeField] bool autoStart = true;
        [SerializeField] bool useGlobalTransform = false;
        [SerializeField] float duration = 0.3f;
        [SerializeField] Transform[] original;

        public UnityEvent onComplete;
        
        public void Shuffle() 
        {
            if (!gameObject.activeInHierarchy) return;

            var original_pos = new Vector3[original.Length];
            for (int i = 0; i < original.Length; i++)
            {
                original_pos[i] = useGlobalTransform ? original[i].position : original[i].localPosition;
            }

            var clone = original.ToArray();
            var shuffledArray = clone.ShuffleItems();
            var sequance = DOTween.Sequence();
            sequance.OnComplete(() => {
                foreach (var item in original)
                {
                    item.TryGetComponent(out Node node);
                    if (!node) node = item.GetComponentInChildren<Node>();
                    if (node) node.ReCenter();
                }
                onComplete.Invoke();
            });
            for (int i = 0; i < original.Length; i++)
            {
                var pos = original_pos[i];
                var item = shuffledArray[i];

                sequance.Insert(0, (useGlobalTransform ? item.DOMove(pos, duration) : item.DOLocalMove(pos, duration))
                    .SetEase(Ease.OutCubic)
                );
            }
            sequance.Play();
        }

        void OnEnable()
        {
            if (autoStart) Shuffle();
        }
    }
}