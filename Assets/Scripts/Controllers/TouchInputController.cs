using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInputController : MonoBehaviour {

    //testing an idea
    enum inputState
    {
        Nuetral,
        EnemySelected,
        FriendlySelected
    };

    public GameObject SelectCursor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        // currently uyses mouse input
        // TODO...
        // Adjust to use touch input for android phones
        if (Input.GetMouseButtonDown(0))
            OnTouch();
		
	}

    void OnTouch()
    {
        Debug.Log("Mouse Clicked!");
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit)
        {
            Instantiate(SelectCursor, hit.transform.position, Quaternion.identity);
            Debug.Log("Clicked on: " + hit.transform.name);
        }

    }
}
