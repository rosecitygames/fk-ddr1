using UnityEngine;
using UnityEditor;

namespace IndieDevTools.SpriteExploder.Editor
{
    /// <summary>
    /// Custom editor for the sprite exploder settings.
    /// </summary>
    [CustomEditor(typeof(SpriteExploderSettings))]
    public class SpriteExploderSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty minimumParticlePixelSize;
        SerializedProperty isCollidable;

        /// <summary>
        /// Set the serialized property values from the serealized settings object.
        /// </summary>
        void OnEnable()
        {
            minimumParticlePixelSize = serializedObject.FindProperty("minimumParticlePixelSize");
            isCollidable = serializedObject.FindProperty("isCollidable");
        }

        /// <summary>
        /// Draw the instpector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(minimumParticlePixelSize, new GUIContent("Min Particle Pixel Size"));
            EditorGUILayout.PropertyField(isCollidable);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
