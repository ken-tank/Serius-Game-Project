using System.Linq;
using Game.Utilities;
using KenTank.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Minggu3_2
{
    public class Scripts : MonoBehaviour
    {
        [System.Serializable]
        class Group {
            public Sprite full;
            public Sprite[] pices;
        }

        [SerializeField] Image full;
        [SerializeField] FlipPositionElement flip;
        [SerializeField] Image[] pices;
        [SerializeField] Image[] bamboozed;
        [SerializeField] Group[] groups;

        public void Init()
        {
            var item = groups.PickOne();
            var picked_bamboozed = groups.SelectMany(x => x.pices).Where(x => !item.pices.Contains(x)).ToArray().ShuffleItems().Take(bamboozed.Length).ToArray();
            for (int i = 0; i < pices.Length; i++)
            {
                pices[i].sprite = item.pices[i];
            }
            for (int i = 0; i < bamboozed.Length; i++)
            {
                bamboozed[i].sprite = picked_bamboozed[i];
            }
            full.sprite = item.full;
            flip.Shuffle();
        }

        void Awake()
        {
            Init();
        }
    }
}
