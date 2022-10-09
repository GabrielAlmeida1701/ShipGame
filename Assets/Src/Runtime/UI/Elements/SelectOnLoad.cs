using UnityEngine;
using UnityEngine.EventSystems;

namespace ShipGame.UI.Elements
{
    public class SelectOnLoad : MonoBehaviour
    {
        private void OnEnable()
        {
            if (EventSystem.current)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }

        void Start()
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}