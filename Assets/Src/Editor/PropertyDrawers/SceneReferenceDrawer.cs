using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using ShipGame.Systems.Attributes;

namespace ShipGameEditor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SceneReferenceAttribute))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }

            var targetScene = property.stringValue;
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(targetScene);

            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUI.ObjectField(position, label, scene, typeof(SceneAsset), false) as SceneAsset;
            if (EditorGUI.EndChangeCheck())
            {
                targetScene = AssetDatabase.GetAssetPath(newScene);
                var sceneIndex = SceneUtility.GetBuildIndexByScenePath(targetScene);
                if (newScene && sceneIndex == -1)
                    Debug.LogError("This scene is not part of the Build");

                property.stringValue = targetScene;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
