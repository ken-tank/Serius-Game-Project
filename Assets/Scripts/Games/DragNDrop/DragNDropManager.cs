using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Games.DragNDrop
{
    public class DragNDropManager : MonoBehaviour
    {
        public static DragNDropManager instance;

        public UnityEvent onAllValid;

        public Node selectedNode {get;set;}

        public List<Slot> slots {get;set;} = new();
        public List<Node> nodes {get;set;} = new();

        public void CheckValidation() 
        {
            if (slots.Count > 0 && slots.All(x => x.isValid))
            {
                onAllValid.Invoke();
            }
        }

        void Awake()
        {
            instance = this;
        }

        void OnDestroy()
        {
            instance = null;
        }
    }
}