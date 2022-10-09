using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
using UnityEngine;
using UnityEditor.Build.Reporting;

namespace ShipGameEditor
{
    [InitializeOnLoad]
    public class EditorScenesValidator : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private static bool ignoreLoadingWarning;
        private static bool isProjectLoading;

        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report) => isProjectLoading = true;

        public void OnPostprocessBuild(BuildReport report) => isProjectLoading = false;

        static EditorScenesValidator() => EditorSceneManager.sceneOpened += ValidateOpendScene;

        private static void ValidateOpendScene(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (isProjectLoading || ignoreLoadingWarning || scene.path.ToLower().Contains("root"))
            {
                ignoreLoadingWarning = false;
                return;
            }

            MainScenes mainScenes = MainScenes.HasMainScenes();
            if (!mainScenes.hasRoot)
                MissingRootWarning(scene.path, mainScenes);
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instaceID, int _)
        {
            if (isProjectLoading) return false;

            Object instance = EditorUtility.InstanceIDToObject(instaceID);

            if (instance.GetType() == typeof(SceneAsset))
            {
                var path = AssetDatabase.GetAssetPath(instance);
                if (!path.ToLower().Contains("root"))
                {
                    MainScenes mainScenes = MainScenes.HasMainScenes();
                    if (!mainScenes.hasRoot)
                        return MissingRootWarning(path, mainScenes);
                }
            }

            return false;
        }

        private static bool MissingRootWarning(string targetScene, MainScenes mainScenes)
        {
            string name = targetScene.Substring(targetScene.LastIndexOf("/") + 1).Replace(".unity", "");
            bool loadRoot = EditorUtility.DisplayDialog(
                            "Missing _Root Scene",
                            "Attention you are loading a scene without the _Root scene loaded " +
                            "or you are unloading the _Root scene, " +
                            "the game may not respond properly.\n" +
                            "There are shortcuts  for each scene in the Object _Root/GameManager under " +
                            "\"Quick Actions\" in the Inspector\n" +
                            "Do you wish to load the _Root Scene Additively?",
                            "Yes, Load _Root",
                            $"No, Only load {name} "
                        );

            if (loadRoot)
            {
                LoadRootScene(targetScene, mainScenes);
                EditorSceneManager.OpenScene(targetScene, OpenSceneMode.Additive);
                return true;
            }

            ignoreLoadingWarning = true;
            return false;
        }

        private static void LoadRootScene(string scenePath, MainScenes mainScenes)
        {
            EditorSceneManager.OpenScene("Assets/Scenes/_Root.unity");
            if (!scenePath.ToLower().Contains("player"))
            {
                if (!mainScenes.hasPlayer)
                    EditorSceneManager.OpenScene("Assets/Scenes/Player.unity", OpenSceneMode.Additive);
            }
        }

        private struct MainScenes
        {
            public bool hasRoot;
            public bool hasPlayer;

            public static MainScenes HasMainScenes()
            {
                MainScenes main = new MainScenes();
                for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                {
                    string path = EditorSceneManager.GetSceneAt(i).path.ToLower();
                    if (path.Contains("root")) main.hasRoot = true;
                    else if (path.Contains("player")) main.hasPlayer = true;
                }

                return main;
            }
        }
    }
}
