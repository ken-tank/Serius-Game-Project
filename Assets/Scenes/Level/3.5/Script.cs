using System;
using FMOD.Studio;
using FMODUnity;
using Game.Core;
using KenTank.Systems.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Minggu3_5
{
    public class Script : MonoBehaviour
    {
        [SerializeField] ZoomInAnimation zoomInAnimation;
        [SerializeField] EventReference[] sound;
        [SerializeField] Button[] buttons;

        int catchIndex;
        EventReference catchSound;

        public UnityEvent onTrue;

        EventInstance instance;

        public void Answer(int index)
        {
            if (index == catchIndex)
            {
                onTrue.Invoke();
            }
        }

        public void PlaySound()
        {
            if (instance.isValid())
            {
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            instance = RuntimeManager.CreateInstance(catchSound);
            instance.start();
        }

        void Awake()
        {
            var lcg = new RandomLCG(gameObject.GetInstanceID() + DateTime.Now.Ticks);
            catchIndex = lcg.Next(0, sound.Length);
            catchSound = sound[catchIndex];
            zoomInAnimation.target = buttons[catchIndex].GetComponent<RectTransform>();
        }

        void OnDisable()
        {
            if (instance.isValid()) instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
