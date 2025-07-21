using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Minggu4_4
{
    public class Script : MonoBehaviour
    {
        [Serializable]
        class Group {
            public Sprite[] sprites;
        }

        [SerializeField] Image[] nodes;
        [SerializeField] Group[] groups;

        void Awake()
        {
            var lcg = new RandomLCG(DateTime.Now.Ticks);
            var selected = groups[lcg.Next(0, groups.Length)];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].sprite = selected.sprites[i];
            }
        }
    }
}
