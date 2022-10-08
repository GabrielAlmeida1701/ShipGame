using ShipGame.Environment;
using ShipGame.ShipElements;
using System.Collections;
using UnityEngine;

namespace ShipGame.Entities.PlayerEntity
{
    public class Player : Ship
    {
        #region Variables
        private static Player me;
        public static Player Instance
        {
            get
            {
                if (!me) me = FindObjectOfType<Player>();
                return me;
            }
        }

        [SerializeField] private Sail sail;
        [SerializeField] private CameraFollow cameraFollow;
        [SerializeField] private float movementDrag = .5f;
        [SerializeField] private CameraShakeSettings shakeOnHit;
        [SerializeField] private CameraShakeSettings shakeOnDead;

        [SerializeField] private int maxLife;
        [SerializeField] private Quaternion startRotation;

        [SerializeField] private int cannon;
        [SerializeField] private Vector2 playbleArea = new Vector2(15, 15);

        public int points;

        private float timer;
        private float turn = 0;
        private bool moveForward;
        private bool lastForward;
        #endregion

        #region Movement
        protected override void Start()
        {
            base.Start();

            keepAfterDeath = true;
            startRotation = transform.rotation;
            maxLife = Life;
        }

        protected override void OnUpdate()
        {
            MoveShip();
            FixPlayerPosition();

            selectedCannon.ShowCannon();
            transform.Rotate(0, 0, -turn * Time.deltaTime);
        }

        private void MoveShip()
        {
            timer += Time.deltaTime;
            float startSpeed = moveForward? 0 : -speed;
            float endSpeed = moveForward ? -speed : 0;
            float targetSpeed = Mathf.Lerp(startSpeed, endSpeed, timer * movementDrag);

            Vector3 movement = new Vector3(0, targetSpeed * Time.deltaTime, 0);
            transform.Translate(movement);

            sail.ToggleSails(moveForward || targetSpeed != 0);
        }

        private void FixPlayerPosition()
        {
            Vector3 position = transform.position;
            void UpdatePosition(ref float target, float limit, bool higher)
            {
                limit /= 2;
                if((higher && target > limit) || (!higher && target < limit))
                {
                    target = -limit;

                    var diff = cameraFollow.transform.position - transform.position;
                    cameraFollow.SetPosition(position + diff);
                    gameplayUI.RepositionEnemies(position);
                }
            }
            
            UpdatePosition(ref position.y, playbleArea.y, true);
            UpdatePosition(ref position.y, -playbleArea.y, false);

            UpdatePosition(ref position.x, playbleArea.x, true);
            UpdatePosition(ref position.x, -playbleArea.x, false);

            transform.position = position;
        }
        #endregion

        #region Implementations
        protected override void OnDie()
        {
            base.OnDie();
            cameraFollow.ShakeCamera(shakeOnDead);
            gameplayUI.EndGame();
        }

        protected override void OnTakeDamage() => cameraFollow.ShakeCamera(shakeOnHit);

        protected override void OnShipCollide(Ship _) => TakeDamage();

        public void ResetPlayer() => StartCoroutine(ResetPosition());
        private IEnumerator ResetPosition()
        {
            yield return new WaitForSeconds(4.5f);
            transform.position = Vector3.zero;
            transform.rotation = startRotation;
            cameraFollow.SetPosition(Vector3.zero);

            points = 0;
            life = maxLife;
            healthBar.SetShip(this);
            shipState.SetState(0);
        }
        #endregion

        #region Inputs
        public void SetTurn(float turn) => this.turn = turn * turnSpeed;

        public void SetMoveForward(bool moveForward)
        {
            if (lastForward == moveForward) return;

            this.moveForward = moveForward;
            lastForward = moveForward;
            timer = 0;
        }

        public void Shoot() => selectedCannon.Shoot();

        public void SwitchCannon(int direction)
        {
            selectedCannon.HideCannon();

            cannon += direction;
            if (cannon >= cannons.Length) cannon = 0;
            if (cannon < 0) cannon = cannons.Length - 1;
            selectedCannon = cannons[cannon];
        }
        #endregion
    }
}