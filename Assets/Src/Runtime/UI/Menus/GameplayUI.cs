using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ShipGame.Helpers;
using ShipGame.UI.Elements;
using ShipGame.Entities;
using ShipGame.Entities.PlayerEntity;
using ShipGame.Systems.Input;
using ShipGame.Entities.Enemies;

namespace ShipGame.UI.Menus
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private Text timerText;
        [SerializeField] private ShipSpawnerAreas enemySpawner;
        
        [Header("UI")]
        [SerializeField] private GameOverMenu gameOverMenu;
        [SerializeField] private RectTransform healthBarsParent;
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private Vector2 heartIconSize = new Vector2(20, 20);
        [SerializeField] private Vector2 healthBarOffset = Vector2.up;

        [SerializeField, HideInInspector] private float timer;
        [SerializeField, HideInInspector] private int spawnRate;
        [SerializeField, HideInInspector] private List<Ship> enemies;

        private void Start()
        {
            Utils.DestroyChildren(healthBarsParent);
            Utils.DestroyChildren(enemySpawner.transform);

            int sessionTime = GameManager.GetSeesionTime();
            spawnRate = GameManager.GetSpawnRate();

            timer = sessionTime * 60;
            timerText.text = $"{sessionTime}:00";
            enemies = new List<Ship>();

            if (Player.Instance)
                Player.Instance.gameplayUI = this;

            StartCoroutine(SpawnEnemy());
        }

        private void Update()
        {
#if UNITY_EDITOR
            if(Player.Instance && !Player.Instance.gameplayUI)
                Player.Instance.gameplayUI = this;
#endif
            if (!GameManager.IsPlaying) return;
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                EndGame();
                return;
            }

            int mins = (int) (timer / 60);
            int secs = (int) (timer % 60);
            timerText.text = $"{mins}:{secs:00}";
        }

        private IEnumerator SpawnEnemy()
        {
            while (!GameManager.IsPlaying)
                yield return null;

            if (!enemySpawner.HasEnemy)
                yield break;

            while (GameManager.IsPlaying)
            {
                GameObject healthBarGO = Instantiate(healthBarPrefab, healthBarsParent);
                HealthBar healthBar = healthBarGO.GetComponent<HealthBar>();
                healthBar.positionOffset = healthBarOffset;
                healthBar.heartSize = heartIconSize;

                Ship ship = enemySpawner.SpawEnemy();
                ship.healthBar = healthBar;
                ship.gameplayUI = this;
                enemies.Add(ship);

                yield return new WaitForSeconds(spawnRate);
            }
        }

        public void RepositionEnemies(Vector3 newPlayerPosition)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                try
                {
                    Ship ship = enemies[i];
                    if (!ship)
                    {
                        enemies.RemoveAt(i);
                        i--;
                        continue;
                    }

                    if (!ship.IsVisible) continue;

                    var diff = ship.transform.position - Player.Instance.transform.position;
                    ship.transform.position = newPlayerPosition + diff;
                }
                catch { i--; }
            }
        }

        public void RemoveEnemey(Ship ship) => enemies.Remove(ship);

        public void EndGame()
        {
            StopAllCoroutines();
            InputManager.SwitchToUI();
            gameOverMenu.ShowGameOverScreen();
            Player.Instance.ResetPlayer();
        }
    }
}
