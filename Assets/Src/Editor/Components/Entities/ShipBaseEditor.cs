using ShipGame.Entities;
using System.Collections.Generic;
using UnityEditor;

namespace ShipGameEditor.Entities
{
    public class ShipBaseEditor : Editor
    {
        protected readonly string[] defaultReferenceProperties = { "shipState", "hitVFX", "cannonBall", "cannons" };
        protected readonly string[] defaultValuesProperties = { "life", "speed", "turnSpeed", "reloadTime" };

        private static bool showRefs;
        private static bool showGamplay = true;

        protected List<SerializedProperty> referencesProperties = new List<SerializedProperty>();
        protected List<SerializedProperty> gameplayProperties = new List<SerializedProperty>();

        public override void OnInspectorGUI()
        {
            EditorUtils.DefaultScriptField(target as Ship);

            showRefs = EditorGUILayout.Foldout(showRefs, "References");
            if (showRefs)
            {
                EditorGUI.indentLevel++;
                DrawReferencesFields();
                EditorGUI.indentLevel--;
            }

            showGamplay = EditorGUILayout.Foldout(showGamplay, "Gameplay Values");
            if (showGamplay)
            {
                EditorGUI.indentLevel++;
                DrawGameplayValuesFields();
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }

        protected List<SerializedProperty> GetProperties(string[] defaultOptions, params string[] properties)
        {
            List<string> allProperties = new List<string>(defaultOptions);
            allProperties.AddRange(properties);
            return GetProperties(allProperties.ToArray());
        }

        protected List<SerializedProperty> GetProperties(params string[] properties)
        {
            List<SerializedProperty> result = new List<SerializedProperty>();
            foreach (var prop in properties)
                result.Add(serializedObject.FindProperty(prop));
            return result;
        }

        protected virtual void DrawReferencesFields()
        {
            EditorUtils.DrawProperties(referencesProperties);
        }

        protected virtual void DrawGameplayValuesFields()
        {
            EditorUtils.DrawProperties(gameplayProperties);
        }
    }
}