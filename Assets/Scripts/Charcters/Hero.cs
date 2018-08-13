using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Character {

	// Use this for initialization
	protected override void Start () {
        Debug.Log("Hero, start method for: " + team.ToString() + "_" + this.name);
        base.Start();
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}
}
