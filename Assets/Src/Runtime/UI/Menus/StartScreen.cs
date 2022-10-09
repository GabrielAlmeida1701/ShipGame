using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ShipGame.UI.Menu
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private Text countDown;

        private IEnumerator Start()
        {
            GameManager.IsPlaying = false;
            for (int i = 0; i < 3; i++)
            {
                countDown.text = (3 - i).ToString();
                yield return new WaitForSeconds(1);
            }

            countDown.text = "GO!";
            yield return new WaitForSeconds(1);

            GameManager.IsPlaying = true;
            gameObject.SetActive(false);
        }
    }
}
