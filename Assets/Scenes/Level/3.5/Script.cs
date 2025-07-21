using System;
using FMOD.Studio;
using FMODUnity;
using Game.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Minggu3_5
{
    public class Script : MonoBehaviour
    {
        [SerializeField] EventReference[] sound;

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
        }

        void OnDisable()
        {
            if (instance.isValid()) instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
