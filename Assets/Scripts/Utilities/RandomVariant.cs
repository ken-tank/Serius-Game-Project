using System;
using System.Linq;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

public class RandomVariant : MonoBehaviour
{
    [Serializable] 
    class Group {
        public string name;
        public bool include = true;
        public Sprite[] sprites;
    }

    [SerializeField] Image[] images;
    [SerializeField] Group[] groups;

    void Awake()
    {
        var lcg = new RandomLCG(DateTime.Now.Ticks);
        var filtered_group = groups.Where(x => x.include).ToArray();
        var index = lcg.Next(0, filtered_group.Length);
        var seleceted = filtered_group[index];

        for (int i = 0; i < images.Length; i++)
        {
            var item = images[i];
            var sprite = seleceted.sprites[i];
            
            item.sprite = sprite;
        }
    }
}
