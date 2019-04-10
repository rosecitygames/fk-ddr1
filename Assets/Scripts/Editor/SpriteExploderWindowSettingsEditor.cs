using UnityEditor;

namespace RCG.SpriteExploder.Editor
{
    [CustomEditor(typeof(SpriteExploderSettings))]
    public class SpriteExploderWindowSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty particlePixelSize;
        SerializedProperty isUsingCollision;

        void OnEnable()
        {
            particlePixelSize = serializedObject.FindProperty("particlePixelSize");
            isUsingCollision = serializedObject.FindProperty("isUsingCollision");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(particlePixelSize);
            EditorGUILayout.PropertyField(isUsingCollision);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
