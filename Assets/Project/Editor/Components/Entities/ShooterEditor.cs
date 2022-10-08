using ShipGame.Entities.Enemies;
using ShipGame.Entities.PlayerEntity;
using UnityEditor;
using UnityEngine;

namespace ShipGameEditor.Entities
{
    [CustomEditor(typeof(Shooter))]
    public class ShooterEditor : ShipEditor
    {
        private Color innerCircle = new Color(.5f, .7f, 1);
        private Color outterCircle = new Color(.31f, .46f, .66f);
        private Color middleCircle = new Color(1f, 0, 0, .6f);

        private bool showAll;
        private SerializedProperty minDist, maxDist, layers;
        private Player player;

        private void OnEnable()
        {
            referencesProperties = GetProperties(defaultReferenceProperties);
            gameplayProperties = GetProperties(defaultValuesProperties);
            LoadReferences();

            layers = serializedObject.FindProperty("layers");
            minDist = serializedObject.FindProperty("minDist");
            maxDist = serializedObject.FindProperty("maxDist");

            player = FindObjectOfType<Player>();

            if (maxDist.floatValue < .5f)
            {
                maxDist.floatValue = .5f;
                serializedObject.ApplyModifiedProperties();
            }
            
            if (minDist.floatValue > maxDist.floatValue)
            {
                minDist.floatValue = maxDist.floatValue;
                serializedObject.ApplyModifiedProperties();
            }
            else if (minDist.floatValue < .5f)
            {
                maxDist.floatValue = .5f;
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            showAll = EditorGUILayout.Foldout(showAll, "Range Settings");
            if (showAll)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(layers);
                EditorGUILayout.PropertyField(maxDist);
                EditorGUILayout.PropertyField(minDist);
                serializedObject.ApplyModifiedProperties();
                EditorGUI.indentLevel--;
            }
        }

        private void OnSceneGUI()
        {
            if (!player || maxDist == null || minDist == null) return;

            Handles.color = middleCircle;
            Handles.RadiusHandle(Quaternion.identity, player.transform.position, (maxDist.floatValue + minDist.floatValue) / 2);

            innerCircle.a = .2f;
            Handles.color = innerCircle;
            Handles.DrawSolidArc(player.transform.position, Vector3.forward, Vector3.up, 360, minDist.floatValue);

            innerCircle.a = 1;
            Handles.color = innerCircle;
            minDist.floatValue = Handles.RadiusHandle(Quaternion.identity, player.transform.position, minDist.floatValue);
            minDist.floatValue = Mathf.Clamp(minDist.floatValue, .5f, maxDist.floatValue);

            outterCircle.a = .05f;
            Handles.color = outterCircle;
            Handles.DrawSolidArc(player.transform.position, Vector3.forward, Vector3.up, 360, maxDist.floatValue);

            outterCircle.a = 1;
            Handles.color = outterCircle;
            maxDist.floatValue = Handles.RadiusHandle(Quaternion.identity, player.transform.position, maxDist.floatValue);
            if (maxDist.floatValue < .5f) maxDist.floatValue = .5f;
            serializedObject.ApplyModifiedProperties();
        }
    }
}