using KenTank.Systems.UI.Buttons;
using UnityEngine;

namespace MenuLevel
{
    public class LevelSpriteDatabase : MonoBehaviour
    {
        [SerializeField] Sprite[] sprites;
        [SerializeField] IconButton[] buttons;

        void Awake()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i <= sprites.Length)
                {
                    if (!sprites[i]) continue;
                    buttons[i].sprite = sprites[i];
                }
            }
        }
    }
}
