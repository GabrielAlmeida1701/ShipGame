using UnityEditor;
using ShipGame.Entities.PlayerEntity;
using ShipGame.UI.Elements;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace ShipGameEditor.Entities
{
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : ShipBaseEditor
    {
        private static readonly Color PlayableAreaColor = new Color(0, 1, 0, .2f);
        private static bool showShake, showPlayableArea;

        private Player player => target as Player;
        private SerializedProperty shakeOnHit, shakeOnDead, playbleArea;
        private BoxBoundsHandle handler;

        private void OnEnable()
        {
            handler = new BoxBoundsHandle();
            referencesProperties = GetProperties("shipState", "sail", "cameraFollow", "hitVFX", "cannonBall", "cannons");
            gameplayProperties = GetProperties(defaultValuesProperties, "movementDrag");

            shakeOnHit = serializedObject.FindProperty("shakeOnHit");
            shakeOnDead = serializedObject.FindProperty("shakeOnDead");
            playbleArea = serializedObject.FindProperty("playbleArea");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            showShake = EditorGUILayout.Foldout(showShake, "Camera Shake Settings");
            if (showShake)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(shakeOnHit);
                EditorGUILayout.PropertyField(shakeOnDead);
                EditorGUI.indentLevel--;
            }

            showPlayableArea = EditorGUILayout.Foldout(showPlayableArea, "PlayableArea");
            if(showPlayableArea)
            {
                if (SceneView.lastActiveSceneView != null)
                    SceneView.lastActiveSceneView.Repaint();

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(playbleArea);
                Vector2 area = playbleArea.vector2Value;
                if (area.x < 1) area.x = 1;
                if (area.y < 1) area.y = 1;
                playbleArea.vector2Value = area;
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }

        protected override void DrawReferencesFields()
        {
            base.DrawReferencesFields();

            EditorGUI.BeginChangeCheck();
            player.healthBar = EditorGUILayout.ObjectField("Health Bar", player.healthBar, typeof(HealthBar), true) as HealthBar;
            if (EditorGUI.EndChangeCheck())
                EditorUtils.SetDirty(target);
        }

        private void OnSceneGUI()
        {
            if (!showPlayableArea) return;

            EditorGUI.BeginChangeCheck();
            handler.center = Vector3.zero;
            handler.size = playbleArea.vector2Value;
            handler.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtils.SetDirty(target);
                playbleArea.vector2Value = handler.size;
                serializedObject.ApplyModifiedProperties();
            }
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        public static void OnDrawGizmos(Player player, GizmoType _)
        {
            if (!showPlayableArea) return;

            SerializedProperty playbleArea = new SerializedObject(player).FindProperty("playbleArea");

            Vector3 size = playbleArea.vector2Value;
            size.z = 1;

            Gizmos.color = PlayableAreaColor;
            Gizmos.DrawCube(Vector3.zero, size);
            Gizmos.color = Color.white;
        }
    }
}