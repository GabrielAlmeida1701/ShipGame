using ShipGame.Systems.Attributes;
using ShipGame.Systems.Input;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShipGame
{
    public class GameManager : MonoBehaviour
    {
        private const string sessionTimeKey = "SessionTime";
        private const string enemySpawnRateKey = "SpawnRate";

        [SerializeField, SceneReference] private string playerScene;
        [SerializeField, SceneReference] private string mainMenuScene;
        [SerializeField, SceneReference] private string gameplayScene;
        [SerializeField] private bool isPlaying;

        public static bool IsPlaying
        {
            get => Instance.isPlaying;
            set => Instance.isPlaying = value;
        }

        private static GameManager me;
        private static GameManager Instance
        {
            get
            {
                if (!me) me = FindObjectOfType<GameManager>();
                return me;
            }
        }

        private void Awake() => me = this;
        private IEnumerator Start()
        {
            ValidateSave();

#if UNITY_EDITOR
            int count = SceneManager.sceneCount;
            bool inMenu = false;
            bool inGame = false;
            bool hasPlayer = false;

            for (int i = 0; i < count; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.path == mainMenuScene) inMenu = true;
                else if (scene.path == gameplayScene) inGame = true;
                else if (scene.path == playerScene) hasPlayer = true;
            }

            if (!inMenu && !inGame && !hasPlayer) BootGame();
            else
            {
                List<AsyncOperation> operations = new List<AsyncOperation>();

                if (!hasPlayer) operations.Add(SceneManager.LoadSceneAsync(playerScene));
                if (inMenu && inGame)
                {
                    inMenu = false;
                    operations.Add(SceneManager.UnloadSceneAsync(mainMenuScene));
                    InputManager.SwitchToGameplay();
                }
                else if(!inMenu && !inGame)
                {
                    operations.Add(SceneManager.LoadSceneAsync(mainMenuScene));
                    InputManager.SwitchToUI();
                }

                if (inMenu && hasPlayer)
                    InputManager.SwitchToUI();

                foreach (var asop in operations)
                {
                    if (asop == null) continue;
                    while (!asop.isDone)
                        yield return null;
                }
            }
#else
            BootGame();
            yield break;
#endif
        }

        private void BootGame()
        {
            InputManager.SwitchToUI();
            SceneManager.LoadScene(playerScene, LoadSceneMode.Additive);
            SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Additive);
        }

        private IEnumerator LoadScenes(bool isGameplay)
        {
            string toLoad = isGameplay ? gameplayScene : mainMenuScene;
            string toUnload = isGameplay ? mainMenuScene : gameplayScene;

            var unload = SceneManager.UnloadSceneAsync(toUnload);
            while (!unload.isDone)
                yield return null;

            var load = SceneManager.LoadSceneAsync(toLoad, LoadSceneMode.Additive);
            while (!load.isDone)
                yield return null;

            if (isGameplay) InputManager.SwitchToGameplay();
            else InputManager.SwitchToUI();
        }

        private IEnumerator ReloadGameplayScene()
        {
            var unload = SceneManager.UnloadSceneAsync(gameplayScene);
            while (!unload.isDone)
                yield return null;

            var load = SceneManager.LoadSceneAsync(gameplayScene, LoadSceneMode.Additive);
            while (!load.isDone)
                yield return null;

            InputManager.SwitchToGameplay();
        }

        private void ValidateSave()
        {
            if (!PlayerPrefs.HasKey(sessionTimeKey))
                SetSessionTime(1);

            if (!PlayerPrefs.HasKey(enemySpawnRateKey))
                SetSpawnRate(1);
        }

        public static void GoToGameplay() => Instance.StartCoroutine(Instance.LoadScenes(true));
        public static void GoToMainMenu() => Instance.StartCoroutine(Instance.LoadScenes(false));
        public static void ReloadGameplay() => Instance.StartCoroutine(Instance.ReloadGameplayScene());

        public static void SetSessionTime(int time) => PlayerPrefs.SetInt(sessionTimeKey, time);
        public static int GetSeesionTime() => PlayerPrefs.GetInt(sessionTimeKey);

        public static void SetSpawnRate(int rate) => PlayerPrefs.SetInt(enemySpawnRateKey, rate);
        public static int GetSpawnRate() => PlayerPrefs.GetInt(enemySpawnRateKey);
    }
}