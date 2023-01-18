/// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class startScript : MonoBehaviour {

	private float camShift = 0.25f;
	private float devShift = 0.25f;
	public Transform menuCam;
	public Transform mobileDevice;

	void OnGUI ()
	{
		
		GUIStyle biggerText = new GUIStyle ();
		biggerText.fontSize = 40;
		biggerText.normal.textColor = Color.white;
		GUI.Label (new Rect (Screen.width / 2.5f, Screen.height / 20, 100, 90), "Welcome to", biggerText);
		GUI.Label (new Rect (Screen.width / 3.2f, Screen.height / 7, 100, 90), "SPORTBIKE PRO KIT", biggerText);
		
		GUIStyle mediumText = new GUIStyle ();
		mediumText.fontSize = 30;
		mediumText.normal.textColor = Color.white;
		GUI.Label (new Rect (Screen.width / 2.6f, Screen.height / 1.1f, 100, 90), "Choose your bike", mediumText);

	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		//camera moving
		menuCam.transform.Rotate (Vector3.up * camShift * Time.deltaTime);
		if (menuCam.transform.eulerAngles.y >=1 && menuCam.transform.eulerAngles.y <= 5) camShift = -0.25f;
		if (menuCam.transform.eulerAngles.y <=359 && menuCam.transform.eulerAngles.y >= 5) camShift = 0.25f;
		//device moving
		mobileDevice.transform.Rotate (Vector3.up * devShift * Time.deltaTime);
		if (mobileDevice.transform.eulerAngles.y >=1 && mobileDevice.transform.eulerAngles.y <= 5) devShift = -10.5f;
		if (mobileDevice.transform.eulerAngles.y <=359 && mobileDevice.transform.eulerAngles.y >= 5) devShift = 10.5f;


		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			Physics.Raycast (ray, out hit);
			if (hit.transform.gameObject.name == "bikeSport") {
				Application.LoadLevel("u5bike_JS");
			}
			if (hit.transform.gameObject.name == "bikeChop") {
				Application.LoadLevel("u5bike_JS_chop");
			}
			if (hit.transform.gameObject.name == "bikeMotard") {
				Application.LoadLevel("u5bike_JS_motard");
			}
			if (hit.transform.gameObject.name == "mobDevice") {
				Application.LoadLevel("u5bike_JS_mobile");
			}
		}
	}
}
