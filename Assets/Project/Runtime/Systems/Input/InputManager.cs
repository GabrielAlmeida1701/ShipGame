using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipGame.Systems.Input
{
    public partial class InputManager : MonoBehaviour
    {
        public PlayerInput input;
        public InputActionAsset inputActions;

        private static InputManager _instance;
        public static InputManager Actions
        {
            get
            {
                if (!_instance) _instance = FindObjectOfType<InputManager>();
                return _instance;
            }
        }

        private void Awake() => _instance = this;

        public static void SwitchToUI()
        {
            Actions.SwitchActionMap("ui");
            GameManager.IsPlaying = false;
        }

        public static void SwitchToGameplay()
        {
            Actions.SwitchActionMap("gameplay");
            //GameManager.IsPlaying = true;
        }

        private void SwitchActionMap(string map) => input.currentActionMap = inputActions.FindActionMap(map);
    }
}
