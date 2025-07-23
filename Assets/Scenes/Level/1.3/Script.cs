using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games.DragNDrop;
using KenTank.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Minggu1_3
{
    public class Script : MonoBehaviour
    {
        [SerializeField] Slot slotRef;
        [SerializeField] Node nodesRef;
        [SerializeField] int nodeAmmount = 3;
        [SerializeField] GameObject[] images;

        public async void Init()
        {
            var index = Random.Range(0, images.Length);
            var target = images[index];
            var target_rect = target.GetComponent<RectTransform>();
            var slot = Instantiate(slotRef, target.transform.parent);
            var slot_rect = slot.GetComponent<RectTransform>();
            var nodes = images.ToArray().ShuffleItems().OrderByDescending(x => x == target).Take(nodeAmmount).ToArray();
            Node target_node = null;
            var node_instances = new List<Node>();
            //slot.alphaThreshold.sprite = target.GetComponent<Image>().sprite;
            foreach (var item in nodes.ShuffleItems())
            {
                var aa = Instantiate(nodesRef, nodesRef.transform.parent);
                var bb = Instantiate(item, aa.transform);
                aa.gameObject.SetActive(true);
                bb.transform.localPosition = Vector3.zero;
                if (item == target)
                {
                    target_node = aa;
                }
                node_instances.Add(aa);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodesRef.transform.parent.GetComponent<RectTransform>());
            await Task.Yield();
            foreach (var item in node_instances)
            {
                item.ReCenter();
            }
            nodesRef.transform.parent.TryGetComponent(out HorizontalLayoutGroup horizontalLayout);
            nodesRef.transform.parent.TryGetComponent(out VerticalLayoutGroup verticalLayout);
            if (horizontalLayout) horizontalLayout.enabled = false;
            if (verticalLayout) verticalLayout.enabled = false;
            slot_rect.pivot = target_rect.pivot;
            slot_rect.anchorMin = target_rect.anchorMin;
            slot_rect.anchorMax = target_rect.anchorMax;
            slot_rect.sizeDelta = target_rect.sizeDelta;
            slot_rect.position = target_rect.position;
            slot.gameObject.SetActive(true);
            target.SetActive(false);
            slot.target = new Node[1] {target_node};
            slotRef.gameObject.SetActive(false);
        }
    } 
}