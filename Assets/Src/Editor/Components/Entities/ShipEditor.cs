using ShipGame.Entities;
using ShipGame.Environment.Cannons;
using ShipGame.ShipElements;
using UnityEditor;
using UnityEngine;

namespace ShipGameEditor.Entities
{
    [CustomEditor(typeof(Ship), true)]
    public class ShipEditor : ShipBaseEditor
    {
        protected Ship ship => target as Ship;

        private static bool showObstacleAvoidance;
        private SerializedProperty groundLayer;
        private SerializedProperty viewDistance;

        protected virtual void OnEnable()
        {
            groundLayer = serializedObject.FindProperty("groundLayer");
            viewDistance = serializedObject.FindProperty("viewDistance");

            referencesProperties = GetProperties(defaultReferenceProperties);
            gameplayProperties = GetProperties(defaultValuesProperties);
            LoadReferences();
        }

        protected void LoadReferences()
        {
            var shipState = serializedObject.FindProperty("shipState");
            if (!shipState.objectReferenceValue)
            {
                shipState.objectReferenceValue = ship.GetComponent<ShipState>();
                serializedObject.ApplyModifiedProperties();
            }

            var cannons = serializedObject.FindProperty("cannons");
            if (cannons.arraySize == 0)
            {
                var childCannons = ship.GetComponentsInChildren<Cannon>();
                for (int i = 0; i < childCannons.Length; i++)
                {
                    cannons.InsertArrayElementAtIndex(i);
                    var newCannon = cannons.GetArrayElementAtIndex(i);
                    newCannon.objectReferenceValue = childCannons[i];
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            showObstacleAvoidance = EditorGUILayout.Foldout(showObstacleAvoidance, "Obstacle Avoidance Settings");
            if (showObstacleAvoidance)
            {
                EditorGUILayout.PropertyField(groundLayer);
                EditorGUILayout.PropertyField(viewDistance);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}