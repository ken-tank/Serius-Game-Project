using TMPro;
using UnityEngine;

namespace KenTank.Systems.UI 
{
    [AddComponentMenu("KenTank/UI/Component/LabelValue")]
    public class LabelValue : MonoBehaviour
    {
        [Header("Attribute")]
        [SerializeField] TextMeshProUGUI labelText;
        [SerializeField] TextMeshProUGUI valueText;

        [Header("Properties")]
        [SerializeField] string _label = "Label";
        [SerializeField] string _value = "Value";
        [SerializeField] float _textSize = 35;

        public string label {
            get => _label;
            set {
                _label = value;
                if (labelText) labelText.SetText(_label);
            }
        }

        public string value {
            get => _value;
            set {
                _value = value;
                if (valueText) valueText.SetText(_value);
            }
        }

        public float textSize {
            get => _textSize;
            set {
                _textSize = Mathf.Clamp(value, 0, Mathf.Infinity);
                if (labelText) labelText.fontSize = _textSize;
                if (valueText) valueText.fontSize = _textSize;
            }   
        }

        void OnValidate()
        {
            label = _label;
            value = _value;
            textSize = _textSize;
        }
    }
}