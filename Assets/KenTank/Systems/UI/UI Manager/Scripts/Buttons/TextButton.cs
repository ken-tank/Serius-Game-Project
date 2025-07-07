using TMPro;
using UnityEngine;

namespace KenTank.Systems.UI.Buttons
{
    [AddComponentMenu("KenTank/UI/Input/TextButton")]
    [RequireComponent(typeof(Button))]
    public class TextButton : MonoBehaviour
    {
        [Header("Attribute")]
        [SerializeField] TextMeshProUGUI a_text;

        [Header("Properties")]
        [SerializeField] string _label = "Button";
        [CustomInspector.AsRange(0, 100)]
        [SerializeField] Vector2 _size = new Vector2(10, 20);

        public string label {
            get => _label;
            set {
                _label = value.Trim();
                if (a_text)
                {   
                    a_text.SetText(_label);
                }
            }
        }

        public Vector2 size {
            get => _size;
            set {
                _size = value;
                if (a_text)
                {
                    a_text.fontSizeMin = _size.x;
                    a_text.fontSizeMax = _size.y;
                }
            }
        }

        void OnValidate()
        {
            label = _label;
            size = _size;
        }
    }
}
