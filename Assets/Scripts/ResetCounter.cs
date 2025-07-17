using KenTank.Core.SceneManager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countText;

    public static string level;
    public static int count = 5;

    public static void ResetLevel() 
    {
        if (level == SceneManager.GetActiveScene().name)
        {
            if (count <= 0)
            {
                SceneActions.LoadScene("Menu Level");
            }
            else
            {
                SceneActions.RestartScene();
                count--;
            }
        }
    }

    void Awake()
    {
        if (countText)
        {
            countText.SetText(count.ToString() + "x");
        }
    }
}
