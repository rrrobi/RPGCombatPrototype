using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class UnitSlot : MonoBehaviour
    {

        bool IsOccupied = false;
        public void SetIsOccupied(bool occupied) { IsOccupied = occupied; }
        public bool GetIsOccupied() { return IsOccupied; }

        GameObject occupyingCharacter;
        public void SetOccupyingCharacter(GameObject character) { occupyingCharacter = character; }
        public GameObject GetOccupyingCharacter() { return occupyingCharacter; }

        // future requirements
        // collider - to make the slot itself click-able on need
        // sprite - to flash, to display to the user the slot is click-able
        Sprite unitSlotSprite;

        // Use this for initialization
        void Start()
        {
            unitSlotSprite = Resources.Load<Sprite>("BattleResources/Sprites/SimpleUnitSlot_v2");
            SpriteRenderer unitSlotSR = this.gameObject.AddComponent<SpriteRenderer>();
            // Set Correct Monster sprite
            unitSlotSR.sprite = unitSlotSprite;
            unitSlotSR.sortingLayerName = "UnitSlots";
            // Set monster's colider, (Make it clickable)
            CircleCollider2D col = this.gameObject.AddComponent<CircleCollider2D>();
            col.radius = 1.0f;
            // Set unclickable to start
            MakeUnclickable();

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void MakeGlowingClickable()
        {
            this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            // Set Sprite Shader to Glow
            this.gameObject.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Custom/Sprite Glow"));
        }

        public void MakeClickable()
        {
            this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            // Set Sprite Shader to Highlight
            this.gameObject.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Custom/Sprite Outline"));
        }

        public void MakeUnclickable()
        {
            this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            // Reset Sprite Shader to Dprite Default (not Glowing)
            this.gameObject.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        }
    }
}