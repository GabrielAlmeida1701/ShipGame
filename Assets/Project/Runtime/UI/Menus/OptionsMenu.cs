using UnityEngine;
using UnityEngine.UI;

namespace ShipGame.UI.Menu
{
    public class OptionsMenu : MonoBehaviour
    {
        //For this, the best option would be to use TextMeshProUGUI
        //but I decided not so I could keep all scripts made by me,
        //since technically TextMeshPro does add new scripts into
        //the project(both in the package and Assets folders)
        [Header("Session Time")]
        [SerializeField] private Slider sessionTimeSlider;
        [SerializeField] private Text sessionTimeValue;
        [SerializeField] private string sessionTimeAddedText = "Minute(s)";

        [Header("Enemy Spawn Time")]
        [SerializeField] private Text enemySpawnTimeValue;
        [SerializeField] private Text enemySpawnTimeLegend;
        [SerializeField] private string enemySpawnAddedText = "Spawn new Enemies each";
        [SerializeField] private Button enemySpawnAddBnt;
        [SerializeField] private Button enemySpawnSubtractBnt;
        [SerializeField, HideInInspector] private int spawRate;

        private void Start()
        {
            int sessionTime = GameManager.GetSeesionTime();
            spawRate = GameManager.GetSpawnRate();

            sessionTimeSlider.value = sessionTime;
            UpdateSessionTimeText(sessionTime);

            UpdateSpawnRateText();
            UpdateSpawnRateLegend();
            ValidateSpawnRateButtons();
        }

        public void OnSessionTimeSliderChange(float value)
        {
            int sessionTime = (int)value;
            UpdateSessionTimeText(sessionTime);
            GameManager.SetSessionTime(sessionTime);
        }

        public void OnEnemySpawnRateChange(int value)
        {
            spawRate += value;

            enemySpawnSubtractBnt.interactable = spawRate > 1;
            enemySpawnAddBnt.interactable = spawRate > 30;

            UpdateSpawnRateText();
            UpdateSpawnRateLegend();
            ValidateSpawnRateButtons();
            GameManager.SetSpawnRate(spawRate);
        }

        private void UpdateSessionTimeText(int value) => sessionTimeValue.text = $"{value} {sessionTimeAddedText}";
        private void UpdateSpawnRateText() => enemySpawnTimeValue.text = spawRate.ToString();
        private void UpdateSpawnRateLegend() => enemySpawnTimeLegend.text = $"{enemySpawnAddedText} {spawRate} Seconds";
        private void ValidateSpawnRateButtons()
        {
            enemySpawnSubtractBnt.interactable = spawRate > 1;
            enemySpawnAddBnt.interactable = spawRate < 30;
        }
    }
}