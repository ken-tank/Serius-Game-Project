using System;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using Game.Core;
using Game.Utilities;
using KenTank.Systems.UI;
using KenTank.Systems.UI.Buttons;
using KenTank.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Minggu4_5
{
    public class Script : MonoBehaviour
    {
        [Serializable] 
        class Item {
            public EventReference voice;
            public Sprite sprite;
        }

        [SerializeField] Button voice;
        [SerializeField] Button[] choices;
        [SerializeField] FlipPositionElement flip;
        [SerializeField] Item[] items;
        [SerializeField] Item[] other;

        public UnityEvent onTrue;

        EventInstance voicecInstance;

        void Awake()
        {
            var lcg = new RandomLCG(DateTime.Now.Ticks);
            var selected = items[lcg.Next(0, items.Length)];
            var other = items.Concat(this.other).Where(x => x != selected).ToArray().ShuffleItems();
            var filtered = other.ToList();
            filtered.Insert(0, selected);
            var taked = filtered.Take(choices.Length).ToArray();

            voice.onClick.AddListener(() => {
                if (voicecInstance.isValid()) voicecInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                voicecInstance = RuntimeManager.CreateInstance(selected.voice);
                voicecInstance.start();
            });

            for (int i = 0; i < choices.Length; i++)
            {
                choices[i].GetComponent<IconButton>().sprite = taked[i].sprite;
                var index = i;
                choices[i].onClick.AddListener(() => {
                    if (index == 0)
                    {
                        onTrue.Invoke();
                    }
                    else
                    {
                        RuntimeManager.PlayOneShot("event:/UI/Warning1");
                    }
                });
            }

            flip.Shuffle();
        }

        void OnDisable()
        {
            if (voicecInstance.isValid()) voicecInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
