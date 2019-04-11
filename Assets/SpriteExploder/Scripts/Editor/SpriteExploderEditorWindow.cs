using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

namespace RCG.SpriteExploder.Editor
{
    public class SpriteExploderEditorWindow : EditorWindow

    {
        private static class Styles
        {
            public static GUIContent presetIcon = EditorGUIUtility.IconContent("Preset.Context");
            public static GUIStyle iconButton = new GUIStyle("IconButton");
        }

        UnityEditor.Editor settingsEditor;

        SpriteExploderSettings serializedSettings;
        SpriteExploderSettings SerializedSettings
        {
            get
            {
                if (serializedSettings == null)
                {
                    serializedSettings = SpriteExploderSettings.GetResource();
                }
                return serializedSettings;
            }
        }

        [MenuItem("Edit/Sprite Exploder Settings...")]
        static void OpenWindow()
        {
            SpriteExploderEditorWindow window = GetWindow<SpriteExploderEditorWindow>();
            window.titleContent = new GUIContent("Sprite Exploder Settings");
        }

        void OnEnable()
        {
            settingsEditor = UnityEditor.Editor.CreateEditor(SerializedSettings);
        }

        void OnDisable()
        {
            DestroyImmediate(settingsEditor);
        }

        void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            settingsEditor.OnInspectorGUI();
        }
    }
}