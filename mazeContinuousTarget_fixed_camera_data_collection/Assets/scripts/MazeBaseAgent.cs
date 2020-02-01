using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MazeBaseAgent : Agent
{
    // Start is called before the first frame update
	public GameObject Ball;
	public GameObject Goal;
	//public Transform Target;

	//Physics.gravity = Vector3(0, -1.0, 0);

	private float desired_physics_update = 1.0f;

	private int[,] Maze;

	private float y_pos_ball = 0.75f;
	private float y_pos_goal = 0.51f;

	private float actionX;
	private float actionZ;

	private GameObject wall;
	private GameObject temp; 

	private Rigidbody m_Rigidbody;
	private Vector3 m_EulerAngleVelocity;

	private colider script; // this will be the container of the script

	private Vector3 integrator;
	private Vector3 error;

	private float maxForce_x = 0.0f;
	private float maxForce_z = 0.0f;


	void Start()
    {
		integrator = new Vector3 (0.0f, 0.0f, 0.0f);

		Maze = new int[,]{{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};


		/*

		// first create the base board

		int dim = Maze.Rank;
		int row = Maze.GetLength(0);
		int columns = Maze.GetLength(1);


		GameObject prefab = Resources.Load("Cube") as GameObject;
		for (int i=0; i<row; i++){
			for (int j = 0; j < columns; j++) {

				// check if array has 0 at a place
				if (Maze[i,j] == 0){
					GameObject go = Instantiate (prefab) as GameObject;
					go.transform.position = new Vector3 (-33.0f +i , 0.55f, -33.0f + j);
					go.transform.parent = gameObject.transform;
				}
			}
		}

		*/
    }







	public override void AgentReset(){
		
		//Ball.transform.position = new Vector3 (0.0f, 0.55f, 0.0f);
		//Target.position = new Vector3 (0.0f, 0.05f, 0.0f);

		int ball_x = 0;
		int ball_y = 0;

		// Random ball position in feasible space
		ball_x = UnityEngine.Random.Range(1, 67);
		ball_y = UnityEngine.Random.Range(1, 67);

		while (Maze [ball_x, ball_y] == 0) {
			ball_x = UnityEngine.Random.Range(1, 67);
			ball_y = UnityEngine.Random.Range(1, 67);
		}

		//Ball.transform.position = new Vector3 (-33.0f + ball_x, y_pos_ball, -33.0f + ball_y);
		Ball.transform.position = new Vector3 (0.0f, 0.0f, 0.0f);
		Ball.GetComponent<Rigidbody>().AddForce(0, 0, 0);
		Ball.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);

		// Random ball position in feasible space
		ball_x = UnityEngine.Random.Range(1, 67);
		ball_y = UnityEngine.Random.Range(1, 67);

		while (Maze[ball_x, ball_y] == 0) {
			ball_x = UnityEngine.Random.Range(1, 67);
			ball_y = UnityEngine.Random.Range(1, 67);
		}

		Goal.transform.position = new Vector3 (-33.0f + ball_x, y_pos_goal, -33.0f + ball_y);
		Goal.transform.parent = gameObject.transform;

		gameObject.transform.position = new Vector3 (0, 0, 0);
		gameObject.transform.rotation = new Quaternion (0.0f, 0.0f, 0.0f, 1);

	}








	public override void CollectObservations(){

		AddVectorObs (gameObject.transform.rotation.x);
		AddVectorObs (gameObject.transform.rotation.z);

		AddVectorObs (Ball.transform.position.x);
		AddVectorObs (Ball.transform.position.z);

		AddVectorObs (Goal.transform.position.x);
		AddVectorObs (Goal.transform.position.z);

		AddVectorObs (Ball.GetComponent<Rigidbody>().velocity.x);
		AddVectorObs (Ball.GetComponent<Rigidbody>().velocity.z);

	}



	void Update()
	{
		maxForce_x = Mathf.Atan(1.0f/Ball.transform.position.x);
		maxForce_z = Mathf.Atan(1.0f/Ball.transform.position.z);

		float delta_x = 0;
		float delta_z = 0;

		float pGain = 0.001f; // the proportional gain
		float iGain = 0.005f; // the integral gain
		float dGain = 0.005f; // differential gain

		delta_x = Mathf.DeltaAngle (gameObject.transform.eulerAngles.x, actionX);
		float delta_y = Mathf.DeltaAngle (gameObject.transform.eulerAngles.y, 0);
		delta_z = Mathf.DeltaAngle (gameObject.transform.eulerAngles.z, actionZ);

		/*
		if ((gameObject.transform.eulerAngles.x <= 0 && actionX >= 0)||(gameObject.transform.eulerAngles.x >= 0 && actionX <= 0)) {
			delta_x = Mathf.DeltaAngle (gameObject.transform.eulerAngles.x, actionX);
			Debug.Log ("x");
		} else {
			delta_x = Mathf.DeltaAngle (gameObject.transform.eulerAngles.x, 0);
			Debug.Log ("x_");
		}


		float delta_y = Mathf.DeltaAngle (gameObject.transform.eulerAngles.y, 0);


		if ((gameObject.transform.eulerAngles.z <= 0 && actionZ >= 0)||(gameObject.transform.eulerAngles.z >= 0 && actionZ <= 0)) {
			delta_z = Mathf.DeltaAngle (gameObject.transform.eulerAngles.z, actionZ);
			Debug.Log ("z");
		} else {
			delta_z = Mathf.DeltaAngle (gameObject.transform.eulerAngles.z, 0);
			Debug.Log ("z_");
		}
		*/

		error = new Vector3 (delta_x, delta_y, delta_z);

		integrator += error * Time.deltaTime; // integrate error
		Vector3 diff = error/ Time.deltaTime; // differentiate error

		// calculate the force summing the 3 errors with respective gains:
		Vector3 force = error * pGain + integrator * iGain + diff * dGain;

		// clamp the force to the max value available
		//force = Vector3.ClampMagnitude(force, maxForce);

		Vector3 finalMove = new Vector3(
			gameObject.transform.eulerAngles.x + Mathf.Clamp(force.x, -1*maxForce_z, maxForce_z), 
			gameObject.transform.eulerAngles.y + force.y,
			gameObject.transform.eulerAngles.z + Mathf.Clamp(force.z, -1*maxForce_x, maxForce_x)); 


		// apply the force to accelerate the rigidbody:
		m_Rigidbody = gameObject.GetComponent<Rigidbody>();
		m_Rigidbody.MoveRotation(Quaternion.Euler(finalMove));
	
	}








	void OnGUI() {
		GUI.Label (new Rect (200, 5, 240, 160), "Angular Rotation: " + gameObject.transform.eulerAngles);
		GUI.Label (new Rect (200, 30, 240, 160), "action Received: " + actionX +" "+ actionZ);
		GUI.Label (new Rect (200, 55, 240, 160), "Angular Velocity: " + m_EulerAngleVelocity);
		GUI.Label (new Rect (200, 80, 240, 160), "Rotation limit: " + maxForce_x + " " + maxForce_z);
	}








	public override void AgentAction(float[] vectorAction, string textAction){

		Vector3 BallTransform = Ball.transform.position;

		//Ball.GetComponent<Rigidbody>().velocity = new Vector3(vectorAction[2], 0.0f, vectorAction[3]);

		// desired rotation
		actionX = vectorAction[0];
		actionZ = vectorAction[1];

		// current rotation
		//float current_rot_x = gameObject.transform.eulerAngles.x;
		//float current_rot_z = gameObject.transform.eulerAngles.z;

		// delta rotation
		//delta_rot_x = actionX - current_rot_x;
		//delta_rot_z = actionZ - current_rot_z;


		float distanceToTarget = Vector3.Distance (Ball.transform.position, Goal.transform.position);

		if (distanceToTarget < 0.5f) {
			SetReward (0.0f);
			Done ();
		} else if (BallTransform.y - gameObject.transform.position.y < -20f ||
			Mathf.Abs (BallTransform.x - gameObject.transform.position.x) > 33.5f ||
			Mathf.Abs (BallTransform.z - gameObject.transform.position.z) > 33.5f) {
			SetReward (-11.0f);
			Done ();
		}
	}
}









