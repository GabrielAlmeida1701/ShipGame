using ShipGame.Entities.PlayerEntity;
using ShipGame.Environment.Cannons;
using UnityEngine;

namespace ShipGame.Entities.Enemies
{
    public class Shooter : EnemyShip
    {
        [SerializeField] private LayerMask layers = 1 << 6 | 1 << 7;
        [SerializeField] private float minDist = 2;
        [SerializeField] private float maxDist = 10;

        [SerializeField, HideInInspector] private bool inRange;
        [SerializeField, HideInInspector] private float angle;
        [SerializeField, HideInInspector] private float selectedCannonRange;

        protected override void Start()
        {
            base.Start();
            selectedCannon = null;
        }

        protected override void OnShipCollide(Ship other) {}

        protected override void OnUpdate()
        {
            if (!Player.Instance) return;

            float distance = DistanceFromPlayer();
            float midPoint = (maxDist + minDist) / 2;

            if (distance <= midPoint) inRange = true;
            if (distance < maxDist && inRange)
            {
                Attack();

                angle += Time.deltaTime;
                if (angle >= 360) angle = 0;

                Vector3 lookAt = -DirectionTowardsPlayer * midPoint;
                lookAt -= transform.up * 2;
                lookAt += Player.Instance.transform.position;

                Debug.DrawLine(lookAt - Vector3.right / 2, lookAt + Vector3.right / 2);
                Debug.DrawLine(lookAt - Vector3.up / 2, lookAt + Vector3.up / 2);

                float rotate = Vector3.Cross((lookAt - transform.position).normalized, transform.up).z;
                transform.Rotate(0, 0, rotate * turnSpeed * Time.deltaTime);
                transform.Translate(0, -speed * Time.deltaTime, 0);
            }
            else
            {
                if (selectedCannon)
                    selectedCannon.HideCannon();

                MoveTowardsPlayer();
                inRange = false;
                angle = 0;
            }
        }

        private void Attack()
        {
            if (selectedCannon == null || !selectedCannon.CanShoot) return;
            
            selectedCannon.ShowCannon();

            var cannonTransform = selectedCannon.transform;
            var hit = Physics2D.Raycast(cannonTransform.position, cannonTransform.right, selectedCannonRange, layers);
            if (hit.collider && hit.collider.CompareTag("Ship"))
                selectedCannon.Shoot();
        }

        public void OnPlayerInRange(Cannon cannon, float range)
        {
            if (selectedCannon && selectedCannon != cannon)
                selectedCannon.HideCannon();

            selectedCannon = cannon;
            selectedCannonRange = range;
        }
    }
}