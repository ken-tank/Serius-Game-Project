using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Utilities
{
    public class TimeRenderer : MonoBehaviour
    {
        [SerializeField] string timeFormat = "hh\\:mm\\:ss";

        public UnityEvent<string> render;

        public void Convert(TimeSpan timeSpan)
        {
            render.Invoke(timeSpan.ToString(timeFormat));
        }
    }
}