/*

	public override void AgentAction(float[] vectorAction, string textAction){

		//Vector3 BallTransform = Ball.transform.position;
		//Ball.GetComponent<Rigidbody>().AddForce(vectorAction[2], 0, vectorAction[3]);
		//Ball.GetComponent<Rigidbody>().velocity = new Vector3(vectorAction[2], 0.0f, vectorAction[3]);

		//Debug.Log (actionZ);
		//Debug.Log (actionX);


		// desired rotation
		var actionZ = vectorAction[0];
		var actionX = vectorAction[1];


		// current rotation
		float current_rot_x = gameObject.transform.rotation.x;
		float current_rot_z = gameObject.transform.rotation.z;


		// delta rotation
		delta_rot_x = actionX - current_rot_x;
		delta_rot_z = actionZ - current_rot_z;



		//gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(delta_rot_x/10, 0.0f, delta_rot_z/10);

		//var actionZ = 2f * Mathf.Clamp(vectorAction[0], -1f, 1f);
		//var actionX = 2f * Mathf.Clamp(vectorAction[1], -1f, 1f);

		if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) ||
			(gameObject.transform.rotation.x > -0.25f && actionX < 0f)) {

			//gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(delta_rot_x/10, 0.0f, 0.0f);
			gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
		}

		if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) ||
			(gameObject.transform.rotation.z > -0.25f && actionZ < 0f)) {

			gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
		}


		//Debug.Log (gameObject.transform.rotation);

		// moving ball to next position
		int x_pos = (int)(-36.0f + vectorAction[0]); 
		int y_pos = (int)(36.0f - vectorAction[1]);

		//Debug.Log (x_pos);
		//Debug.Log (y_pos);

		if (distanceToTarget < 0.5f) {
			SetReward (0.0f);
			Done ();
		} else if (BallTransform.y - gameObject.transform.position.y < -20f ||
			Mathf.Abs (BallTransform.x - gameObject.transform.position.x) > 33.5f ||
			Mathf.Abs (BallTransform.z - gameObject.transform.position.z) > 33.5f) {
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
			if (found == false) {
				SetReward (-10.0f);
			}
		}
	}
	*/


