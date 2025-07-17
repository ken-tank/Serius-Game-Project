using System;
using System.Threading.Tasks;
using DG.Tweening;
using Game.Core;
using KenTank.Systems.UI.Procedural;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Games.DragNDrop
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class Node : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Attribute")]
        [SerializeField] ProceduralRoundedRectangle shape;

        [Header("Properties")]
        [SerializeField] bool lockPosition = true;
        [SerializeField] Gradient fillColor;
        
        DragNDropManager manager => DragNDropManager.instance;
        PointerEventData pointer;
        Vector2 offset;
        Vector2 initialPos;
        Canvas canvas;
        GraphicRaycaster graphicRaycaster;

        public bool isGraping {get;private set;}
        public bool isValid {get;set;}
        public Slot selectedSlot {get; set;}

        string idSnapPos => gameObject.GetInstanceID() + "-snapPos";
        string idPinch => gameObject.GetInstanceID() + "-pinch";

        public void OnPointerDown(PointerEventData data)
        {
            isGraping = true;
            pointer = data;
            offset = (Vector2)transform.position - pointer.position;
            manager.selectedNode = this;
            canvas.overrideSorting = true;
            graphicRaycaster.enabled = false;

            DOTween.Kill(idSnapPos);
            DOTween.Kill(idPinch);
            transform.DOScale(new Vector3(0.9f, 1.2f), 0.5f)
            .SetEase(Ease.OutElastic)
            .SetId(idPinch);
        }

        public void OnPointerUp(PointerEventData data)
        {
            isGraping = false;
            pointer = null;
            offset = Vector2.zero;
            manager.selectedNode = null;
            canvas.overrideSorting = false;
            graphicRaycaster.enabled = true;

            DOTween.Kill(idSnapPos);
            DOTween.Kill(idPinch);

            transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutElastic)
            .SetId(idPinch);

            if (isValid)
            {
                transform.DOMove(selectedSlot.transform.position, 0.2f)
                .SetEase(Ease.OutQuad)
                .SetId(idSnapPos);
            }
            else
            {
                canvas.overrideSorting = true;
                graphicRaycaster.enabled = false;
                if (lockPosition) 
                {
                    
                    transform.DOMove(initialPos, 0.5f)
                    .SetEase(Ease.OutQuad)
                    .SetId(idSnapPos)
                    .OnComplete(() => {
                        canvas.overrideSorting = false;
                        graphicRaycaster.enabled = true;
                    });
                };
                
                if (selectedSlot) selectedSlot = null;
            }

            manager.CheckValidation();
        }

        public void ReCenter() 
        {
            initialPos = transform.position;
        }

        async void Awake()
        {
            initialPos = transform.position;
            canvas = GetComponent<Canvas>();
            graphicRaycaster = GetComponent<GraphicRaycaster>();

            shape.color = fillColor.Evaluate(UnityEngine.Random.Range(0, 1.1f));
            shape.SetVerticesDirty();

            canvas.sortingOrder = 5;
            canvas.overrideSorting = false;
            while (!manager) await Task.Yield();
            manager.nodes.Add(this);
        }

        void OnDisable()
        {
            pointer = null;
            isGraping = false;
            offset = Vector2.zero;
            transform.localScale = Vector2.one;
        }

        void LateUpdate()
        {
            if (isGraping)
            {
                transform.position = pointer.position + offset;
            }
        }
    }
}