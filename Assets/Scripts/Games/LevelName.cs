using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelName : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textSource;
    
    void Awake()
    {
        var aa = SceneManager.GetActiveScene().name.Split(".");
        var text = textSource.text;
        var w = aa[0];
        var d = aa[1];
        text = text.Replace("{w}", w);
        text = text.Replace("{d}", d);
        textSource.SetText(text);
    }
}
