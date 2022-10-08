using ShipGame.Helpers;
using System;
using UnityEngine.InputSystem;

namespace ShipGame.Systems.Input
{
    public partial class InputManager
    {
        public static Action<InputValue> onMove;
        public static Action<InputValue> onPreviusCannon;
        public static Action<InputValue> onNextCannon;
        public static Action<InputValue> onShoot;

        private void OnMove(InputValue value)
        {
            Utils.CheckInputDevice(input);
            onMove?.Invoke(value);
        }

        private void OnPreviousCannon(InputValue value)
        {
            Utils.CheckInputDevice(input);
            onPreviusCannon?.Invoke(value);
        }

        private void OnNextCannon(InputValue value)
        {
            Utils.CheckInputDevice(input);
            onNextCannon?.Invoke(value);
        }

        private void OnShoot(InputValue value)
        {
            Utils.CheckInputDevice(input);
            onShoot?.Invoke(value);
        }
    }
}
