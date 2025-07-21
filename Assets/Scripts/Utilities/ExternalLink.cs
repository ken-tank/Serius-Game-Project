using UnityEngine;

public class ExternalLink : MonoBehaviour
{
    [SerializeField, TextArea(minLines:1, maxLines:3)] string link;

    public void OpenCurrentLink()
    {
        OpenLink(link);
    }

    public static void OpenLink(string url) 
    {
        Application.OpenURL(url);
    }
}
