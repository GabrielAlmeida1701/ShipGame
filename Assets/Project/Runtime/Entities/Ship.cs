using System.Collections;
using ShipGame.ShipElements;
using UnityEngine;
using ShipGame.UI.Elements;
using ShipGame.UI.Menus;
using ShipGame.Entities.PlayerEntity;
using ShipGame.Environment.Cannons;

namespace ShipGame.Entities
{
    [RequireComponent(typeof(ShipState))]
    public abstract class Ship : MonoBehaviour
    {
        [SerializeField] protected ShipState shipState;
        [SerializeField] protected int life = 5;
        [SerializeField] protected float speed = 3f;
        [SerializeField] protected float turnSpeed = 60f;

        public float reloadTime = 5;
        [SerializeField] protected GameObject cannonBall;
        [SerializeField] protected Cannon[] cannons;

        [SerializeField] private GameObject hitVFX;
        [HideInInspector] public HealthBar healthBar;
        [HideInInspector] public GameplayUI gameplayUI;

        [SerializeField, HideInInspector] protected Cannon selectedCannon;
        [SerializeField, HideInInspector] private int nextState;
        [SerializeField, HideInInspector] private int damageToNextState;
        [SerializeField, HideInInspector] protected bool keepAfterDeath;

        public bool Alive => life > 0;
        public int Life => life;
        public bool IsVisible
        {
            get
            {
                if (!healthBar) return true;
                return healthBar.IsVisible;
            }
        }

        protected virtual void Start()
        {
            damageToNextState = Mathf.Clamp(Life / 3 - 1, 0, Life);

            if (healthBar) healthBar.SetShip(this);
            if (cannons == null || cannons.Length == 0) return;
            foreach(Cannon c in cannons)
            {
                c.cannonBall = cannonBall;
                c.reloadTime = reloadTime;
            }
            selectedCannon = cannons[0];
        }

        private void Update()
        {
            if (!GameManager.IsPlaying) return;
            if (Alive)
                OnUpdate();
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            var other = coll.gameObject;
            if (other.CompareTag("CannonBall"))
            {
                TakeDamage();
                Destroy(other);

                if (!(this is Player) && !Alive)
                    Player.Instance.points++;
            }
            else if (other.CompareTag("Ship"))
                OnShipCollide(other.GetComponent<Ship>());
        }

        public void TakeDamage() => TakeDamage(1);
        public void TakeDamage(int ammount)
        {
            if (life <= 0) return;

            nextState++;
            life -= ammount;

            if (life <= 0) OnDie();
            else
            {
                if (nextState >= damageToNextState)
                {
                    nextState = 0;
                    shipState.NextState();
                }

                if (healthBar)
                    healthBar.UpdateHealthBar();

                OnTakeDamage();
            }
        }

        protected virtual void OnDie()
        {
            shipState.SetState(99);
            StartCoroutine(DeathFX());

            if (!keepAfterDeath)
            {
                if (healthBar)
                    Destroy(healthBar.gameObject);
                Destroy(gameObject, 5f);
                Destroy(gameObject.GetComponent<Collider2D>());
            }
        }

        private IEnumerator DeathFX()
        {
            int limit = Random.Range(3, 9);
            for (int i = 0; i < limit; i++)
            {
                var position = (Vector2) transform.position + Random.insideUnitCircle;
                var go = Instantiate(hitVFX, position, Quaternion.identity);
                Destroy(go, 1f);

                yield return new WaitForSeconds(.2f);
            }
        }

        protected virtual void OnTakeDamage() { }
        protected abstract void OnUpdate();
        protected abstract void OnShipCollide(Ship other);
    }
}