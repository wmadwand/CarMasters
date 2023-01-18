/// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class carScript : MonoBehaviour {

	// defeine wheel colliders
	public WheelCollider wheelRR;
	public WheelCollider wheelRL;
	public int enginePower;

	bool go = false;


	// Use this for initialization
	void Start () {
	
	
	}


	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Space)) {
			go = true;
		}



		if (go) {
			wheelRR.motorTorque = enginePower;
			wheelRL.motorTorque = enginePower;
		}
	}
}
