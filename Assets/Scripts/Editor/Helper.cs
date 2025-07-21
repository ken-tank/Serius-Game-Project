using Game.Utilities;
using Games.DragNDrop;
using Games.DrawGame;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

public class Helper : Editor
{
    [MenuItem("Helpers/GameObjects/RenameFromSpriteImage")]
    public static void RenameObjectWithCurrentSpriteImage() 
    {
        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        foreach (var item in Selection.gameObjects)
        {
            item.TryGetComponent(out Image image);
            if (image && image.sprite)
            {
                Undo.RecordObject(item, $"rename to {image.sprite.name}");
                item.name = image.sprite.name;
            }
        }
        Undo.CollapseUndoOperations(group);
    }
    
    [MenuItem("Helpers/GameObjects/TemporaryAction")]
    public static void TemporaryAction() 
    {   
        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        var items = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Data/Font/Fredoka.asset");
        foreach (var item in items)
        {
            Undo.RecordObject(item, "asdadw");
            item.font = font;
            if (item.color == Color.black)
            {
                ColorUtility.TryParseHtmlString("#5A1000", out Color color);
                item.color = color;
            }
        }
        Undo.CollapseUndoOperations(group);
    }   
}
