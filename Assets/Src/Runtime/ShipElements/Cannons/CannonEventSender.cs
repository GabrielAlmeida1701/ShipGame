using ShipGame.Entities.PlayerEntity;
using UnityEngine;
using UnityEngine.Events;

namespace ShipGame.Environment.Cannons
{
    [RequireComponent(typeof(Cannon))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class CannonEventSender : MonoBehaviour
    {
        public UnityEvent<Cannon, float> onEnterRange;

        [SerializeField, HideInInspector] private CapsuleCollider2D capsule;
        [SerializeField, HideInInspector] private Cannon cannon;

        private void Start()
        {
            capsule = GetComponent<CapsuleCollider2D>();
            cannon = GetComponent<Cannon>();
        }

        public bool CanShoot => cannon.CanShoot;
        public float Range => capsule.size.x;

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.TryGetComponent(out Player _))
                onEnterRange?.Invoke(cannon, Range);
        }
    }
}