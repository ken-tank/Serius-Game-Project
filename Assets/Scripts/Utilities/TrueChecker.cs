using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Utilities
{
    public class TrueChecker : MonoBehaviour
    {
        [SerializeField] TrueItem[] items;

        public UnityEvent<bool> onStatus;
        public UnityEvent onTrue;
        public UnityEvent onFalse;

        public void Check() 
        {
            var status = items.All(x => x.value);
            if (status)
            {
                onTrue.Invoke();
            }
            else
            {
                onFalse.Invoke();
            }
            onStatus.Invoke(status);
        }

        void Awake()
        {
            foreach (var item in items)
            {
                item.onChangedValue += value => Check();
            }
        }
    }
}