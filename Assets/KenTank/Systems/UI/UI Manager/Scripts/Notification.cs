using DG.Tweening;
using FMODUnity;
using UnityEngine;

namespace KenTank.Systems.UI
{
    public abstract class Notification : MonoBehaviour
    {
        public EventReference showAudio;
        
        public abstract Sequence Show(string msg);
        public abstract Sequence Hide();
    }
}