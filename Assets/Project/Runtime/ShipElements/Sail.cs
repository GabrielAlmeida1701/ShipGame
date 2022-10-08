using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.ShipElements
{
    public class Sail : ShipBodyPart
    {
        [SerializeField] private Vector3 closedSailSize = Vector3.one * 1.25f;
        [SerializeField] private Vector3 openSailSize = Vector3.one * .9f;

        [SerializeField] private Vector3 closedSailPosition = new Vector3(0, .35f, 0);
        [SerializeField] private Vector3 openSailPosition = new Vector3(0, .03f, 0);

        [SerializeField] private List<Sprite> offStatesSprites;
        [SerializeField, HideInInspector] private bool isOpen = true;

        public void ToggleSails(bool open)
        {
            if (offStatesSprites == null || offStatesSprites.Count == 0) return;

            isOpen = open;
            transform.localScale = open ? openSailSize : closedSailSize;
            transform.localPosition = open ? openSailPosition : closedSailPosition;
            UpdateShip();
        }

        public override void UpdateShip()
        {
            if(isOpen) base.UpdateShip();
            else if (offStatesSprites != null && state < offStatesSprites.Count)
                Renderer.sprite = offStatesSprites[state];
        }
    }
}