using KenTank.Systems.UI.Procedural;
using KenTank.Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace KenTank.Systems.UI.Editor
{
    public class UIManager_Editor : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/KenTank/Button/TextButton", false)]
        static void CreateTextButton()
        {
            HirearchyExtensions.CreateInstance("UI/Button/TextButton");
        }
        [MenuItem("GameObject/UI/KenTank/Button/Procedural TextButton", false)]
        static void CreateProceduralTextButton()
        {
            HirearchyExtensions.CreateInstance("UI/Button/Procedural TextButton");
        }
        [MenuItem("GameObject/UI/KenTank/Button/IconButton", false)]
        static void CreateIconButton()
        {
            HirearchyExtensions.CreateInstance("UI/Button/IconButton");
        }
        [MenuItem("GameObject/UI/KenTank/Button/Procedural IconButton", false)]
        static void CreateProceduralIconButton()
        {
            HirearchyExtensions.CreateInstance("UI/Button/Procedural IconButton");
        }
        [MenuItem("GameObject/UI/KenTank/Window/DialogBox", false)]
        static void CreateDialogBox()
        {
            HirearchyExtensions.CreateInstance("UI/Window/DialogBox");
        }

        [MenuItem("GameObject/UI/KenTank/Window/ConfirmationBox", false)]
        static void CreateConfirmationBox()
        {
            HirearchyExtensions.CreateInstance("UI/Window/ConfirmationBox");
        }
        [MenuItem("GameObject/UI/KenTank/Input/Dropdown", false)]
        static void CreateDropdown()
        {
            HirearchyExtensions.CreateInstance("UI/Input/Dropdown");
        }
        [MenuItem("GameObject/UI/KenTank/Input/Procedural Dropdown", false)]
        static void CreateProceduralDropdown()
        {
            HirearchyExtensions.CreateInstance("UI/Input/Procedural Dropdown");
        }
        [MenuItem("GameObject/UI/KenTank/Text/LabelValue", false)]
        static void CreateLabelValue()
        {
            HirearchyExtensions.CreateInstance("UI/Label/LabelValue");
        }
        [MenuItem("GameObject/UI/KenTank/Input/Slider", false)]
        static void CreateSlider()
        {
            HirearchyExtensions.CreateInstance("UI/Input/Slider");
        }
        [MenuItem("GameObject/UI/KenTank/Input/TextField", false)]
        static void CreateTextField()
        {
            HirearchyExtensions.CreateInstance("UI/Input/TextField");
        }
        [MenuItem("GameObject/UI/KenTank/Input/Toggle", false)]
        static void CreateToggle()
        {
            HirearchyExtensions.CreateInstance("UI/Input/Toggle");
        }

        [MenuItem("GameObject/UI/KenTank/Animation/UI Transition", false)]
        public static void CreateAwakeAnimateRoot()
        {
            var selected = Selection.activeTransform;

            // Undo group start
            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();

            // Create objects with Undo
            var root = new GameObject("AnimationRect");
            Undo.RegisterCreatedObjectUndo(root, "Create AnimationRect");

            var subroot = new GameObject("Root");
            Undo.RegisterCreatedObjectUndo(subroot, "Create Root");

            // Parent setting
            Undo.SetTransformParent(root.transform, selected, "Set Parent");
            Undo.SetTransformParent(subroot.transform, root.transform, "Set Parent");

            // Add Components
            var root_rect = Undo.AddComponent<RectTransform>(root);
            var root_canvas = Undo.AddComponent<CanvasGroup>(root);
            var root_animate = Undo.AddComponent<UITransition>(root);
            var subroot_rect = Undo.AddComponent<RectTransform>(subroot);

            // Rect setup
            root_rect.anchorMin = new Vector2(0, 0);
            root_rect.anchorMax = new Vector2(1, 1);
            root_rect.sizeDelta = Vector2.zero;
            root_rect.anchoredPosition = Vector2.zero;

            subroot_rect.anchorMin = new Vector2(0, 0);
            subroot_rect.anchorMax = new Vector2(1, 1);
            subroot_rect.sizeDelta = Vector2.zero;
            subroot_rect.anchoredPosition = Vector2.zero;

            root.transform.localScale = Vector3.one;

            // Serialized Object changes
            var so = new SerializedObject(root_animate);
            so.FindProperty("alpha").objectReferenceValue = root_canvas;
            so.FindProperty("root").objectReferenceValue = subroot_rect;
            so.ApplyModifiedProperties();

            // Set selection
            Selection.activeTransform = root.transform;

            Undo.CollapseUndoOperations(group);
        }

        [MenuItem("GameObject/UI/KenTank/Animation/UI Transition (From Selected)", false)]
        public static void CreateAwakeAnimateRootSelected()
        {
            var selecteds = Selection.transforms;
            if (selecteds.Length == 0)
            {
                Debug.LogWarning("No selected objects.");
                return;
            }

            // Start undo group outside loop
            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            Transform lastCreated = null;

            foreach (var selected in selecteds)
            {
                if (!selected.TryGetComponent(out RectTransform selected_rect))
                {
                    Debug.LogWarning($"GameObject '{selected.name}' does not have a RectTransform.");
                    continue; // Skip instead of return
                }

                if (!selected.parent)
                {
                    Debug.LogWarning($"GameObject '{selected.name}' does not have a parent.");
                    continue;
                }

                int selectedIndex = selected.GetSiblingIndex();

                // === Create Wrapper Hierarchy ===
                var root = new GameObject("AnimationRect");
                Undo.RegisterCreatedObjectUndo(root, "Create AnimationRect");

                var subroot = new GameObject("Root");
                Undo.RegisterCreatedObjectUndo(subroot, "Create Root");

                var root_rect = Undo.AddComponent<RectTransform>(root);
                var root_canvas = Undo.AddComponent<CanvasGroup>(root);
                var root_animate = Undo.AddComponent<UITransition>(root);
                var subroot_rect = Undo.AddComponent<RectTransform>(subroot);

                // Parent assignment with undo
                Undo.SetTransformParent(root.transform, selected.parent, "Reparent Root");
                Undo.SetTransformParent(subroot.transform, root.transform, "Reparent SubRoot");

                // Layout for RectTransforms
                subroot_rect.anchorMin = Vector2.zero;
                subroot_rect.anchorMax = Vector2.one;
                subroot_rect.sizeDelta = Vector2.zero;
                subroot_rect.anchoredPosition = Vector2.zero;

                root_rect.anchorMin = selected_rect.anchorMin;
                root_rect.anchorMax = selected_rect.anchorMax;
                root_rect.sizeDelta = selected_rect.sizeDelta;
                root_rect.anchoredPosition = selected_rect.anchoredPosition;

                root.transform.localScale = Vector3.one;

                // Setup Serialized Properties
                var so = new SerializedObject(root_animate);
                so.FindProperty("alpha").objectReferenceValue = root_canvas;
                so.FindProperty("root").objectReferenceValue = subroot_rect;
                so.ApplyModifiedProperties();

                // Move selected into subroot
                Undo.SetTransformParent(selected, subroot.transform, "Reparent Selected");

                // Record before modifying transform
                Undo.RecordObject(selected_rect, "Modify RectTransform");
                selected_rect.anchorMin = Vector2.zero;
                selected_rect.anchorMax = Vector2.one;
                selected_rect.sizeDelta = Vector2.zero;
                selected_rect.anchoredPosition = Vector2.zero;

                // Rename and set sibling index
                root.name = selected.name;
                root.transform.SetSiblingIndex(selectedIndex);

                lastCreated = root.transform;
            }

            // Set last created as active (optional)
            if (lastCreated)
            {
                Selection.activeTransform = lastCreated;
            }

            // Collapse all undo operations to a single action
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/UI/KenTank/Shape/Rectangle", false)]
        public static void CreateProceduralRoundedRectangle() 
        {
            var selected = Selection.activeTransform;

            var go = new GameObject("Procedural Rectangle");
            var rect_go = go.AddComponent<RectTransform>();
            go.AddComponent<ProceduralRoundedRectangle>();
            go.transform.SetParent(selected);
            rect_go.anchoredPosition = Vector2.zero;
            rect_go.sizeDelta = new (100, 100);
            go.transform.localPosition = Vector2.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Procedural Rectangle");
        }

        [MenuItem("GameObject/UI/KenTank/Shape/Circle", false)]
        public static void CreateProceduralCircle() 
        {
            var selected = Selection.activeTransform;

            var go = new GameObject("Procedural Circle");
            var rect_go = go.AddComponent<RectTransform>();
            go.AddComponent<ProceduralCircle>();
            go.transform.SetParent(selected);
            rect_go.anchoredPosition = Vector2.zero;
            rect_go.sizeDelta = new (100, 100);
            go.transform.localPosition = Vector2.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Procedural Circle");
        }
    }
}