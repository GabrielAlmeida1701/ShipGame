using ShipGame.Entities.PlayerEntity;
using ShipGame.Environment.Cannons;
using ShipGame.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace ShipGame.UI.Menu
{
    [System.Serializable]
    public struct CannonUI
    {
        public Cannon cannon;
        public Image reload;
        public Vector3 distance;
        [HideInInspector] public float reloadTimer;

        public Transform transform => cannon.transform;
        public RectTransform rectTransform => reload.transform as RectTransform;
    }

    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private GameObject content;
        [SerializeField] private CannonUI[] cannons;

        private void Update()
        {
            content.SetActive(GameManager.IsPlaying);
            if (!Player.Instance) return;

            for (int i = 0; i < cannons.Length; i++)
            {
                var ui = cannons[i];
                ui.reload.enabled = !ui.cannon.CanShoot;
                if (!ui.cannon.CanShoot) UpdateIconPosition(ref ui);
                else ui.reloadTimer = 0;

                cannons[i] = ui;
            }
        }

        private void UpdateIconPosition(ref CannonUI ui)
        {
            float angle = Player.Instance.transform.eulerAngles.z;
            Vector3 cannonPosition = ui.cannon.CannonPosition + ui.distance;
            Vector3 finalPos = Quaternion.AngleAxis(angle, Vector3.forward) * cannonPosition;
            finalPos += Player.Instance.transform.position;

            var position = Utils.MainCamera.WorldToScreenPoint(finalPos);

            ui.rectTransform.anchoredPosition = position;
            ui.reloadTimer += Time.deltaTime / ui.cannon.reloadTime;
            ui.reload.fillAmount = Mathf.Lerp(0, 1, ui.reloadTimer);
        }
    }
}
