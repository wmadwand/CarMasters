/// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class keyboardControls : MonoBehaviour {



	private GameObject ctrlHub;// making a link to corresponding bike's script
	private controlHub outsideControls;// making a link to corresponding bike's script
	


	// Use this for initialization
	void Start () {
		ctrlHub = GameObject.Find("gameScenario");//link to GameObject with script "controlHub"
		outsideControls = ctrlHub.GetComponent<controlHub>();// making a link to corresponding bike's script
	}
	
	// Update is called once per frame
	void Update () {
		//////////////////////////////////// ACCELERATE, braking & full throttle //////////////////////////////////////////////
		// First time it sounds strange but "W" key and "Arrow Up" key causes only 90% of throttle in this script(only keyboard mode).
		// I've made it because 100% of throttle causes "wheelie" at most bikes.
		// I bet wheelie after EVERY acceleration is very unwanted by rider. And you don't want it everytime.
		// That's why I've implement one more "special" acceleration key - Alpha2(key 2) for full 100% throttle.
		// So, "W", "Arrow Up" gives you 1-90% of throttle and key "2" gives you 100%.
		// And yes, this "2" key causes wheelie on most bikes.
		if (!Input.GetKey (KeyCode.Alpha2)) {
			outsideControls.Vertical = Input.GetAxis ("Vertical") / 1.112f;//to get less than 0.9 as acceleration to prevent wheelie(wheelie begins at >0.9)
			if(Input.GetAxis ("Vertical") <0) outsideControls.Vertical = outsideControls.Vertical * 1.112f;//need to get 1(full power) for front brake
		}

		//////////////////////////////////// STEERING /////////////////////////////////////////////////////////////////////////
		outsideControls.Horizontal = Input.GetAxis("Horizontal");
		if (Input.GetKey (KeyCode.Alpha2)) outsideControls.Vertical = 1;
		//}

		//////////////////////////////////// Rider's mass translate ////////////////////////////////////////////////////////////
		//this strings controls pilot's mass shift along bike(vertical)
		if (Input.GetKey (KeyCode.F)) {
			outsideControls.VerticalMassShift = outsideControls.VerticalMassShift += 0.1f;
			if (outsideControls.VerticalMassShift > 1.0f) outsideControls.VerticalMassShift = 1.0f;
		}

		if (Input.GetKey(KeyCode.V)){
			outsideControls.VerticalMassShift = outsideControls.VerticalMassShift -= 0.1f;
			if (outsideControls.VerticalMassShift < -1.0f) outsideControls.VerticalMassShift = -1.0f;
		}
		if(!Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.V)) outsideControls.VerticalMassShift = 0;

		//this strings controls pilot's mass shift across bike(horizontal)
		if (Input.GetKey(KeyCode.E)){
			outsideControls.HorizontalMassShift = outsideControls.HorizontalMassShift += 0.1f;
			if (outsideControls.HorizontalMassShift >1.0f) outsideControls.HorizontalMassShift = 1.0f;
		}

		if (Input.GetKey(KeyCode.Q)){
			outsideControls.HorizontalMassShift = outsideControls.HorizontalMassShift -= 0.1f;
			if (outsideControls.HorizontalMassShift < -1.0f) outsideControls.HorizontalMassShift = -1.0f;
		}
		if(!Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Q)) outsideControls.HorizontalMassShift = 0;


		//////////////////////////////////// ESP switch on/off ////////////////////////////////////////////////////////////////
		//ESP controls
		if(Input.GetKeyDown(KeyCode.Z)){
			if(!outsideControls.ESPMode){
				outsideControls.ESPMode = true;
			} else {
				outsideControls.ESPMode = false;
			}
		}

		//////////////////////////////////// Rear Brake ////////////////////////////////////////////////////////////////
		// Rear Brake
		if (Input.GetKey (KeyCode.X)) {
			outsideControls.rearBrakeOn = true;
		} else
			outsideControls.rearBrakeOn = false;

		//////////////////////////////////// Restart ////////////////////////////////////////////////////////////////
		// Restart & full restart
		if (Input.GetKey (KeyCode.R)) {
			outsideControls.restartBike = true;
		} else
			outsideControls.restartBike = false;

		// RightShift for full restart
		if (Input.GetKey (KeyCode.RightShift)) {
			outsideControls.fullRestartBike = true;
		} else
			outsideControls.fullRestartBike = false;

		//////////////////////////////////// Reverse ////////////////////////////////////////////////////////////////
		// Restart & full restart
		if(Input.GetKeyDown(KeyCode.C)){
				outsideControls.reverse = true;
		} else outsideControls.reverse = false;
		///
	}
}
