using System;
using Game.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI 
{
    public class RandomPickSprite : MonoBehaviour
    {
        [SerializeField] Sprite[] sprites;
        
        [SerializeField] UnityEvent<Sprite> onSelected;

        void Awake()
        {
            var offsetSeed = gameObject.GetInstanceID();
            var seed = (int)DateTime.Now.Ticks + offsetSeed;
            var lcg = new RandomLCG(seed);
            var random_index = lcg.Next(0, sprites.Length);
            var sprite = sprites[random_index];
            onSelected.Invoke(sprite);
        }
    }
}