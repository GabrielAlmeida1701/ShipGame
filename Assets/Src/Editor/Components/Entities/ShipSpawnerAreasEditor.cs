using ShipGame.Entities.Enemies;
using ShipGameEditor.CustomFields;
using System;
using System.Collections.Generic;
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
        private MultiRangeSlider multiRangeSlider;
        private static int selectedArea;

        private void OnEnable()
        {
            handler = new BoxBoundsHandle();
            multiRangeSlider = new MultiRangeSlider();
            avaliableEnemies = serializedObject.FindProperty("avaliableEnemies");

            multiRangeSlider.SetCallbacks(GetSliderItemLabel);

            if (spawner.spawnChance == null)
                spawner.spawnChance = new List<float>();

            if (spawner.avaliableAreas == null)
                spawner.avaliableAreas = new List<Bounds>();

            FixPercentages();
        }

        public override void OnInspectorGUI()
        {
            EditorUtils.DefaultScriptField(spawner);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(avaliableEnemies);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                FixPercentages();
            }
            multiRangeSlider.Draw("Chances of Spawning", ref spawner.spawnChance);

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
            if (spawner.avaliableAreas == null || selectedArea < 0 || selectedArea >= spawner.avaliableAreas.Count) return;

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
    
        private string GetSliderItemLabel(int id)
        {
            if (id >= avaliableEnemies.arraySize) return "Null";
            
            var enemy = avaliableEnemies.GetArrayElementAtIndex(id);
            if (!enemy.objectReferenceValue) return "Null";
            return enemy.objectReferenceValue.name;
        }

        private void FixPercentages()
        {
            if (spawner.spawnChance.Count == 0 && avaliableEnemies.arraySize == 0)
                return;

            int spawnChanceCount = spawner.spawnChance.Count;
            int diference = spawnChanceCount - avaliableEnemies.arraySize;
            
            float modified = 0;
            if (diference > 0)
            {
                for (int i = 0; i < diference; i++)
                {
                    modified += spawner.spawnChance[^1];
                    spawner.spawnChance.RemoveAt(spawnChanceCount - 1);
                }

                if (spawner.spawnChance.Count == 0)
                    return;
            }
            else
            {
                for (int i = 0; i < -diference; i++)
                {
                    modified += .2f;
                    spawner.spawnChance.Add(.2f);
                }
            }

            float total = 0;
            foreach (float val in spawner.spawnChance)
                total += val;
            total = (float) Math.Round(total, 4);

            if (modified == 0 && total == 1) return;
            
            float remaining = 1 - total;
            if (remaining > 0) spawner.spawnChance[^1] += remaining;
            else
            {
                for (int i = 0; i < spawnChanceCount; i++)
                    spawner.spawnChance[i] -= .2f / spawnChanceCount;
            }
        }
    }
}