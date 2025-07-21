using System.Linq;
using Games.DragNDrop;
using UnityEngine;

namespace Minggu2_4
{
    public class Script : MonoBehaviour
    {
        [System.Serializable]
        class Group {
            public string catagory;
            public Node[] nodes;
        }

        [SerializeField] Slot[] slots;
        [SerializeField] Group[] groups;

        void Awake()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                var item = slots[i];
                item.target = groups.Select(x => x.nodes[i]).ToArray();
                item.allowInvalid = false;
                var index = i;
                item.onFilled += node => {
                    var aa = groups.FirstOrDefault(x => x.nodes.Contains(node));
                    for (int a = 0; a < slots.Length; a++)
                    {
                        var c = groups.FirstOrDefault(x => x.catagory == aa.catagory).nodes[a];
                        slots[a].target = new Node[1] {c};
                        slots[a].onFilled = null;
                    }
                };
            }
        }
    }
}
