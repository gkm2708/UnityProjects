using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTracker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnGUI(){
		GUI.Label (new Rect (8, 30, 240, 80), "Ball Position: " + transform.position);
		GUI.Label (new Rect (8, 55, 240, 80), "Ball Velocity: " + transform.GetComponent<Rigidbody>().velocity);
	}
}
