﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTracker : MonoBehaviour
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
		GUI.Label (new Rect (8, 5, 240, 80), "Goal Position: " + transform.position);
	}
}
