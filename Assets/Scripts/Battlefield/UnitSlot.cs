using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSlot : MonoBehaviour {

    bool IsOccupied = false;
    public void SetIsOccupied(bool occupied) { IsOccupied = occupied; }
    public bool GetIsOccupied() { return IsOccupied; }

    GameObject occupyingCharacter;
    public void SetOccupyingCharacter(GameObject character) { occupyingCharacter = character; }
    public GameObject GetOccupyingCharacter() { return occupyingCharacter; }

    // future requirements
    // collider - to make the slot itself click-able on need
    // sprite - to flash, to display to the user the slot is click-able

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
