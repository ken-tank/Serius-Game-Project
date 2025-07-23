using System;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Game.Core;
using KenTank.Systems.UI.Procedural;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Games.DragNDrop
{
    public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Attribute")]
        [SerializeField] ProceduralRoundedRectangle shape;
        public Image alphaThreshold;

        [Header("Properties")]
        public bool allowInvalid;
        public bool allowMultiFilled = false;
        [SerializeField] Color outlineColor;
        [SerializeField] Gradient outlineColorHighlight;
        [SerializeField] Gradient fillColor;
        public Node[] target;

        DragNDropManager manager => DragNDropManager.instance;

        public bool isValid {get;set;}
        public Node filled {get;set;}
        public Node activeNode {get;set;}

        public Action<Node> onFilled = null;

        public void OnPointerEnter(PointerEventData data)
        {
            if (manager.selectedNode)
            {
                //if (!CheckIsAlphaPoint(pointer)) return;
                if (activeNode)
                {
                    if (activeNode != manager.selectedNode)
                    {
                        return;
                    }
                }
                if (target.Contains(manager.selectedNode))
                {
                    isValid = true;
                    if (!allowMultiFilled) activeNode = manager.selectedNode;
                }
                else
                {
                    isValid = false;
                }

                manager.selectedNode.isValid = allowInvalid || isValid;
                manager.selectedNode.selectedSlot = this;
                filled = manager.selectedNode;
                Highlight(true);
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            //if (!CheckIsAlphaPoint(data)) return;
            if (manager.selectedNode)
            {
                if (activeNode)
                {
                    if (activeNode != manager.selectedNode)
                    {
                        return;
                    }
                }
                manager.selectedNode.isValid = false;
                manager.selectedNode.selectedSlot = null;
                filled = null;
                activeNode = null;
                isValid = false;
                Highlight(false);
            }
        }

        public void Highlight(bool value)
        {
            if (!shape) return;

            var id = gameObject.GetInstanceID() + "-highlight";
            DOTween.Kill(id);
            var sequance = DOTween.Sequence();
            sequance.SetId(id);
            
            sequance.Insert(0, DOVirtual.Color(shape.color, value ? outlineColorHighlight.Evaluate(isValid ? 1 : 0) : outlineColor, 0.3f, color => {
                shape.outlineColor = color;
                shape.SetVerticesDirty();
            }));

            sequance.Play();
        }

        bool CheckIsAlphaPoint(PointerEventData eventData)
        {
            return true;
            if (!alphaThreshold || !alphaThreshold.sprite) return true;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                alphaThreshold.rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

            Rect rect = alphaThreshold.rectTransform.rect;

            Vector2 uv = new Vector2(
                (localPoint.x - rect.x) / rect.width,
                (localPoint.y - rect.y) / rect.height
            );

            if (IsAlphaOpaque(alphaThreshold.sprite, uv))
            {
                Debug.Log("Klik mengenai area solid!");
                return true;
            }
            else
            {
                Debug.Log("Klik di area transparan, abaikan.");
                return false;
            }
        }

        bool IsAlphaOpaque(Sprite sprite, Vector2 uv, float threshold = 0.1f)
        {
            if (sprite == null || sprite.texture == null)
                return false;

            if (!sprite.texture.isReadable)
            {
                Debug.LogWarning($"Texture '{sprite.texture.name}' not readable. Aktifkan 'Read/Write Enabled' di import settings.");
                return false;
            }

            // Clamp UV
            uv.x = Mathf.Clamp01(uv.x);
            uv.y = Mathf.Clamp01(uv.y);

            // Hitung posisi pixel di atlas
            Rect texRect = sprite.textureRect;
            int x = Mathf.FloorToInt(texRect.x + uv.x * texRect.width);
            int y = Mathf.FloorToInt(texRect.y + uv.y * texRect.height);

            Color pixel = sprite.texture.GetPixel(x, y);
            return pixel.a >= threshold;
        }

        async void Awake()
        {
            var lcg = new RandomLCG(gameObject.GetInstanceID() + DateTime.Now.Ticks);
            if (shape)
            {
                shape.color = fillColor.Evaluate(lcg.Next(0.0f, 1.1f));
                shape.SetVerticesDirty();
            }
            while(!manager) await Task.Yield();
            manager.slots.Add(this);
        }

        void OnDisable()
        {
            Highlight(false);
            isValid = false;
        }
    }
}