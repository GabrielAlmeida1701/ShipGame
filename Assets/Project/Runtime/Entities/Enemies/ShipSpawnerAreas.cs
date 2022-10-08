using ShipGame.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Entities.Enemies
{
    public class ShipSpawnerAreas : MonoBehaviour
    {
        [SerializeField] private List<GameObject> avaliableEnemies;
        public List<Bounds> avaliableAreas;

        public bool HasEnemy => avaliableEnemies != null && avaliableEnemies.Count > 0;

        public Ship SpawEnemy()
        {
            GameObject enemy = Utils.GetRandom(avaliableEnemies);
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