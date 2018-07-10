using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleUIController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CharcterSelectbuttonPressed()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }

    public void ActionSelectbuttonPressed()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }

    public void EnemySelectbuttonPressed()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }
}
