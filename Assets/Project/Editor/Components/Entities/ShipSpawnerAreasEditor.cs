using ShipGame.Entities.Enemies;
using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ShipGameEditor.Components.Entities
{
    [CustomEditor(typeof(ShipSpawnerAreas))]
    public class ShipSpawnerAreasEditor : Editor
    {
        private static readonly Color SpawnableAreaColor = new Color(1, 0, 0, .2f);
        private static readonly Color SelectedSpawnableAreaColor = new Color(1, 0, 0, .4f);

        private ShipSpawnerAreas spawner => target as ShipSpawnerAreas;
        private bool showAreas;
        private BoxBoundsHandle handler;
        private SerializedProperty avaliableEnemies;
        private static int selectedArea;

        private void OnEnable()
        {
            handler = new BoxBoundsHandle();
            avaliableEnemies = serializedObject.FindProperty("avaliableEnemies");
        }

        public override void OnInspectorGUI()
        {
            EditorUtils.DefaultScriptField(spawner);

            EditorGUILayout.PropertyField(avaliableEnemies);
            serializedObject.ApplyModifiedProperties();

            showAreas = EditorGUILayout.Foldout(showAreas, "Avaliable Areas");
            if (showAreas)
                AvaliableAreasFields();
        }

        private void AvaliableAreasFields()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            DrawAreasList();
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    selectedArea = spawner.avaliableAreas.Count;
                    spawner.avaliableAreas.Add(new Bounds(Vector3.zero, Vector3.one));
                }

                GUI.enabled = spawner.avaliableAreas.Count > 0;
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    int selected = selectedArea == -1 ? spawner.avaliableAreas.Count - 1 : selectedArea;
                    if (selectedArea == selected)
                    {
                        selectedArea = selected - 1;
                        if (selectedArea < 0 && spawner.avaliableAreas.Count > 0)
                            selectedArea = 0;
                    }

                    spawner.avaliableAreas.RemoveAt(selected);
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
                EditorUtils.SetDirty(spawner);
        }

        private void DrawAreasList()
        {
            for (int i = 0; i < spawner.avaliableAreas.Count; i++)
            {
                try
                {
                    Bounds value = spawner.avaliableAreas[i];
                    EditorGUILayout.BeginHorizontal();
                    bool selected = i == selectedArea;
                    GUI.color = selected ? Color.green : Color.white;

                    GUI.enabled = false;
                    EditorGUILayout.BoundsField("Area " + i, value);
                    GUI.enabled = true;

                    if (GUILayout.Button("<-", GUILayout.Width(30)))
                    {
                        selectedArea = selected ? -1 : i;
                        SceneView.lastActiveSceneView?.Repaint();
                    }

                    GUI.color = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
                catch (ArgumentOutOfRangeException) { i = 0; }
            }
        }

        private void OnSceneGUI()
        {
            if (selectedArea < 0 || selectedArea >= spawner.avaliableAreas.Count) return;

            EditorGUI.BeginChangeCheck();
            Bounds bounds = spawner.avaliableAreas[selectedArea];

            var position = Handles.PositionHandle(bounds.center, Quaternion.identity);
            handler.center = position;
            handler.size = bounds.size;
            handler.DrawHandle();

            position.z = 0;
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtils.SetDirty(target);
                bounds.center = position;
                bounds.size = handler.size;

                spawner.avaliableAreas[selectedArea] = bounds;
                serializedObject.ApplyModifiedProperties();
            }
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NonSelected)]
        private static void OnDrawGizmos(ShipSpawnerAreas spawner, GizmoType _)
        {
            for (int i = 0; i < spawner.avaliableAreas.Count; i++)
            {
                try
                {
                    Gizmos.color = i == selectedArea? SelectedSpawnableAreaColor : SpawnableAreaColor;
                    Gizmos.DrawCube(spawner.avaliableAreas[i].center, spawner.avaliableAreas[i].size);
                    Gizmos.color = Color.white;
                }
                catch (ArgumentOutOfRangeException) { i = 0; }
            }
        }
    }
}