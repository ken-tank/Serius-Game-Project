using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace KenTank.Systems.UI 
{
    public class ConfirmationBox : MonoBehaviour
    {
        [Header("Attribute")]
        [SerializeField] DialogBox a_root;
        [SerializeField] TextMeshProUGUI a_header;
        [SerializeField] TextMeshProUGUI a_content;

        [Header("Properties")]
        [SerializeField] string p_header;
        [SerializeField, TextArea] string p_content;
        [SerializeField] bool waitAnimation = true;

        [Header("Events")]
        public UnityEvent onTrue;
        public UnityEvent onFalse;

        bool isConfirmed = false;

        public string g_header {
            get => p_header;
            set {
                p_header = value;
                if (a_header)
                {
                    a_header.SetText(p_header);
                }
            }
        }

        public string g_content {
            get => p_content;
            set {
                p_content = value;
                if (a_content)
                {
                    a_content.SetText(p_content);
                }
            }
        }

        public void SetConfirmation(bool value)
        {
            if (isConfirmed) return;

            a_root.Show(false);
            var id = "delayCall-" + gameObject.GetInstanceID();
            var delay = waitAnimation ? a_root.p_duration : 0;
            DOVirtual.DelayedCall(delay, () => {
                if (value) 
                {
                    onTrue.Invoke();
                }
                else
                {
                    onFalse.Invoke();
                }
            })
            .SetId(id);
            isConfirmed = true;
        }

        void OnValidate()
        {
            g_header = p_header;
            g_content = p_content;
        }

        void OnEnable()
        {
            isConfirmed = false;
        }
    }
}