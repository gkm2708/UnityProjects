using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class MazeBaseAgent : Agent
{
    // Start is called before the first frame update

	public GameObject Ball;
	public Transform Target;

	private colider script; // this will be the container of the script

	public Vector3 Ball1;
	public Vector3 Ball2;

	int[,] array2D = new int[9,9] { 
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
		{ 1, 1, 0, 0, 0, 0, 0, 1, 1 }, 
		{ 1, 1, 0, 0, 0, 0, 0, 1, 1 }, 
		{ 1, 1, 0, 0, 0, 0, 0, 1, 1 }, 
		{ 1, 1, 0, 0, 0, 0, 0, 1, 1 }, 
		{ 1, 1, 0, 0, 0, 0, 0, 1, 1 }, 
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0 } };

	void Start()
    {
		print("test");

		// make a list of possible positions
		string gap = ", ";
		string gap1 = "        ";

		for (int i = 0; i < this.transform.childCount; i++) {
			if (this.transform.GetChild (i).gameObject.tag == "wall") {
				if (this.transform.GetChild (i).gameObject.transform.position.x*2 <= 4 
					&& this.transform.GetChild (i).gameObject.transform.position.x*2 >= -4
					&& this.transform.GetChild (i).gameObject.transform.position.z*2 <= 4
					&& this.transform.GetChild (i).gameObject.transform.position.z*2 >= -4){

					array2D[(int)(this.transform.GetChild(i).gameObject.transform.position.x*2+4), (int)(this.transform.GetChild(i).gameObject.transform.position.z*2+4)] = 1;
				}
			}
		}
    }

	public override void AgentReset(){

		// reset the board position
		this.transform.position = new Vector3 (0, 0, 0);
		this.transform.rotation = new Quaternion (0, 0, 0, 1);
		int x;
		int y;
		// set a random position for Target
		x = UnityEngine.Random.Range (0, 8);
		y = UnityEngine.Random.Range (0, 8);
		while(array2D[x, y] != 0){
			x = UnityEngine.Random.Range (0, 8);
			y = UnityEngine.Random.Range (0, 8);
		}
		Target.position = new Vector3 ((x-4.0f)*0.5f, 0.05f, (y-4.0f)*0.5f);

		//Target.position = new Vector3 (0.0f, 0.05f, 0.0f);

		// set a random position for Ball Agent For restricted maze
		/*
		y = UnityEngine.Random.Range (0, 8);
		if (y == 0 || y == 8) {
			x = UnityEngine.Random.Range (0, 8);
		} else 	if (y == 1 || y == 7) {
			x = UnityEngine.Random.Range (0, 2);
			if (x == 0) {
				x = 0;
			} else if (x == 1) {
				x = 4;
			} else {
				x = 8;
			}
		} else {
			x = 4;
		}*/
		x = UnityEngine.Random.Range (0, 8);
		y = UnityEngine.Random.Range (0, 8);
		while(array2D[x, y] != 0){
			x = UnityEngine.Random.Range (0, 8);
			y = UnityEngine.Random.Range (0, 8);
		}
		Ball.transform.position = new Vector3 ((x-4.0f)*0.5f, 0.05f, (y-4.0f)*0.5f);
		//Ball.transform.position = new Vector3 (0.0f, 0.05f, 0.0f);
	}


	public override void CollectObservations(){

		AddVectorObs (gameObject.transform.rotation.x);
		AddVectorObs (gameObject.transform.rotation.z);

		AddVectorObs (Ball.transform.position.x);
		AddVectorObs (Ball.transform.position.z);

		AddVectorObs (Target.position.x);
		AddVectorObs (Target.position.z);
	}
		
	public override void AgentAction(float[] vectorAction, string textAction){

		Vector3 BallTransform = Ball.transform.position;

		bool baseLeft = false;
		bool goalTouch = false;
		bool wallTouch = false;

		var actionZ = Mathf.Clamp(vectorAction[0], -1f, 1f);
		var actionX = Mathf.Clamp(vectorAction[1], -1f, 1f);

		if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) ||
			(gameObject.transform.rotation.z > -0.25f && actionZ < 0f)) {

			gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
			gameObject.transform.Rotate(new Vector3(0, 1, 1), 0.0f);
		}

		if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) ||
			(gameObject.transform.rotation.x > -0.25f && actionX < 0f)) {

			gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
			gameObject.transform.Rotate(new Vector3(0, 1, 0), 0.0f);
		}
			
		float distanceToTarget = Vector3.Distance (Ball.transform.position, Target.position);

		if (BallTransform.y - gameObject.transform.position.y < -2f ||
		    Mathf.Abs (BallTransform.x - gameObject.transform.position.x) > 2.5f ||
		    Mathf.Abs (BallTransform.z - gameObject.transform.position.z) > 2.5f) {
			SetReward (-11.0f);
			Done ();
		} else {
			for (int i = 0; i < this.transform.childCount; i++) {					
				if (this.transform.GetChild (i).gameObject.tag == "wall") {
					script = this.transform.GetChild (i).gameObject.GetComponent<colider> ();
					if (script.stat == true) {
						//wallTouch = true;
						script.stat = false;
					}
				}
				if (this.transform.GetChild (i).gameObject.tag == "Base") {
					script = this.transform.GetChild (i).gameObject.GetComponent<colider> ();
					if (script.stat == false) {
						baseLeft = true;
					} else {
						script.stat = false;
					}
				}
				if (this.transform.GetChild (i).gameObject.tag == "Goal") {
					script = this.transform.GetChild (i).gameObject.GetComponent<colider> ();
					if (script.stat == true) {
						goalTouch = true;
						script.stat = false;
					}
				}
			}
			if (goalTouch == true || Vector3.Distance (Ball.transform.position, Target.position) < 0.5f) {
				Done ();
				SetReward (0.0f);
			} else if (baseLeft == true) {
				Done ();
				SetReward (-10.0f);
			} else if (wallTouch == true) {
				SetReward (-10.0f);
			} else if (Vector3.Distance (Ball2, Ball1) > Vector3.Distance (Ball2, Ball.transform.position)) {
				SetReward (-10.0f);
			} else {
				SetReward (-1.0f);
			}
		}

		Ball2 = Ball1;
		Ball1 = Ball.transform.position;
	}
}


