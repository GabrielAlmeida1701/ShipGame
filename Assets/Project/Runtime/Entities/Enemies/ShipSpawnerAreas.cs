using System.Collections.Generic;
using ShipGame.Helpers;
using UnityEngine;

namespace ShipGame.Entities.Enemies
{
    public class ShipSpawnerAreas : MonoBehaviour
    {
        [SerializeField] private List<GameObject> avaliableEnemies;
        [HideInInspector] public List<float> spawnChance;
        public List<Bounds> avaliableAreas;

        [SerializeField, HideInInspector]
        private List<float> orderedSpawnChance;

        private void Start()
        {
            orderedSpawnChance = new List<float>(spawnChance);
            orderedSpawnChance.Sort();
            orderedSpawnChance.Reverse();
        }

        public bool HasEnemy => avaliableEnemies != null && avaliableEnemies.Count > 0;

        public Ship SpawEnemy()
        {
            GameObject enemy = null;
            float value = Random.value;
            float percent = 0;
            foreach(float chance in orderedSpawnChance)
            {
                percent += chance;
                if (percent < value) continue;
                
                int chanceId = spawnChance.IndexOf(chance);
                enemy = avaliableEnemies[chanceId];
                break;
            }

            Vector3 spawnPosition = GetRandomPoint();
            GameObject shipGO = Instantiate(enemy, spawnPosition, Quaternion.identity);
            shipGO.transform.parent = transform;
            return shipGO.GetComponent<Ship>();
        }

        private Vector3 GetRandomPoint()
        {
            Bounds bounds = Utils.GetRandom(avaliableAreas);
            return new Vector3()
            {
                x = bounds.center.x + Random.Range(-bounds.size.x, bounds.size.x),
                y = bounds.center.y + Random.Range(-bounds.size.y, bounds.size.y),
                z = 0
            };
        }
    }
}