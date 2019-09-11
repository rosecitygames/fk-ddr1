using UnityEditor;
using UnityEngine;

namespace IndieDeveloperTools.SpriteExploder.Editor
{
    /// <summary>
    /// The editor window for the sprite exploder settings.
    /// </summary>
    public class SpriteExploderEditorWindow : EditorWindow
    {
        // An instance of the sprite exploder settings editor.
        UnityEditor.Editor settingsEditor;

        /// <summary>
        /// A reference to the settings scriptable object resource.
        /// </summary>
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
        SpriteExploderSettings serializedSettings;

        /// <summary>
        /// Helper method to get a referece to the editor window.
        /// </summary>
        [MenuItem("Edit/Sprite Exploder Settings...")]
        static void OpenWindow()
        {
            SpriteExploderEditorWindow window = GetWindow<SpriteExploderEditorWindow>();
            window.titleContent = new GUIContent("Sprite Exploder Settings");
        }

        /// <summary>
        /// Creates an instance of the settings editor from the serialized settings. 
        /// </summary>
        void OnEnable()
        {
            settingsEditor = UnityEditor.Editor.CreateEditor(SerializedSettings);
        }

        /// <summary>
        /// Destroy the settings editor instance when the editor window is disabled.
        /// </summary>
        void OnDisable()
        {
            DestroyImmediate(settingsEditor);
        }

        /// <summary>
        /// Draw the editor GUI.
        /// </summary>
        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            settingsEditor.OnInspectorGUI();
        }
    }
}