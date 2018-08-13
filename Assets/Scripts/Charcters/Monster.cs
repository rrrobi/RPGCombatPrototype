using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using UnityEngine.UI;

public class Monster : Character {

    void Awake()
    {
    }

	// Use this for initialization
	protected override void Start () {
        Debug.Log("Monster, start method for: " + team.ToString() + "_" + this.name);
        base.Start();
        
    }

    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();

    }
    
}
