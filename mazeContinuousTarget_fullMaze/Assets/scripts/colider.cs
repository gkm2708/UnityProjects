using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colider : MonoBehaviour
{
	public bool stat;

	// Start is called before the first frame update
    void Start()
    {
		stat = false;
    }


	void OnCollisionEnter(Collision col)
	{
		stat = true;

	}


    // Update is called once per frame
    void Update()
    {
        
    }
}
