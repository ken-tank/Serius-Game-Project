using System;
using UnityEngine;

namespace Game.Utilities
{
    public class TrueItem : MonoBehaviour
    {
        public Action<bool> onChangedValue;

        bool _value;

        public bool value {
            get => _value;
            set {
                _value = value;
                onChangedValue?.Invoke(_value);
            }
        }
        
        public bool Toggle() 
        {
            value = !value;
            return value;
        }
    }
}