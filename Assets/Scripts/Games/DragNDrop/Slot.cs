using System;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Game.Core;
using KenTank.Systems.UI.Procedural;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Games.DragNDrop
{
    public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Attribute")]
        [SerializeField] ProceduralRoundedRectangle shape;

        [Header("Properties")]
        [SerializeField] bool allowInvalid;
        [SerializeField] Color outlineColor;
        [SerializeField] Gradient outlineColorHighlight;
        [SerializeField] Gradient fillColor;
        public Node[] target;

        DragNDropManager manager => DragNDropManager.instance;

        public bool isValid {get;set;}
        public Node filled {get;set;}

        public void OnPointerEnter(PointerEventData data)
        {
            if (manager.selectedNode)
            {
                if (target.Contains(manager.selectedNode))
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }

                manager.selectedNode.isValid = allowInvalid ? true : isValid;
                manager.selectedNode.selectedSlot = this;
                filled = manager.selectedNode;
                Highlight(true);
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (manager.selectedNode)
            {
                manager.selectedNode.isValid = false;
                manager.selectedNode.selectedSlot = null;
                filled = null;
                isValid = false;
                Highlight(false);
            }
        }

        public void Highlight(bool value)
        {
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

        async void Awake()
        {
            shape = GetComponent<ProceduralRoundedRectangle>();
            var lcg = new RandomLCG(gameObject.GetInstanceID() + DateTime.Now.Ticks);
            shape.color = fillColor.Evaluate(lcg.Next(0.0f, 1.1f));
            shape.SetVerticesDirty();
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