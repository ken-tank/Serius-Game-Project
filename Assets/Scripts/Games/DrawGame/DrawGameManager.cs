using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Games.DrawGame
{
    public class DrawGameManager : MonoBehaviour
    {
        public LineRenderer lineRef;
        public RectTransform root;

        public UnityEvent onComplete;

        public Point origin {get;set;}
        public List<Point> point_list {get;set;} = new();

        public static DrawGameManager instance;

        public void Check() 
        {
            if (point_list.Count == 0) return;

            if (point_list.Where(x => x.forceComplete).All(x => x.IsConnectedWithTarget())
             && point_list.Where(x => x.shouldConnect).All(x => x.IsConnectedWithTarget())
            )
            {
                onComplete.Invoke();
                return;
            }
            if (point_list.All(x => x.IsConnectedWithTarget()))
            {
                onComplete.Invoke();
            }
        }

        void Awake()
        {
            instance = this;
            lineRef.gameObject.SetActive(false);
        }
    }
}
