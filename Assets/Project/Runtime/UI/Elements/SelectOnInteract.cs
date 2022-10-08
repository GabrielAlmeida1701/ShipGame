using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShipGame.UI.Elements
{
    [RequireComponent(typeof(Button))]
    public class SelectOnInteract : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnEnable()
        {
            UnityEditor.AssemblyReloadEvents.afterAssemblyReload += Start;
        }

        private void OnDisable()
        {
            UnityEditor.AssemblyReloadEvents.afterAssemblyReload -= Start;
        }
#endif

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => StartCoroutine(SelectMe()));
        }

        private IEnumerator SelectMe()
        {
            yield return new WaitForSeconds(.01f);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}