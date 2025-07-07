using UnityEngine;
using UnityEngine.UI;

namespace KenTank.Systems.UI.Buttons 
{
    [AddComponentMenu("KenTank/UI/Input/IconButton")]
    [RequireComponent(typeof(Button))]
    public class IconButton : MonoBehaviour
    {
        [Header("Attribute")]
        [SerializeField] Image a_icon;
        [SerializeField] Shadow a_iconShadow;
        [SerializeField] Outline a_iconOutline;

        [Header("Properties")]
        [SerializeField] Sprite _sprite;
        [SerializeField] float _scale = 0.75f;
        [SerializeField] bool _iconShadow = false;
        [SerializeField] Vector2 _shadowOffset;
        [SerializeField] bool _iconOutline = false;
        [SerializeField] float _outlineWeight;

        public Sprite sprite {
            get => _sprite;
            set {
                _sprite = value;
                if (a_icon)
                {   
                    a_icon.gameObject.SetActive(_sprite);
                    a_icon.sprite = _sprite;
                }
            }
        }

        public float scale {
            get => _scale;
            set {
                _scale = value;
                if (a_icon)
                {
                    a_icon.transform.localScale = Vector3.one * _scale;
                }
            }
        }

        public bool iconShadow {
            get => _iconShadow;
            set {
                _iconShadow = value;
                if (a_iconShadow)
                {
                    a_iconShadow.enabled = _iconShadow;
                }
            }
        }

        public Vector2 shadowOffset {
            get => _shadowOffset;
            set {
                _shadowOffset = value;
                if (a_iconShadow)
                {
                    a_iconShadow.effectDistance = _shadowOffset;
                }
            }
        }

        public bool iconOutline {
            get => _iconOutline;
            set {
                _iconOutline = value;
                if (a_iconOutline)
                {
                    a_iconOutline.enabled = _iconOutline;
                }
            }
        }
        
        public float outlineWeight {
            get => _outlineWeight;
            set {
                _outlineWeight = value;
                if (a_iconOutline)
                {
                    a_iconOutline.effectDistance = new(_outlineWeight, _outlineWeight);
                }
            }
        }

        void OnValidate()
        {
            sprite = _sprite;
            scale = _scale;
            iconShadow = _iconShadow;
            shadowOffset = _shadowOffset;
            iconOutline = _iconOutline;
            outlineWeight = _outlineWeight;
        }
    }
}
