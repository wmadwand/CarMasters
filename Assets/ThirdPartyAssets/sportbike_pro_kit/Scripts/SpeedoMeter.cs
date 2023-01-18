/// Writen by Boris Chuprin smokerr@mail.ru
/// Great gratitude to everyone who helps me to convert it to C#
/// Thank you so much !!
using UnityEngine;
using System.Collections;

public class SpeedoMeter : MonoBehaviour 
{
	//speedometer
	public Texture2D GUIDashboard;
	public Texture2D dashboardArrow;
	private float topSpeed;//220 for sport/ 180 for chopper
	private float stopAngle;//-200 for sport/ ... for chopper
	private float topSpeedAngle;
	public float speed;

	//tachometer
	public Texture2D chronoTex;
	private float topRPM;// 14000 for sport/ ... for chopper
	private float stopRPMAngle;//-200 for sport/... ... for chopper
	private float topRPMAngle;
	public float RPM;

	//link to bike script
	public BikeController linkToBike;


	void Start () 
	{
		linkToBike = GameObject.Find("rigid_bike").GetComponent<BikeController>();
        //linkToBike = GetComponent<BikeController>();
        findCurrentBike();
	}


	void OnGUI() {
		// speedometer
		GUI.DrawTexture(new Rect(Screen.width*0.85f, Screen.height*0.8f, GUIDashboard.width/2, GUIDashboard.height/2), GUIDashboard);
		Vector2 centre = new Vector2(Screen.width*0.85f + GUIDashboard.width / 4, Screen.height*0.8f + GUIDashboard.height / 4);
		var savedMatrix = GUI.matrix;
		var speedFraction = speed / topSpeed;
		var needleAngle = Mathf.Lerp(stopAngle, topSpeedAngle, speedFraction);
		GUIUtility.RotateAroundPivot(needleAngle, centre);
		GUI.DrawTexture(new Rect(centre.x, centre.y - dashboardArrow.height/4, dashboardArrow.width/2, dashboardArrow.height/2), dashboardArrow);
		GUI.matrix = savedMatrix;

		//tachometer
		GUI.DrawTexture(new Rect(Screen.width*0.70f, Screen.height*0.7f, chronoTex.width/1.5f, chronoTex.height/1.5f), chronoTex);
		var centreTacho = new Vector2(Screen.width*0.70f + chronoTex.width / 3, Screen.height*0.7f + chronoTex.height / 3);
		var savedTachoMatrix = GUI.matrix;
		var tachoFraction = RPM / topRPM;
		var needleTachoAngle = Mathf.Lerp(stopRPMAngle, topRPMAngle, tachoFraction);
		GUIUtility.RotateAroundPivot(needleTachoAngle, centreTacho);
		GUI.DrawTexture(new Rect(centreTacho.x, centreTacho.y - dashboardArrow.height/3, dashboardArrow.width/1.5f, dashboardArrow.height/1.5f), dashboardArrow);
		GUI.matrix = savedTachoMatrix;
	}

	void FixedUpdate()
	{
		speed = linkToBike.bikeSpeed;
		RPM = linkToBike.EngineRPM;
	}

	void findCurrentBike()
	{
		GameObject curBikeName;
		curBikeName = GameObject.Find("rigid_bike");
		if (curBikeName != null){
			StartCoroutine(SetSpeedometerSettings("sport"));
			return;
		}
	}

	IEnumerator SetSpeedometerSettings(string par)
	{
		if (par == "sport")
		{
			topSpeed = 210;
			stopAngle = -215;
			topSpeedAngle = 0;
			topRPM = 12000;
			stopRPMAngle = -200;
			topRPMAngle = 0;
			yield return new WaitForSeconds(0.5f);	
			linkToBike = GameObject.Find("rigid_bike").GetComponent<BikeController>();
		}
	}
}
