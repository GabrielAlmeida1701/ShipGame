using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ShipGameEditor
{
    public class EditorUtils
    {
        public static void DefaultScriptField(MonoBehaviour target)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target), target.GetType(), false);
            GUI.enabled = true;
        }

        public static void DrawProperties(List<SerializedProperty> referencesProps)
        {
            foreach(var prop in referencesProps)
                EditorGUILayout.PropertyField(prop);
        }

        public static void Header(string label, bool useSpace = true)
        {
            if (useSpace) GUILayout.Space(EditorGUIUtility.singleLineHeight);
            GUILayout.Label(label, EditorStyles.boldLabel);
        }

        public static void SetDirty(Object target, string description = null)
        {
            Undo.RegisterCompleteObjectUndo(target, description ?? "Change " + target.name);
            EditorUtility.SetDirty(target);

            if (target is Component || target is GameObject)
            {
                if (target is GameObject) EditorSceneManager.MarkSceneDirty((target as GameObject).scene);
                else EditorSceneManager.MarkSceneDirty((target as Component).gameObject.scene);
            }
        }
    }
}
