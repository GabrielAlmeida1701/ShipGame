using ShipGame.Systems.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipGame.Entities.PlayerEntity
{
    public class PlayerInputManager : MonoBehaviour
    {
        #region Settup
        private void OnEnable()
        {
            SetInputs();

#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.afterAssemblyReload += SetInputs;
#endif
        }

        private void OnDisable()
        {
            ClearInputs();

#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.afterAssemblyReload -= SetInputs;
#endif
        }
        #endregion

        private void SetInputs()
        {
            InputManager.onMove += OnMove;
            InputManager.onPreviusCannon += OnPreviusCannon;
            InputManager.onNextCannon += OnNextCannon;
            InputManager.onShoot += OnShoot;
        }

        private void ClearInputs()
        {
            InputManager.onMove -= OnMove;
            InputManager.onPreviusCannon -= OnPreviusCannon;
            InputManager.onNextCannon -= OnNextCannon;
            InputManager.onShoot -= OnShoot;
        }

        public void OnMove(InputValue value)
        {
            Vector2 direction = value.Get<Vector2>();

            Player.Instance.SetMoveForward(direction.y > 0);
            Player.Instance.SetTurn(direction.x);
        }

        public void OnPreviusCannon(InputValue value) => Player.Instance.SwitchCannon(-1);
        public void OnNextCannon(InputValue value) => Player.Instance.SwitchCannon(1);
        public void OnShoot(InputValue _) => Player.Instance.Shoot();
    }
}