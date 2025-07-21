using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Games.DrawGame
{
    public class Point : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        public Point targetConnect;
        public bool isNetral;
        public bool forceComplete;
        public bool shouldConnect = true;

        public UnityEvent onConnected;

        public Point inConnect {get;set;}
        public Point outConnect {get;set;}

        DrawGameManager manager => DrawGameManager.instance;
        LineRenderer line;
        PointerEventData pointer;
        bool isSelect;

        public void OnPointerDown(PointerEventData data)
        {
            if (outConnect) 
            {
                return;
            }
            isSelect = true;
            pointer = data;
            CreateLine();
        }

        public void OnPointerUp(PointerEventData data)
        {
            isSelect = false;
            pointer = null;
            if (manager.origin) manager.origin.DestroyLine();
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (!manager.origin) return;
            if (manager.origin == this) return;
            if (inConnect) return;
            pointer = data;
            manager.origin.Connect(this);
        }

        public bool IsConnectedWithTarget() 
        {
            if (outConnect == targetConnect || inConnect == targetConnect || !targetConnect || isNetral)
            {
                return true;
            }
            return false;
        }

        void CreateLine() 
        {
            line = Instantiate(manager.lineRef, manager.lineRef.transform.parent);
            line.gameObject.SetActive(true);
            line.SetPosition(0, ConvertCanvasToWorld(transform.position));
            manager.origin = this;
            isSelect = true;
        }

        void DestroyLine() 
        {
            if (line)
            {
                Destroy(line.gameObject);
                line = null;
                isSelect = false;
                manager.origin = null;
            }
        }

        void Connect(Point target) 
        {
            line.SetPosition(1, ConvertCanvasToWorld(target.transform.position));
            outConnect = target;
            target.inConnect = this;
            manager.origin = target;
            isSelect = false;
            pointer = null;
            
            target.CreateLine();
            manager.Check();
            onConnected?.Invoke();
        }

        Vector2 ConvertScreenPointToRelative(Vector2 pos) 
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(manager.root, pos, Camera.main, out Vector2 result);
            return result;
        }

        Vector2 ConvertCanvasToWorld(Vector2 world)
        {
            return ConvertScreenPointToRelative(RectTransformUtility.WorldToScreenPoint(Camera.main, world));
        }

        async void Awake()
        {
            while (!manager)
            {
                await Task.Yield();
            }
            manager.point_list.Add(this);
        }

        void OnDisable()
        {
            manager.point_list.Remove(this);
        }

        void LateUpdate()
        {
            if (isSelect)
            {
                line.SetPosition(1, ConvertScreenPointToRelative(pointer.position));
            }
        }
    }
}
