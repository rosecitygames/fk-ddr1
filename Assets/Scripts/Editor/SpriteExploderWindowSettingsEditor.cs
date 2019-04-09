using UnityEditor;

namespace RCG.SpriteExploder.Editor
{
    [CustomEditor(typeof(SpriteEploderWindowSettings))]
    public class SpriteExploderWindowSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty defaultPixelSize;
        SerializedProperty isUsingCollision;

        void OnEnable()
        {
            defaultPixelSize = serializedObject.FindProperty("defaultPixelSize");
            isUsingCollision = serializedObject.FindProperty("isUsingCollision");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(defaultPixelSize);
            EditorGUILayout.PropertyField(isUsingCollision);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
