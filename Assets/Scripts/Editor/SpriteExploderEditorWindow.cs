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
        SpriteEploderWindowSettings serializedSettings;

        public int DefaultPixelSize
        {
            get { return EditorPrefs.GetInt("SpriteExploder_DefaultPixelSize"); }
            set { EditorPrefs.SetInt("SpriteExploder_DefaultPixelSize", value); }
        }

        public bool IsUsingCollision
        {
            get { return EditorPrefs.GetBool("SpriteExploder_IsUsingCollision"); }
            set { EditorPrefs.GetBool("SpriteExploder_IsUsingCollision", value); }
        }

        [MenuItem("Edit/Sprite Exploder Settings...")]
        static void OpenWindow()
        {
            GetWindow<SpriteExploderEditorWindow>();
        }

        void OnEnable()
        {
            serializedSettings = CreateInstance<SpriteEploderWindowSettings>();
            serializedSettings.Init(this);
            settingsEditor = UnityEditor.Editor.CreateEditor(serializedSettings);
        }

        void OnDisable()
        {
            Object.DestroyImmediate(serializedSettings);
            Object.DestroyImmediate(settingsEditor);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var buttonPosition = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, Styles.iconButton);

            if (EditorGUI.DropdownButton(buttonPosition, Styles.presetIcon, FocusType.Passive, Styles.iconButton))
            {
                var presetReceiver = CreateInstance<SpriteExploderSettingsReceiver>();
                presetReceiver.Init(serializedSettings, this);

                PresetSelector.ShowSelector(serializedSettings, null, true, presetReceiver);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            settingsEditor.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                serializedSettings.ApplySettings(this);
            }
        }
    }
}