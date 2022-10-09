using ShipGame;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ShipGameEditor.Components
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        private bool canLoadPlayer;
        private bool canLoadMenu;
        private bool canLoadGame;

        private SerializedProperty playerPath;
        private SerializedProperty menuPath;
        private SerializedProperty gameplayPath;

        private void OnEnable()
        {
            playerPath = serializedObject.FindProperty("playerScene");
            menuPath = serializedObject.FindProperty("mainMenuScene");
            gameplayPath = serializedObject.FindProperty("gameplayScene");

            if(!string.IsNullOrEmpty(playerPath.stringValue))
            {
                var sceneAux = AssetDatabase.LoadAssetAtPath<SceneAsset>(playerPath.stringValue);
                if (!sceneAux) playerPath.stringValue = "";
            }

            if(!string.IsNullOrEmpty(menuPath.stringValue))
            {
                var sceneAux = AssetDatabase.LoadAssetAtPath<SceneAsset>(menuPath.stringValue);
                if (!sceneAux) menuPath.stringValue = "";
            }

            if(!string.IsNullOrEmpty(gameplayPath.stringValue))
            {
                var sceneAux = AssetDatabase.LoadAssetAtPath<SceneAsset>(gameplayPath.stringValue);
                if (!sceneAux) gameplayPath.stringValue = "";
            }

            canLoadPlayer = true;
            canLoadMenu = true;
            canLoadGame = true;

            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);

                if (scene.path == playerPath.stringValue) canLoadPlayer = false;
                else if (scene.path == menuPath.stringValue) canLoadMenu = false;
                else if (scene.path == gameplayPath.stringValue) canLoadGame = false;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorUtils.Header("Quick Actions");

            canLoadPlayer = canLoadPlayer &&!string.IsNullOrEmpty(playerPath.stringValue);
            canLoadMenu = canLoadMenu && !string.IsNullOrEmpty(menuPath.stringValue);
            canLoadGame = canLoadGame && !string.IsNullOrEmpty(gameplayPath.stringValue);

            GUI.enabled = canLoadPlayer;
            if (GUILayout.Button("Load Player"))
                LoadPlayer();

            GUI.enabled = canLoadMenu;
            if (GUILayout.Button("Load Main Menu"))
            {
                LoadPlayer();
                UnloadScene(gameplayPath, ref canLoadGame);
                EditorSceneManager.OpenScene(menuPath.stringValue, OpenSceneMode.Additive);
                canLoadMenu = false;
            }

            GUI.enabled = canLoadGame;
            if (GUILayout.Button("Load Gameplay"))
            {
                LoadPlayer();
                UnloadScene(menuPath, ref canLoadMenu);
                EditorSceneManager.OpenScene(gameplayPath.stringValue, OpenSceneMode.Additive);
                canLoadGame = false;
            }

            GUI.enabled = true;
        }

        private void LoadPlayer()
        {
            if (!canLoadPlayer) return;

            EditorSceneManager.OpenScene(playerPath.stringValue, OpenSceneMode.Additive);
            canLoadPlayer = false;
        }

        private void UnloadScene(SerializedProperty scenePath, ref bool sceneUnloaded)
        {
            if (sceneUnloaded) return;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                var scene = EditorSceneManager.GetSceneByPath(scenePath.stringValue);
                EditorSceneManager.CloseScene(scene, true);
                sceneUnloaded = true;
            }
        }
    }
}
