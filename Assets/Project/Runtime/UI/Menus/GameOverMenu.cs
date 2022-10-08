using ShipGame.Entities.PlayerEntity;
using UnityEngine;
using UnityEngine.UI;

namespace ShipGame.UI.Menus
{
    public class GameOverMenu : MonoBehaviour
    {
        [SerializeField] private Image bg;
        [SerializeField] private Text title;
        [SerializeField] private Text points;
        [SerializeField] private GameObject content;
        [SerializeField] private float fadeInDuration = 5;

        [SerializeField, HideInInspector] private bool hasStarted;

        private float timer;

        private void Start()
        {
            if (hasStarted) return;

            content.SetActive(false);
            gameObject.SetActive(false);
            hasStarted = true;
        }

        public void ShowGameOverScreen()
        {
            if (!hasStarted) Start();
            gameObject.SetActive(true);

            title.text = Player.Instance.Life > 0 ? "You Survived" : "You Died";
            points.text = Player.Instance.points.ToString();
        }

        private void Update()
        {
            if (timer < fadeInDuration)
            {
                timer += Time.deltaTime;
                bg.color = Color.Lerp(Color.clear, Color.black, timer / fadeInDuration);
            }
            else content.SetActive(true);
        }

        public void PlayAgain() => GameManager.ReloadGameplay();
        public void GoToMainMenu() => GameManager.GoToMainMenu();
    }
}