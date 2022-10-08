using UnityEngine;

namespace ShipGame.Environment.Cannons
{
    public class CannonBall : MonoBehaviour
    {
        public float speed = 15f;
        private void Update() => transform.Translate(0, speed * Time.deltaTime, 0);
    }
}