using UnityEngine;

namespace ShipGame.ShipElements
{
    public class ShipState : MonoBehaviour
    {
        [SerializeField, Range(0, 3)] private int state;
        [SerializeField, HideInInspector]
        private ShipBodyPart[] bodyParts;

        private void Start()
        {
            bodyParts = GetComponentsInChildren<ShipBodyPart>();
            UpdateSprite();
        }

        public void NextState() => SetState(state + 1);

        public void SetState(int state)
        {
            this.state = state;
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            foreach (var part in bodyParts)
                part.SetState(state);
        }
    }
}