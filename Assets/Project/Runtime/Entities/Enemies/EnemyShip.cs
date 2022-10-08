using ShipGame.Entities.PlayerEntity;
using UnityEngine;

namespace ShipGame.Entities.Enemies
{
    public abstract class EnemyShip : Ship
    {
        protected Vector2 DirectionTowardsPlayer => (Player.Instance.transform.position - transform.position).normalized;

        protected void MoveTowardsPlayer()
        {
            float rotate = Vector3.Cross(DirectionTowardsPlayer, transform.up).z;
            transform.Rotate(0, 0, rotate * turnSpeed * Time.deltaTime);
            transform.Translate(0, -speed * Time.deltaTime, 0);
        }

        protected override void OnDie()
        {
            base.OnDie();

            if (gameplayUI)
                gameplayUI.RemoveEnemey(this);
        }

        protected float DistanceFromPlayer() => Vector3.Distance(transform.position, Player.Instance.transform.position);
    }
}