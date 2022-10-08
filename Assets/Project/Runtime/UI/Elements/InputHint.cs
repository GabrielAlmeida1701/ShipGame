using ShipGame.Helpers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShipGame.UI.Elements
{
    [RequireComponent(typeof(Image))]
    public class InputHint : MonoBehaviour
    {
        public List<Sprite> hintInputIcons = new List<Sprite>();
        [SerializeField] private Image image;

        #region Settup
        private void OnEnable()
        {
            SetEvents();
#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.afterAssemblyReload += SetEvents;
#endif
        }

        private void OnDisable()
        {
            ClearEvents();
#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.afterAssemblyReload -= SetEvents;
#endif
        }

        private void SetEvents() => Utils.onChangeInput += SetInputIcon;
        private void ClearEvents() => Utils.onChangeInput -= SetInputIcon;
        #endregion

        private void SetInputIcon() => image.sprite = hintInputIcons[(int)Utils.InputType];
    }
}