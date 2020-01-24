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
		float graph_x = (transform.position.x + 33);
		float graph_z = (33 - transform.position.z);

		GUI.Label (new Rect (8, 30, 240, 80), "Ball Position: " + graph_x + "," + graph_z);
		GUI.Label (new Rect (8, 55, 240, 80), "Ball Velocity: " + transform.GetComponent<Rigidbody>().velocity);
	}
}
