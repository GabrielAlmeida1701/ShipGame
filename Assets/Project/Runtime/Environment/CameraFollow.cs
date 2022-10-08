using UnityEngine;

namespace ShipGame.Environment
{
    [System.Serializable]
    public class CameraShakeSettings
    {
        public float shakeAmount = .2f;
        public float shakeDuration = 1f;
        public float decreaseFactor = 1f;
    }

    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        [SerializeField] private float followSpeed = 2f;

        private float shakeDuration;
        private float shakeAmount;
        private float decreaseFactor;
        private Vector3 originalPos;
        private Vector3 velocity;

        private void LateUpdate()
        {
            SimpleCameraFollow();

            if (shakeDuration > 0)
            {
                transform.position = originalPos + (Vector3) Random.insideUnitCircle * shakeAmount;
                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
        }

        private void SimpleCameraFollow()
        {
            if (!target) return;

            var position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, followSpeed);
            SetPosition(position);
        }

        public void ShakeCamera(CameraShakeSettings shakeSettings)
        {
            originalPos = transform.localPosition;

            shakeDuration = shakeSettings.shakeDuration;
            decreaseFactor = shakeSettings.decreaseFactor;
            shakeAmount = shakeSettings.shakeAmount;
        }

        public void SetPosition(Vector3 position)
        {
            position.z = -10;
            transform.position = position;
        }
    }
}