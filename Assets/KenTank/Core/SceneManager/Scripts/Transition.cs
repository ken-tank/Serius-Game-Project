using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KenTank.Core.SceneManager
{
    public abstract class Transition : MonoBehaviour
    {
        [Header("Transition Attribute")]
        public Slider loadingSlider;

        [Header("Transition Properties")]
        public float duration;

        public bool isAnimating {get;set;}
        public Sequence currentSequance {get;set;}
        
        public abstract void Show(bool value);
    }
}