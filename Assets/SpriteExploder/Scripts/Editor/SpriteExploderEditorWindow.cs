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
            GetWindow<SpriteExploderEditorWindow>();
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
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var buttonPosition = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, Styles.iconButton);

            if (EditorGUI.DropdownButton(buttonPosition, Styles.presetIcon, FocusType.Passive, Styles.iconButton))
            {
                var presetReceiver = CreateInstance<SpriteExploderSettingsReceiver>();
                presetReceiver.Init(SerializedSettings, this);

                PresetSelector.ShowSelector(SerializedSettings, null, true, presetReceiver);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            settingsEditor.OnInspectorGUI();
        }
    }
}