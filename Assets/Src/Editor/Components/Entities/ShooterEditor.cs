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

        private bool showRange;
        private SerializedProperty minDist, maxDist, layers;
        private Transform reference;

        protected override void OnEnable()
        {
            base.OnEnable();
            layers = serializedObject.FindProperty("layers");
            minDist = serializedObject.FindProperty("minDist");
            maxDist = serializedObject.FindProperty("maxDist");

            var player = FindObjectOfType<Player>();
            if (player) reference = player.transform;
            else reference = ship.transform;

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
            showRange = EditorGUILayout.Foldout(showRange, "Range Settings");
            if (showRange)
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
            if (maxDist == null || minDist == null) return;

            Handles.color = middleCircle;
            Handles.RadiusHandle(Quaternion.identity, reference.position, (maxDist.floatValue + minDist.floatValue) / 2);

            innerCircle.a = .2f;
            Handles.color = innerCircle;
            Handles.DrawSolidArc(reference.position, Vector3.forward, Vector3.up, 360, minDist.floatValue);

            innerCircle.a = 1;
            Handles.color = innerCircle;
            minDist.floatValue = Handles.RadiusHandle(Quaternion.identity, reference.position, minDist.floatValue);
            minDist.floatValue = Mathf.Clamp(minDist.floatValue, .5f, maxDist.floatValue);

            outterCircle.a = .05f;
            Handles.color = outterCircle;
            Handles.DrawSolidArc(reference.position, Vector3.forward, Vector3.up, 360, maxDist.floatValue);

            outterCircle.a = 1;
            Handles.color = outterCircle;
            maxDist.floatValue = Handles.RadiusHandle(Quaternion.identity, reference.position, maxDist.floatValue);
            if (maxDist.floatValue < .5f) maxDist.floatValue = .5f;

            serializedObject.ApplyModifiedProperties();
        }
    }
}