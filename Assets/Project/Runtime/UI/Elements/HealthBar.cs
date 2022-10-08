using ShipGame.Entities;
using ShipGame.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace ShipGame.UI.Elements
{
    public class HealthBar : MonoBehaviour
    {
        private RectTransform rectTransform => transform as RectTransform;

        [SerializeField, HideInInspector] private Ship ship;
        [SerializeField, HideInInspector] private bool usingLabel;
        [SerializeField, HideInInspector] private Text heartCount;

        public Sprite healthIcon;
        public Font counterFont;
        public int counterFontSize = 25;
        public Vector2 heartSize;
        public Vector2 positionOffset = Vector2.up;

        public bool IsVisible { get; private set; }

        public void SetShip(Ship ship)
        {
            Utils.DestroyChildren(transform);

            this.ship = ship;
            if (ship.Life <= 10) CreateHearts();
            else CreateLabel();
        }

        private void Update()
        {
            if (!ship || !ship.Alive) return;

            var shipPosition = ship.transform.position + (Vector3) positionOffset;
            Vector2 position = Utils.MainCamera.WorldToScreenPoint(shipPosition);

            rectTransform.anchoredPosition = position;
            IsVisible = position.x > 0 && position.x < Screen.width && position.y > 0 && position.y < Screen.height;
        }

        public void UpdateHealthBar()
        {
            if (ship.Life == 0) return;

            if (ship.Life <= 10 && usingLabel)
            {
                Destroy(heartCount.gameObject);
                CreateHearts();
            }
            else if (ship.Life > 10) heartCount.text = ship.Life.ToString();
            else
            {
                var heart = transform.GetChild(ship.Life - 1);
                Destroy(heart.gameObject);
            }
        }

        private void CreateHearts()
        {
            for (int i = 0; i < ship.Life; i++)
            {
                GameObject heart = new GameObject(
                    i.ToString(),
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Image)
                );

                var rectTransform = heart.transform as RectTransform;
                rectTransform.SetParent(transform);
                rectTransform.sizeDelta = heartSize;
                heart.GetComponent<Image>().sprite = healthIcon;
            }
            usingLabel = false;
        }

        private void CreateLabel()
        {
            GameObject label = new GameObject(
                "HeartsCount",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text)
            );

            var rectTransform = label.transform as RectTransform;
            rectTransform.SetParent(transform);

            heartCount = label.GetComponent<Text>();
            heartCount.font = counterFont;
            heartCount.text = ship.Life.ToString();
            heartCount.fontSize = counterFontSize;
            heartCount.alignment = TextAnchor.MiddleCenter;
            heartCount.color = Color.red;

            usingLabel = true;
        }
    }
}