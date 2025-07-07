using DG.Tweening;
using FMODUnity;
using UnityEngine;

namespace KenTank.Systems.UI 
{
    public class NotificationCenter : MonoBehaviour
    {
        public static NotificationCenter instance;

        [SerializeField] float showDuration = 3;
        [SerializeField] Notification upNotification;

        [RuntimeInitializeOnLoadMethod]
        public static void Init() 
        {
            if (instance)
            {
                return;
            }

            var reference = Resources.Load("UI/NotificationCenter");
            var item = Instantiate(reference);
            item.name = $"[{reference.name}]";
        }

        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Show(string msg)
        {
            var id = "notification-up";

            DOTween.Kill(id);
            var sequance = DOTween.Sequence();

            sequance.SetId(id);
            sequance.SetUpdate(true);
            
            sequance.AppendCallback(() => upNotification.Show(msg));
            sequance.AppendInterval(showDuration);
            sequance.AppendCallback(() => upNotification.Hide());
            
            if (!upNotification.showAudio.IsNull) RuntimeManager.PlayOneShot(upNotification.showAudio);
        }
    }
}