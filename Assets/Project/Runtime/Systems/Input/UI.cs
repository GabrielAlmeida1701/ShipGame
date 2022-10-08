using ShipGame.Helpers;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using ShipGame.UI.Elements;
using System;

namespace ShipGame.Systems.Input
{
    public partial class InputManager
    {
        public static Action onCancel;

        private void OnNavigate(InputValue _)
        {
            Utils.CheckInputDevice(input);
            if (!EventSystem.current.currentSelectedGameObject)
            {
                var selectGameObject = FindObjectOfType<SelectOnLoad>();
                if(selectGameObject)
                    EventSystem.current.SetSelectedGameObject(selectGameObject.gameObject);
            }
        }

        private void OnPoint(InputValue _) => Utils.CheckInputDevice(input);

        private void OnCancel(InputValue _)
        {
            Utils.CheckInputDevice(input);
            onCancel?.Invoke();
        }
    }
}
