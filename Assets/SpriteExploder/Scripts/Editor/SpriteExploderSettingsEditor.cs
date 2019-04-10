using UnityEngine;
using UnityEditor;

namespace RCG.SpriteExploder.Editor
{
    [CustomEditor(typeof(SpriteExploderSettings))]
    public class SpriteExploderSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty minimumParticlePixelSize;
        SerializedProperty isCollidable;

        void OnEnable()
        {
            minimumParticlePixelSize = serializedObject.FindProperty("minimumParticlePixelSize");
            isCollidable = serializedObject.FindProperty("isCollidable");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(minimumParticlePixelSize, new GUIContent("Min Particle Pixel Size"));
            EditorGUILayout.PropertyField(isCollidable);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
