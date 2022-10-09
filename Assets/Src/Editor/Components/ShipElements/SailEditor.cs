using ShipGame.ShipElements;
using UnityEditor;

namespace ShipGameEditor.ShipElements
{
    [CustomEditor(typeof(Sail))]
    public class SailEditor : Editor
    {
        private SerializedProperty closedSailSize, openSailSize, closedSailPosition, openSailPosition;
        private bool showSailInfo;

        private void OnEnable()
        {
            closedSailSize = serializedObject.FindProperty("closedSailSize");
            openSailSize = serializedObject.FindProperty("openSailSize");
            closedSailPosition = serializedObject.FindProperty("closedSailPosition");
            openSailPosition = serializedObject.FindProperty("openSailPosition");
        }

        public override void OnInspectorGUI()
        {
            EditorUtils.DefaultScriptField(target as Sail);
            DrawPropertiesExcluding(
                serializedObject,
                "m_Script", "closedSailSize", "openSailSize", "closedSailPosition", "openSailPosition"
            );

            showSailInfo = EditorGUILayout.Foldout(showSailInfo, "Sail Transform Info.");
            if (showSailInfo)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(openSailPosition);
                EditorGUILayout.PropertyField(openSailSize);

                EditorGUILayout.PropertyField(closedSailPosition);
                EditorGUILayout.PropertyField(closedSailSize);
                EditorGUI.indentLevel--;
            }
        }
    }
}