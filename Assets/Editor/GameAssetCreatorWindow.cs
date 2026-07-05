using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Slafurry.Editor.GameAssetCreator
{
    /// <summary>
    /// Thin window that just hosts a button - clicking it opens
    /// GameAssetCreatorDropdown, an AdvancedDropdown that supports nested
    /// categories (folder within folder) and has search built in.
    /// </summary>
    public class GameAssetCreatorWindow : EditorWindow
    {
        [MenuItem("Slafurry/Create Game Asset")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameAssetCreatorWindow>("Create Game Asset");
            window.minSize = new Vector2(280, 70);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);

            Rect buttonRect = EditorGUILayout.GetControlRect(GUILayout.Height(30));
            if (GUI.Button(buttonRect, "Select Asset Type to Create", EditorStyles.popup))
            {
                var dropdown = new GameAssetCreatorDropdown(new AdvancedDropdownState());
                dropdown.Show(buttonRect);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "New asset is created inside whichever folder is currently selected in the Project window.",
                MessageType.Info);
        }
    }
}