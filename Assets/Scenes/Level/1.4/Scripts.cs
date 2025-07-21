using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Minggu1_4
{
    public class Scripts : MonoBehaviour
    {
        [System.Serializable] 
        class Catagory {
            public string name;
            public Color color;
            public Image[] images;
        }

        class SelectedItem {
            public string catagory;
            public Image image;
        }

        [SerializeField] Catagory[] catagories;

        public UnityEvent onComplete;

        List<Image> allImages = new();
        List<SelectedItem> selected = new();

        void Check(string catagory)
        {
            var item = catagories.FirstOrDefault(x => x.name == catagory);
            if (item.images.Length == selected.Count)
            {
                foreach (var a in selected)
                {
                    a.image.gameObject.SetActive(false);
                }
                selected.Clear();
            }
            Status();
        }

        void Status() 
        {
            if (allImages.All(x => !x.gameObject.activeSelf))
            {
                onComplete.Invoke();
            }
        }

        void Awake()
        {
            foreach (var a in catagories)
            {
                foreach (var b in a.images)
                {
                    allImages.Add(b);
                    var button = b.GetComponent<Button>();
                    button.onClick.AddListener(() => {
                        if (selected.Count > 0)
                        {
                            if (selected.Last().catagory != a.name)
                            {
                                return;
                            }
                        }
                        b.color = a.color;
                        selected.Add(new SelectedItem{
                            catagory = a.name,
                            image = b
                        });
                        Check(a.name);
                    });
                }
            }
        }
    }
}
