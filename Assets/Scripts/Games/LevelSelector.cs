using KenTank.Core.SceneManager;
using KenTank.Systems.UI;
using UnityEngine;

namespace Games
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] GameObject[] minggu_list;

        void Awake()
        {
            for (int i = 0; i < minggu_list.Length; i++)
            {
                var item = minggu_list[i];
                var h = 1;
                for (int a = 0; a < 2; a++)
                {
                    for (int b = 0; b < 3; b++)
                    {
                        var button = item.transform.GetChild(a).GetChild(b).GetChild(0).GetChild(0).GetComponent<Button>();
                        var target = $"{i+1}.{h}";
                        button.onClick.AddListener(() => {
                            ResetCounter.count = 5;
                            ResetCounter.level = target;
                            SceneActions.LoadScene(target);
                        });
                        h++;
                    }
                }
            }
        }
    }
}