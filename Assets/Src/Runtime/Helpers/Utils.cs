using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace ShipGame.Helpers
{
    public enum InputDeviceType
    {
        Keyboard,
        Xbox,
        PlayStation
    }

    public class Utils
    {
        public static bool _isJoystick;
        public static bool IsJoystick
        {
            get => _isJoystick;
            set
            {
                bool changed = _isJoystick != value;
                _isJoystick = value;

                if (changed)
                    onChangeInput?.Invoke();
            }
        }

        public static InputDeviceType InputType { get; private set; }

        public static Action onChangeInput;

        private static Camera _main;
        public static Camera MainCamera
        {
            get
            {
                if (!_main) _main = Camera.main;
                return _main;
            }
        }


        public static void CheckInputDevice(PlayerInput input)
        {
            SetInputDeviceType(input.devices);
            IsJoystick = input.currentControlScheme == "gamepad";
        }

        public static void SetInputDeviceType(ReadOnlyArray<InputDevice> devices)
        {
            if (devices.Count == 0) return;

            InputDevice device = null;
            foreach (var id in devices)
            {
                if (id.enabled)
                {
                    device = id;
                    break;
                }
            }
            if (device == null) return;

            string displayName = device.displayName.ToLower();
            string deviceName = device.name.ToLower();

            if (displayName.Contains("xbox") || deviceName.Contains("xinput"))
                InputType = InputDeviceType.Xbox;
            else if (displayName.Contains("ps") || deviceName.Contains("dualshock"))
                InputType = InputDeviceType.PlayStation;
            else
                InputType = InputDeviceType.Keyboard;
        }

        public static T GetRandom<T>(List<T> list)
        {
            if (list.Count == 0) return default;
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static void DestroyChildren(Transform transform)
        {
            foreach (Transform child in transform)
                UnityEngine.Object.Destroy(child.gameObject);
        }
    }
}
