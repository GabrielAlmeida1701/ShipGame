using ShipGame.Systems.Input;
using UnityEngine;

namespace ShipGame.UI.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public GameObject mainMenu;
        public GameObject optionsMenu;

        private void Start()
        {
            mainMenu.SetActive(true);
            optionsMenu.SetActive(false);
            InputManager.onCancel += OnCancel;
        }

        public void GoToGame() => GameManager.GoToGameplay();

        private void OnCancel()
        {
            if (!optionsMenu.activeSelf) return;
            optionsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
}