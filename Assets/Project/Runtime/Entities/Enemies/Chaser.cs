using ShipGame.Entities.PlayerEntity;

namespace ShipGame.Entities.Enemies
{
    public class Chaser : EnemyShip
    {
        protected override void OnShipCollide(Ship other)
        {
            TakeDamage(Life);
            Destroy(gameObject, 1f);
        }

        protected override void OnUpdate()
        {
            if (!Player.Instance) return;
            MoveTowardsPlayer();
        }
    }
}