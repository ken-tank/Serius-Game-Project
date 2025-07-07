using TMPro;
using UnityEngine;

namespace KenTank.Systems.UI 
{
    [AddComponentMenu("KenTank/UI/Input/Slider")]
    public class Slider : MonoBehaviour
    {
        [Header("Attribute")]
        [SerializeField] UnityEngine.UI.Slider a_slider;
        [SerializeField] TMP_InputField a_input;

        [Header("Properties")]
        [SerializeField] bool _showTextField = true;
        [SerializeField, Range(0, 1)] float _value;
        [SerializeField] string textFormat = "0.0#";

        public bool showTextField {
            get => _showTextField;
            set {
                _showTextField = value;
                if (a_input)
                {
                    a_input.transform.parent.gameObject.SetActive(_showTextField);
                }
            }
        }

        public float value {
            get => _value;
            set {
                _value = value;
                if (!a_slider && !a_input) return;
                a_slider.value = Mathf.Lerp(a_slider.minValue, a_slider.maxValue, _value);
                a_input.text = a_slider.value.ToString(textFormat);
            }
        }

        public float actual_value => a_slider.value;

        void OnValidate()
        {
            showTextField = _showTextField;
        }

        void Awake()
        {
            a_slider.onValueChanged.AddListener(value => a_input.text = value.ToString(textFormat));
            a_input.onEndEdit.AddListener(value => a_slider.value = float.Parse(value));
            a_input.text = a_slider.value.ToString(textFormat);
        }
    }
}