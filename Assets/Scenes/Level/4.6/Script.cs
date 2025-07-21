using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using Game.Core;
using Games.DragNDrop;
using KenTank.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Button = KenTank.Systems.UI.Button;

namespace Minggu4_6
{
    public class Script : MonoBehaviour
    {
        [Serializable]
        class Group {
            public string catagory;
            public EventReference voice;
            public Sprite[] sprites;
        }

        [SerializeField] Slot slot;
        [SerializeField] Button voice;
        [SerializeField] Node[] nodes;
        [SerializeField] Group[] groups;
        [SerializeField] Group[] pelengkap;

        public UnityEvent onComplete;

        EventInstance audioInstance;
        List<bool> targetNodes = new();

        void Awake()
        {
            var lcg = new RandomLCG(DateTime.Now.Ticks);
            var selected = groups[lcg.Next(0, groups.Length)];

            voice.onClick.AddListener(() => {
                if (audioInstance.isValid()) audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                audioInstance = RuntimeManager.CreateInstance(selected.voice);
                audioInstance.start();
            });

            List<Sprite> temp = new();
            var seletedSprite = selected.sprites.ShuffleItems().Take(Mathf.RoundToInt(nodes.Length/2)).ToArray();
            var restSprite = groups.Concat(pelengkap).Where(x => x != selected).SelectMany(x => x.sprites).Where(x => !selected.sprites.Contains(x)).ToArray().ShuffleItems();
            var filterDuplicateRestSprite = new List<Sprite>();
            foreach (var item in restSprite)
            {
                if (!filterDuplicateRestSprite.Contains(item))
                {
                    filterDuplicateRestSprite.Add(item);
                }
            }

            temp.AddRange(seletedSprite);
            temp.AddRange(filterDuplicateRestSprite);

            List<Node> trueNode = new();
            Sprite[] sprites = temp.Take(nodes.Length).ToArray();
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].GetComponent<Image>().sprite = sprites[i];
                if (seletedSprite.Contains(sprites[i]))
                {
                    var index = i;
                    nodes[i].onValid.AddListener(() => {
                        nodes[index].GetComponent<GraphicRaycaster>().enabled = false;
                        nodes[index].GetComponent<Image>().color = new(1,1,1,0.3f);
                        targetNodes[index] = true;
                        Check();
                        slot.activeNode = null;
                    });
                    trueNode.Add(nodes[i]);
                    targetNodes.Add(false);
                }
            }
            slot.target = trueNode.ToArray();
        }

        void OnDisable()
        {
            audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        void Check() 
        {
            if (targetNodes.All(x => x))
            {
                onComplete.Invoke();
            }
        }
    }
}
