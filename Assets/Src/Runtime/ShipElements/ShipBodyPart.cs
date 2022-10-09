using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.ShipElements
{
    public class ShipBodyPart : MonoBehaviour
    {
        [SerializeField, Range(0, 2)] protected int state;
        [SerializeField] private List<Sprite> statesSprites;

        [SerializeField, HideInInspector] private SpriteRenderer _renderer;
        protected SpriteRenderer Renderer
        {
            get
            {
                if (!_renderer) _renderer = GetComponent<SpriteRenderer>();
                return _renderer;
            }
        }

        private void OnValidate() => UpdateShip();

        public void SetState(int state)
        {
            this.state = state;
            UpdateShip();
        }

        public virtual void UpdateShip()
        {
            if (statesSprites == null || state >= statesSprites.Count)
                state = statesSprites.Count - 1;
            Renderer.sprite = statesSprites[state];
        }
    }
}