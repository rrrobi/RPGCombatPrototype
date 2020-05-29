using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterPanel : MonoBehaviour
{
    private float smoothing = 4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3 target)
    {
        StartCoroutine("Movement", target);
    }

    IEnumerator Movement(Vector3 target)
    {
        Vector3 startPos = transform.localPosition;
        while (Vector3.Distance(transform.localPosition, target) > 0.5f)
        {
            float t = smoothing * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, t);

            Debug.Log($"{this.name} has moved from {startPos} to {transform.localPosition}, t = {t}");
            yield return null;
        }
        transform.localPosition = target;
    }
}
