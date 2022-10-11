using ShipGame.Entities.PlayerEntity;
using UnityEngine;

namespace ShipGame.Entities.Enemies
{
    public abstract class EnemyShip : Ship
    {
        protected Vector2 DirectionTowardsPlayer => (Player.Instance.transform.position - transform.position).normalized;

        private int viewOffset;
        private Vector3 tempTarget;

        [SerializeField] private LayerMask groundLayer = 1 << 7;
        [SerializeField] private int viewDistance = 5;

        protected float DistanceFromPlayer() => Vector3.Distance(transform.position, Player.Instance.transform.position);

        protected override void OnDie()
        {
            base.OnDie();

            if (gameplayUI)
                gameplayUI.RemoveEnemey(this);
        }

        protected void MoveTowardsPlayer()
        {
            if (tempTarget == Vector3.zero)
            {
                var hit = Physics2D.Raycast(transform.position, -transform.up, viewDistance, groundLayer);

                if (!hit.collider) FollowPlayer();
                else CalculateAvoidancePath();
            }
            else MoveTowardsTarget();
        }

        #region Moviment
        protected void MoveTowardsTarget()
        {
            float distance = Vector2.Distance(transform.position, tempTarget);

            if (distance <= 1.9f) tempTarget = Vector3.zero;
            else MoveTowards(tempTarget - transform.position);
        }

        private void FollowPlayer()
        {
            viewOffset = 2;
            tempTarget = Vector3.zero;

            MoveTowards(DirectionTowardsPlayer);
        }

        private void MoveTowards(Vector3 direction)
        {
            float rotate = Vector3.Cross(direction, transform.up).z;
            transform.Rotate(0, 0, rotate * turnSpeed * Time.deltaTime);
            transform.Translate(0, -speed * Time.deltaTime, 0);
        }
        #endregion

        #region Obstacle Avoidance
        private void CalculateAvoidancePath()
        {
            float distance = viewDistance * viewOffset;
            Vector3 rightDirection = CalculateSightDirection(transform.right);
            Vector3 leftDirection = CalculateSightDirection(-transform.right);

            bool rightHit;
            bool leftHit;
            do
            {
                rightHit = Physics2D.Raycast(transform.position, rightDirection, distance, groundLayer).collider;
                leftHit = Physics2D.Raycast(transform.position, leftDirection, distance, groundLayer).collider;

                if (rightHit && leftHit)
                {
                    viewOffset += 1;
                    rightDirection = CalculateSightDirection(transform.right);
                    leftDirection = CalculateSightDirection(-transform.right);
                }
            } while (rightHit && leftHit);

            if (!rightHit) SetTargetPoint(rightDirection * distance);
            else SetTargetPoint(leftDirection * distance);
        }

        private Vector3 CalculateSightDirection(Vector3 side)
        {
            Vector3 point = transform.position + side * viewOffset;
            Vector3 pointEnd = point - transform.up * viewDistance;
            return (pointEnd - transform.position).normalized;
        }
        
        private void SetTargetPoint(Vector3 end) => tempTarget = transform.position + end / 2;
        #endregion
    }
}