/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class MazeBaseAgent : Agent
{
    // Start is called before the first frame update

	public GameObject Ball;
	public Transform Target;

	private colider script; // this will be the container of the script

	public Vector3 Ball1;
	public Vector3 Ball2;

	int[,] array2D = new int[9,9] { 
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
		{ 0, 1, 1, 1, 0, 1, 1, 1, 0 }, 
		{ 1, 1, 1, 1, 0, 1, 1, 1, 1 }, 
		{ 1, 1, 1, 1, 0, 1, 1, 1, 1 }, 
		{ 1, 1, 1, 1, 0, 1, 1, 1, 1 }, 
		{ 1, 1, 1, 1, 0, 1, 1, 1, 1 }, 
		{ 1, 1, 1, 1, 0, 1, 1, 1, 1 }, 
		{ 0, 1, 1, 1, 0, 1, 1, 1, 0 }, 
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0 } };

	void Start()
    {
		print("test");

		// make a list of possible positions
		string gap = ", ";
		string gap1 = "        ";

		for (int i = 0; i < this.transform.childCount; i++) {
			if (this.transform.GetChild (i).gameObject.tag == "wall") {
				if (this.transform.GetChild (i).gameObject.transform.position.x*2 <= 4 
					&& this.transform.GetChild (i).gameObject.transform.position.x*2 >= -4
					&& this.transform.GetChild (i).gameObject.transform.position.z*2 <= 4
					&& this.transform.GetChild (i).gameObject.transform.position.z*2 >= -4){

					array2D[(int)(this.transform.GetChild(i).gameObject.transform.position.x*2+4), (int)(this.transform.GetChild(i).gameObject.transform.position.z*2+4)] = 1;
				}
			}
		}
    }

	public override void AgentReset(){

		// reset the board position
		this.transform.position = new Vector3 (0, 0, 0);
		this.transform.rotation = new Quaternion (0, 0, 0, 1);

		// set a random position for Target
		int x = UnityEngine.Random.Range (0, 8);
		int y = UnityEngine.Random.Range (0, 8);
		while(array2D[x, y] != 0){
			x = UnityEngine.Random.Range (0, 8);
			y = UnityEngine.Random.Range (0, 8);
		}
		Target.position = new Vector3 ((x-4.0f)*0.5f, 0.05f, (y-4.0f)*0.5f);

		// set a random position for Ball Agent
		x = UnityEngine.Random.Range (0, 8);
		y = UnityEngine.Random.Range (0, 8);
		while(array2D[x, y] != 0){
			x = UnityEngine.Random.Range (0, 8);
			y = UnityEngine.Random.Range (0, 8);
		}
		Ball.transform.position = new Vector3 ((x-4.0f)*0.5f, 0.05f, (y-4.0f)*0.5f);
		//Ball.transform.position = new Vector3 (0.0f, 0.05f, 0.0f);
	}


	public override void CollectObservations(){

		AddVectorObs (gameObject.transform.rotation.x);
		AddVectorObs (gameObject.transform.rotation.z);

		AddVectorObs (Ball.transform.position.x);
		AddVectorObs (Ball.transform.position.z);

		AddVectorObs (Target.position.x);
		AddVectorObs (Target.position.z);
	}

	public float speed = 0.1f;

	public override void AgentAction(float[] vectorAction, string textAction){


		Vector3 BallTransform = Ball.transform.position;

		var actionZ = 2f * Mathf.Clamp(vectorAction[0], -1f, 1f);
		var actionX = 2f * Mathf.Clamp(vectorAction[1], -1f, 1f);

		if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) ||
			(gameObject.transform.rotation.z > -0.25f && actionZ < 0f)) {

			gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
		}

		if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) ||
			(gameObject.transform.rotation.x > -0.25f && actionX < 0f)) {

			gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
		}
			
		float distanceToTarget = Vector3.Distance (Ball.transform.position, Target.position);

			if (distanceToTarget < 0.5f) {
				SetReward (0.0f);
				Done ();
			} else if (BallTransform.y - gameObject.transform.position.y < -2f ||
			           Mathf.Abs (BallTransform.x - gameObject.transform.position.x) > 2.5f ||
			           Mathf.Abs (BallTransform.z - gameObject.transform.position.z) > 2.5f) {
				SetReward (-11.0f);
				Done ();
			} else {
				bool found = false;
				for (int i = 0; i < this.transform.childCount; i++) {
					
					if (this.transform.GetChild (i).gameObject.tag == "wall") {
						script = this.transform.GetChild (i).gameObject.GetComponent<colider> ();
						if (script.stat == true) {
							SetReward (-11.0f);
							found = true;
							script.stat = false;
						}
					}
				}

				if(Vector3.Distance (Ball2, Ball1) > Vector3.Distance (Ball2, Ball.transform.position) && found == false) {

					SetReward (-10.0f);

				} else if (found == false) {

					SetReward (-10.0f);

				}
			}

		Ball2 = Ball1;
		Ball1 = Ball.transform.position;
	}
}


*/