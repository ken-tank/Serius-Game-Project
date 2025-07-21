using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Utilities
{
    public class Timer : MonoBehaviour
    {
        public enum Mode { forward = 1, backward = -1 }

        [ContextMenuItem("ConvertFromHours", "ConvertHoursToSeconds")]
        [ContextMenuItem("ConvertFromMinutes", "ConvertMinutesToSeconds")]
        public float startFrom;
        public Mode mode = Mode.forward;

        [Header("Events")]
        public UnityEvent<float> backwardPercent;
        public UnityEvent<TimeSpan> now;
        public UnityEvent onStart;
        public UnityEvent onEnd;

        public double seconds {get;private set;}
        public bool isRunning {get;set;}

        public void ResetTimer() 
        {
            isRunning = true;
            seconds = startFrom;
            onStart.Invoke();
        }

        public void Pause(bool value)
        {
            isRunning = !value;
        }

        void ConvertHoursToSeconds()
        {
            var value = startFrom * 3600;
            startFrom = value;
        }

        void ConvertMinutesToSeconds()
        {
            var value = startFrom * 60;
            startFrom = value;
        }

        void OnEnable()
        {
            seconds = startFrom;
            onStart.Invoke();
            isRunning = true;
        }

        void Update()
        {
            if (!isRunning) return;

            var deltatime = Time.deltaTime;
            seconds += deltatime * (int)mode;

            var timeSpan = TimeSpan.FromSeconds(seconds);
            now.Invoke(timeSpan);

            if (mode == Mode.backward)
            {
                backwardPercent.Invoke(Mathf.InverseLerp(0, startFrom, (float)seconds));
                if (seconds <= 0)
                {
                    onEnd.Invoke();
                    isRunning = false;
                }
            }
        }
    }
}