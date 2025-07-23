using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Games.ConnectGame
{
    public class Node : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public enum Direction {
            Positive, Negative
        }
        [Header("Attribute")]
        [SerializeField] CanvasGroup glow;

        [Header("Properties")]
        public Direction direction = Direction.Positive;
        public Node targetConnect;
        public bool multiConnect = false;
        public bool allowInvalid = false;
        
        PointerEventData pointer;
        LineRenderer line;
        ConnectGameManager manager => ConnectGameManager.instance;

        public bool isSelect {get;set;}
        public bool isConnected {get;set;}
        public Node connectedWith {get;set;}

        string clickAnimationID => gameObject.GetInstanceID() + "-click";

        public void OnPointerDown(PointerEventData data)
        {
            if (isConnected)
            {
                DestroyLine();
                connectedWith.connectedWith = null;
                connectedWith = null;
            }

            pointer = data;
            isSelect = true;
            CreateLine();

            DOTween.Kill(clickAnimationID);
            transform.DOScale(new Vector3(1.1f, 0.85f, 1), 0.7f)
            .SetId(clickAnimationID)
            .SetEase(Ease.OutElastic);
        }   

        public void OnPointerUp(PointerEventData data)
        {
            pointer = null;
            isSelect = false;
            Hover(false, Color.white);
            manager.selected = null;
            if (manager.secondSelected)
            {
                manager.secondSelected.Hover(false, Color.white);
                manager.secondSelected = null;
            }
            
            if (allowInvalid)
            {
                if (connectedWith)
                {
                    Connect();
                }
                else
                {
                    DestroyLine();
                }
            }
            else
            {
                if (!targetConnect || connectedWith == targetConnect)
                {
                    Connect();
                }
                else
                {
                    DestroyLine();
                    connectedWith.Hover(false, Color.white);
                }
            }
            DOTween.Kill(clickAnimationID);
            transform.DOScale(Vector3.one, 1f)
            .SetId(clickAnimationID)
            .SetEase(Ease.OutElastic);
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (isConnected 
                || !manager.selected
                || manager.selected == this
                || manager.selected.direction == direction
            )
            {
                if (manager.selected) 
                {
                    Hover(true, Color.red);
                    manager.secondSelected = this;
                }
                return;
            }

            manager.selected.connectedWith = this;
            Hover(true, Color.green);
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (!manager.selected) 
            {
                return;
            }

            manager.selected.connectedWith = null;
            manager.secondSelected = null;
            Hover(false, Color.white);
        }

        public bool isConnectWithTarget() 
        {
            if (!targetConnect) return true;
            if (!connectedWith || !targetConnect) return false;
            return connectedWith == targetConnect;
        }

        void CreateLine() 
        {
            line = Instantiate(manager.lineRef, manager.lineRef.transform.parent);
            line.gameObject.SetActive(true);
            line.SetPosition(0, ConvertCanvasToWorld(transform.position));
            manager.selected = this;
        }

        void DestroyLine() 
        {
            if (line)
            {
                Destroy(line.gameObject);
                line = null;
                if (connectedWith)
                {
                    connectedWith.line = null;
                    connectedWith.isConnected = false;
                }
                isConnected = false;
            }
        }

        void Connect() 
        {
            line.SetPosition(1, ConvertCanvasToWorld(connectedWith.transform.position));
            connectedWith.line = line;
            connectedWith.connectedWith = this;
            connectedWith.isConnected = true;
            connectedWith.Hover(false, Color.white);
            isConnected = true;
            manager.Check();
        }

        void Hover(bool value, Color color)
        {
            var id = gameObject.GetInstanceID() + "-hover";
            DOTween.Kill(id);

            var sequance = DOTween.Sequence();
            sequance.SetId(id);

            sequance.Insert(0, glow.DOFade(value ? 1 : 0, 0.2f));
            sequance.Insert(0, glow.GetComponent<Image>().DOColor(color, 0.2f));

            sequance.Play();
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
            manager.node_list.Add(this);
        }

        void LateUpdate()
        {
            if (isSelect)
            {
                line.SetPosition(1, ConvertScreenPointToRelative(pointer.position));
            }
        }

        void OnValidate()
        {
            if (targetConnect && !multiConnect)
            {
                targetConnect.targetConnect = this;
            }
        }
    }
}