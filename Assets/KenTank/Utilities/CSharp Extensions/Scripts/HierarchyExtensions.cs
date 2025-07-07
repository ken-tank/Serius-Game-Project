using UnityEngine;

namespace KenTank.Utilities.Extensions
{
    #if UNITY_EDITOR
    public static class HirearchyExtensions
    {
        public static void CreateInstance(string path)
        {
            var selection = UnityEditor.Selection.activeTransform;
            var data = Resources.Load<GameObject>(path);
            var item = UnityEditor.PrefabUtility.InstantiatePrefab(data) as GameObject;
            if (selection) 
            {
                item.transform.SetParent(selection);
                if (item.TryGetComponent(out RectTransform rect))
                {
                    var original = data.GetComponent<RectTransform>();
                    rect.localPosition = original.localPosition;
                    rect.localEulerAngles = original.localEulerAngles;
                    rect.sizeDelta = original.sizeDelta;
                    rect.localScale = data.transform.localScale;
                }
                else
                {
                    item.transform.localPosition = data.transform.localPosition;
                    item.transform.localEulerAngles = data.transform.localEulerAngles;
                    item.transform.localScale = data.transform.localScale;
                }
            }
            UnityEditor.Undo.RegisterCreatedObjectUndo(item, "Create " + item.name);
            UnityEditor.Selection.activeObject = item;
        }
    }
    #endif
}