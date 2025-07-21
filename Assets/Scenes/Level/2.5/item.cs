using Game.Utilities;
using Games.DragNDrop;
using UnityEngine;
using UnityEngine.UI;

namespace Minggu2_5
{
    public class item : MonoBehaviour
    {
        void Awake()
        {
            var node = GetComponent<Node>();
            var raycast = GetComponent<GraphicRaycaster>();
            var item = GetComponent<TrueItem>();

            node.onValid.AddListener(() => {
                raycast.enabled = false;
                item.value = true;
            });
        }
    }
}
