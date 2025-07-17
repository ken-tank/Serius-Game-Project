using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Games.ConnectGame
{
    public class ConnectGameManager : MonoBehaviour
    {
        public RectTransform root;
        public LineRenderer lineRef;

        public UnityEvent onComplete;

        public Node selected {get;set;}
        public Node secondSelected {get;set;}

        public List<Node> node_list {get;set;} = new();

        public static ConnectGameManager instance;

        public void Check() 
        {
            if (node_list.Count == 0) return;
            if (node_list.All(x => x.isConnectWithTarget()))
            {
                onComplete.Invoke();
            }
        }

        void Awake()
        {
            instance = this;
            lineRef.gameObject.SetActive(false);
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
    }
}