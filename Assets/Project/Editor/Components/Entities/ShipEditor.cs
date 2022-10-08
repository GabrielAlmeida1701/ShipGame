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

        private void OnEnable()
        {
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
    }
}