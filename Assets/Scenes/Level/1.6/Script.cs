using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Games.DragNDrop;
using KenTank.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Minggu1_6
{
    public class Script : MonoBehaviour
    {
        [System.Serializable]
        class Item {
            public Sprite slot;
            public Sprite node;
            public EventReference sound;

            public Slot asignSlot {get;set;}
        }

        [SerializeField] RectTransform lineRef;
        [SerializeField] Slot[] slots;
        [SerializeField] Node nodeRef;
        [SerializeField] Item[] items;
        [SerializeField] int maxNodeShow = 6;
        [SerializeField] int nodePerLine = 6;

        IEnumerator InitAsync()
        {
            var cc = items.ShuffleItems().Take(slots.Length).ToArray();

            for (int i = 0; i < cc.Length; i++)
            {
                var image = slots[i].transform.GetChild(2).GetComponent<Image>();
                var button = slots[i].transform.GetChild(1).GetComponent<KenTank.Systems.UI.Button>();
                image.sprite = cc[i].slot;
                image.gameObject.SetActive(true);
                var sound = cc[i].sound;
                button.onClick.AddListener(() => RuntimeManager.PlayOneShot(sound));
                items.FirstOrDefault(x => x == cc[i]).asignSlot = slots[i];
            }

            List<Node> nodes = new();
            var pickedNode = items.ToArray();
            foreach (var item in cc)
            {
                pickedNode = pickedNode.OrderByDescending(x => x == item).ToArray();
            }
            var isOverflow = maxNodeShow > nodePerLine;
            List<RectTransform> lines = new();
            if (isOverflow)
            {
                var count = Mathf.RoundToInt((float)maxNodeShow/(float)nodePerLine);
                for (int i = 0; i < (maxNodeShow % nodePerLine > 0 ? count + 1 : count) ; i++)
                {
                    var line = Instantiate(lineRef, lineRef.parent);
                    lines.Add(line);
                    line.gameObject.SetActive(true);
                }
            }
            int countPerLine = 1;
            int indexLine = 0;
            foreach (var item in pickedNode.Take(maxNodeShow).ToArray().ShuffleItems())
            {
                if (countPerLine > nodePerLine) 
                {
                    indexLine += 1;
                    countPerLine = 1;
                }
                var parent = isOverflow ? lines[indexLine] : nodeRef.transform.parent;
                var aa = Instantiate(nodeRef, parent);
                aa.gameObject.SetActive(true);
                aa.transform.GetChild(0).GetComponent<Image>().sprite = item.node;
                nodes.Add(aa);

                if (item.asignSlot)
                {
                    item.asignSlot.target = new Node[1] {aa};
                }
                countPerLine ++;
            }
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodeRef.transform.parent.GetComponent<RectTransform>());
            if (nodeRef.transform.parent.TryGetComponent(out HorizontalLayoutGroup horizontalLayout))
            {
                horizontalLayout.enabled = false;
            }
            if (nodeRef.transform.parent.TryGetComponent(out VerticalLayoutGroup verticalLayout))
            {
                verticalLayout.enabled = false;
            }
            if (lineRef)
            {
                foreach (var item in lines)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(item.GetComponent<RectTransform>());
                    if (item.TryGetComponent(out HorizontalLayoutGroup hhh))
                    {
                        hhh.enabled = false;
                    }
                    if (item.TryGetComponent(out VerticalLayoutGroup vvv))
                    {
                        vvv.enabled = false;
                    }
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(lineRef.transform.parent.GetComponent<RectTransform>());
                if (lineRef.transform.parent.TryGetComponent(out HorizontalLayoutGroup hh))
                {
                    hh.enabled = false;
                }
                if (lineRef.transform.parent.TryGetComponent(out VerticalLayoutGroup vv))
                {
                    vv.enabled = false;
                }
            }
            foreach (var item in nodes)
            {
                item.ReCenter();
            }
        }

        public void Init() 
        {
            StartCoroutine(InitAsync());
        }

        void Awake()
        {
            Init();
        }
    }
}
