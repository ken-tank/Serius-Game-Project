using KenTank.Systems.UI.Procedural;
using UnityEngine;

namespace KenTank.Systems.UI.Buttons
{
    [AddComponentMenu("KenTank/UI/Input/ProceduralButton")]
    [RequireComponent(typeof(Button))]
    public class ProceduralButton : MonoBehaviour
    {
        [Header("Attribute")]
        [SerializeField] ProceduralRoundedRectangle a_bg;
        [SerializeField] ProceduralRoundedRectangle a_outline;
        [SerializeField] RectTransform a_backShadow;
        [SerializeField] RectTransform a_shadeShadow;

        [Header("Elements")]
        [SerializeField] bool _backShadow = true;
        [SerializeField] bool _shadeShadow = true;
        [SerializeField] bool _outline = false;
        
        [Header("Properties")]
        [SerializeField] float _roundedCorner = 8;

        [Header("Color")]
        [SerializeField] Color _bgColor = new Color(0.2117647f, 0.2980392f, 0.6588235f, 1);
        [SerializeField] Color _outlineColor = Color.black;

        Button button;

        public bool backShadow {
            get => _backShadow;
            set {
                _backShadow = value;
                if (a_backShadow)
                {
                    var buttonActive = button ? button.showBackground : true;
                    a_backShadow.gameObject.SetActive(_backShadow && buttonActive);
                }
            }
        }

        public bool shadeShadow {
            get => _shadeShadow;
            set {
                _shadeShadow = value;
                if (a_shadeShadow)
                {
                    var buttonActive = button ? button.showBackground : true;
                    a_shadeShadow.gameObject.SetActive(_shadeShadow && buttonActive);
                }
            }
        }

        public bool outline {
            get => _outline;
            set {
                _outline = value;
                if (a_outline)
                {
                    a_outline.gameObject.SetActive(_outline);
                }
            }
        }

        public float roundedCorner {
            get => _roundedCorner;
            set {
                _roundedCorner = value;
                var final = _roundedCorner;
                if (a_bg)
                {
                    a_bg.universalCornerRadius = final;
                    a_bg.SetVerticesDirty();
                }
                if (a_outline)
                {
                    a_outline.universalCornerRadius = final;
                    a_outline.SetVerticesDirty();
                }
            }
        }

        public Color bgColor {
            get => _bgColor;
            set {
                _bgColor = value;
                if (a_bg)
                {
                    a_bg.color = _bgColor;
                }
            }
        }

        public Color outlineColor {
            get => _outlineColor;
            set {
                _outlineColor = value;
                if (a_outline)
                {
                    a_outline.outlineColor = _outlineColor;
                }
            }
        }

        void OnValidate()
        {
            void UpdatePropertiesDirty() 
            {
                backShadow = _backShadow;
                shadeShadow = _shadeShadow;
                outline = _outline;

                #if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall -= UpdatePropertiesDirty;
                #endif
            }

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += UpdatePropertiesDirty;
            #else
            UpdatePropertiesDirty();
            #endif

            roundedCorner = _roundedCorner;
            bgColor = _bgColor;
            outlineColor = _outlineColor;
        }

        void Awake()
        {
            button = GetComponent<Button>();
        }
    }
}
