/// Writen by Boris Chuprin smokerr@mail.ru
/// Great gratitude to everyone who helps me to convert it to C#
/// Thank you so much !!
using UnityEngine;
using System;
using System.Collections;

public class BikeController : MonoBehaviour{
    ///////////////////////////////////////////////////////// wheels ///////////////////////////////////////////////////////////
    // defeine wheel colliders
    public WheelCollider coll_frontWheel;
    public WheelCollider coll_rearWheel;
    // visual for wheels
    public GameObject meshFrontWheel;
    public GameObject meshRearWheel;
    // check isn't front wheel in air for front braking possibility
    bool isFrontWheelInAir = true;
    
    //////////////////////////////////////// Stifness, CoM(center of mass), crashed /////////////////////////////////////////////////////////////
    //for stiffness counting when rear brake is on. Need that to lose real wheel's stiffness during time
    float stiffPowerGain = 0.0f;
    //for CoM moving along and across bike. Pilot's CoM.
    float tmpMassShift = 0.0f;
    // crashed status. To know when we need to desable controls because bike is too leaned.
    public bool crashed = false;
    // there is angles when bike takes status crashed(too much lean, or too much stoppie/wheelie)
    public float crashAngle01;//crashed status is on if bike have more Z(side fall) angle than this												// 70 sport, 55 chopper
    public float crashAngle02;//crashed status is on if bike have less Z(side fall) angle than this 												// 290 sport, 305 chopper
    public float crashAngle03;//crashed status is on if bike have more X(front fall) angle than this 												// 70 sport, 70 chopper
    public float crashAngle04;//crashed status is on if bike have more X(back fall) angle than this												// 280 sport, 70 chopper
    											
    // define CoM of bike
    public Transform CoM; //CoM object
    public float normalCoM; //normalCoM is for situation when script need to return CoM in starting position										// -0.77 sport, -0.38 chopper
    public float CoMWhenCrahsed; //we beed lift CoM for funny bike turning around when crashed													// -0.2 sport, 0.2 chopper
    
    
    
    //////////////////// "beauties" of visuals - some meshes for display visual parts of bike ////////////////////////////////////////////
    public Transform rearPendulumn; //rear pendulumn
    public Transform steeringWheel; //wheel bar
    public Transform suspensionFront_down; //lower part of front forge
    private int normalFrontSuspSpring; // we need to declare it to know what is normal front spring state is
    private int normalRearSuspSpring; // we need to declare it to know what is normal rear spring state is
    bool forgeBlocked = true; // variable to lock front forge for front braking
    //why we need forgeBlocked ?
    //There is little bug in PhysX 3.3 wheelCollider - it works well only with car mass of 1600kg and 4 wheels.
    //If your vehicle is not 4 wheels or mass is not 1600 but 400 - you are in troube.
    //Problem is epic force created by suspension spring when it's full compressed, stretched or wheel comes underground between frames(most catastrophic situation)
    //In all 3 cases your spring will generate crazy force and push rigidbody to the sky.
    //so, my solution is find this moment and make spring weaker for a while then return to it's normal condition.
    
    private float baseDistance; // need to know distance between wheels - base. It's for wheelie compensate(dont want wheelie for long bikes)
    
    // we need to clamp wheelbar angle according the speed. it means - the faster bike rides the less angle you can rotate wheel bar
    public AnimationCurve wheelbarRestrictCurve = new AnimationCurve(new Keyframe(0f, 20f), new Keyframe(100f, 1f));//first number in Keyframe is speed, second is max wheelbar degree
    
    // temporary variable to restrict wheel angle according speed
    float tempMaxWheelAngle;
    
    //variable for cut off wheel bar rotation angle at high speed
    //float wheelPossibleAngle = 0.0f;
    
    //for wheels vusials match up the wheelColliders
    private Vector3 wheelCCenter;
    private RaycastHit hit;
    
    /////////////////////////////////////////// technical variables ///////////////////////////////////////////////////////
    public float bikeSpeed; //to know bike speed km/h
    public bool isReverseOn = false; //to turn On and Off reverse speed
    // Engine
    public float frontBrakePower; //brake power absract - 100 is good brakes																		// 100 sport, 70 chopper
    public float EngineTorque; //engine power(abstract - not in HP or so)																		// 85 sport, 100 chopper
    // airRes is for wind resistance to large bikes more than small ones
    public float airRes; //Air resistant 																										// 1 is neutral
    /// GearBox
    public float MaxEngineRPM; //engine maximum rotation per minute(RPM) when gearbox should switch to higher gear 								// 12000 sport, 7000 chopper
    public float EngineRedline; 																													// 12200 sport, 7200 chopper
    public float MinEngineRPM; //lowest RPM when gear need to be switched down																	// 6000 sport, 2500 chopper
    public float EngineRPM; // engine current rotation per minute(RPM)
    // gear ratios(abstract)
    public float[] GearRatio;//array of gears                                                                                                 	// 6 sport, 5 chopper
    
    public int CurrentGear = 0; // current gear
    
    GameObject ctrlHub;// gameobject with script control variables 
    controlHub outsideControls;// making a link to corresponding bike's script

    //newDefinition of rigidBody
    public Rigidbody m_body;

    ///////////////////////////////////////////////////  ESP system ////////////////////////////////////////////////////////
    bool ESP = false;//ESP turned off by default

    ////////////////////////////////////////////////  ON SCREEN INFO ///////////////////////////////////////////////////////
    public void OnGUI()
    {
    	GUIStyle biggerText = new GUIStyle((GUIStyle)"label");
      	biggerText.fontSize = 40;
      	GUIStyle middleText = new GUIStyle((GUIStyle)"label");
      	middleText.fontSize = 22;
      	GUIStyle smallerText = new GUIStyle((GUIStyle)"label");
      	smallerText.fontSize = 14;
      	
      	//to show in on display interface: speed, gear, ESP status
    	GUI.color = UnityEngine.Color.black;
        GUI.Label(new Rect(Screen.width*0.875f,Screen.height*0.9f, 120.0f, 80.0f), String.Format(""+ "{0:0.}", bikeSpeed), biggerText);
        GUI.Label (new Rect (Screen.width*0.76f,Screen.height*0.88f, 60.0f, 80.0f), "" + (CurrentGear+1),biggerText);
        if (!ESP){
    		GUI.color = UnityEngine.Color.grey;
    		GUI.Label (new Rect (Screen.width*0.885f, Screen.height*0.86f,60.0f,40.0f), "ESP", middleText);
    	} else {
    		GUI.color = UnityEngine.Color.green;
    		GUI.Label (new Rect (Screen.width*0.885f, Screen.height*0.86f,60.0f,40.0f), "ESP", middleText);
    	}
    	 if (!isReverseOn){
    		GUI.color = UnityEngine.Color.grey;
    		GUI.Label (new Rect (Screen.width*0.885f, Screen.height*0.96f,60.0f,40.0f), "REAR", smallerText);
    	} else {
    		GUI.color = UnityEngine.Color.red;
    		GUI.Label (new Rect (Screen.width*0.885f, Screen.height*0.96f,60.0f,40.0f), "REAR", smallerText);
    	}
        
        // user info help lines
        GUI.color = UnityEngine.Color.white;
        GUI.Box (new Rect (10.0f,10.0f,180.0f,20.0f), "A,W,S,D - main control", smallerText);
        GUI.Box (new Rect (10.0f,25.0f,220.0f,20.0f), "2 - more power to accelerate", smallerText);
        GUI.Box (new Rect (10.0f,40.0f,120.0f,20.0f), "X - rear brake", smallerText);
        GUI.Box (new Rect (10.0f,55.0f,320.0f,20.0f), "Q,E,F,V - shift center of mass of biker", smallerText);
        GUI.Box (new Rect (10.0f,70.0f,320.0f,20.0f), "R - restart / RightShift+R - full restart", smallerText);
        GUI.Box (new Rect (10.0f,85.0f,180.0f,20.0f), "RMB - rotate camera around", smallerText);
      	GUI.Box (new Rect (10.0f,100.0f,120.0f,20.0f), "Z - turn on/off ESP", smallerText);
      	GUI.Box (new Rect (10.0f,115.0f,320.0f,20.0f), "C - toggle reverse", smallerText);
      	GUI.Box (new Rect (10.0f,130.0f,320.0f,20.0f), "Esc - return to main menu", smallerText);
      	GUI.color = UnityEngine.Color.black; 
      	
      	
    }
    
    
    public void Start() 
    {    	
        ctrlHub = GameObject.Find("gameScenario");//link to GameObject with script "controlHub"
    	outsideControls = ctrlHub.GetComponent<controlHub>();//to connect mulicontrol script to this one
    
    	Vector3 setInitialTensor = GetComponent<Rigidbody>().inertiaTensor;//this string is necessary for Unity 5.3 with new PhysX feature when Tensor decoupled from center of mass
    	GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);// now Center of Mass(CoM) is alligned to GameObject "CoM"
    	GetComponent<Rigidbody>().inertiaTensor = setInitialTensor;////this string is necessary for Unity 5.3 with new PhysX feature when Tensor decoupled from center of mass

        m_body = GetComponent<Rigidbody>();//seems strange, I know but three strings above strange too but necessary for PhysX ;( 

        // wheel colors for understanding of accelerate, idle, brake(white is idle status)
        meshFrontWheel.GetComponent<Renderer>().material.color = UnityEngine.Color.black;
    	meshRearWheel.GetComponent<Renderer>().material.color = UnityEngine.Color.black;
    	
    	//for better physics of fast moving bodies
    	m_body.interpolation = RigidbodyInterpolation.Interpolate;
    	
    	// too keep EngineTorque variable like "real" horse powers
    	EngineTorque = EngineTorque * 20;
    	
    	//*30 is for good braking to keep frontBrakePower = 100 for good brakes. So, 100 is like sportsbike's Brembo
    	frontBrakePower = frontBrakePower * 30;//30 is abstract but necessary for Unity5
    	
    	//technical variables
    	normalRearSuspSpring = (int)coll_rearWheel.suspensionSpring.spring;
    	normalFrontSuspSpring = (int)coll_frontWheel.suspensionSpring.spring;
    	
    	baseDistance = coll_frontWheel.transform.localPosition.z - coll_rearWheel.transform.localPosition.z;// now we know distance between two wheels
    }
    
    
    public void FixedUpdate(){
    	
    	// if RPM is more than engine can hold we should shift gear up
    	EngineRPM = coll_rearWheel.rpm * GearRatio[CurrentGear];
    	if (EngineRPM > EngineRedline){
    		EngineRPM = MaxEngineRPM;
    	}
    	ShiftGears();
    	// turn ESP on (no stoppie, no wheelie, no falls when brake is on when leaning)
    	ESP = outsideControls.ESPMode;
    
    	ApplyLocalPositionToVisuals(coll_frontWheel);
    	ApplyLocalPositionToVisuals(coll_rearWheel);
     	
     	
     	//////////////////////////////////// part where rear pendulum, wheelbar and wheels meshes matched to wheelsColliers and so on
     	//beauty - rear pendulumn is looking at rear wheel
     	var tmp_cs1 = rearPendulumn.transform.localRotation;
         var tmp_cs2 = tmp_cs1.eulerAngles;
         tmp_cs2.x = 0-8+(meshRearWheel.transform.localPosition.y*100);
         tmp_cs1.eulerAngles = tmp_cs2;
         rearPendulumn.transform.localRotation = tmp_cs1;
     	//beauty - wheel bar rotating by front wheel
    	var tmp_cs3 = suspensionFront_down.transform.localPosition;
        tmp_cs3.y = (meshFrontWheel.transform.localPosition.y - 0.15f);
        suspensionFront_down.transform.localPosition = tmp_cs3;
    	var tmp_cs4 = meshFrontWheel.transform.localPosition;
        tmp_cs4.z = meshFrontWheel.transform.localPosition.z - (suspensionFront_down.transform.localPosition.y + 0.4f)/5;
        meshFrontWheel.transform.localPosition = tmp_cs4;
    
        // debug - all wheels are white in idle(no accelerate, no brake)
    	meshFrontWheel.GetComponent<Renderer>().material.color = Color.black;
    	meshRearWheel.GetComponent<Renderer>().material.color = Color.black;
    	
        // drag and angular drag for emulate air resistance
    	if (!crashed){
    		m_body.drag = m_body.velocity.magnitude / 210  * airRes; // when 250 bike can easy beat 200km/h // ~55 m/s
    		m_body.angularDrag = 7 + m_body.velocity.magnitude/20;
    	}
    	
        //determinate the bike speed in km/h
    	bikeSpeed = Mathf.Round((m_body.velocity.magnitude * 3.6f)*10f) * 0.1f; //from m/s to km/h
    
    //////////////////////////////////// acceleration & brake /////////////////////////////////////////////////////////////
    //////////////////////////////////// ACCELERATE /////////////////////////////////////////////////////////////
    		if (!crashed && outsideControls.Vertical >0 && !isReverseOn){//case with acceleration from 0.0 to 0.9 throttle
    			coll_frontWheel.brakeTorque = 0.0f;//we need that to fix strange unity bug when bike stucks if you press "accelerate" just after "brake".
    			coll_rearWheel.motorTorque = EngineTorque * outsideControls.Vertical;
    
    		    // debug - rear wheel is green when accelerate
    			meshRearWheel.GetComponent<Renderer>().material.color = Color.green;
    			
    			// when normal accelerating CoM z is averaged
    			var tmp_cs5 = CoM.localPosition;
                tmp_cs5.z = 0.0f + tmpMassShift;
                tmp_cs5.y = normalCoM;
                CoM.localPosition = tmp_cs5;
    			m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		//case for reverse
    		if (!crashed && outsideControls.Vertical >0 && isReverseOn){
    			coll_rearWheel.motorTorque = EngineTorque * -outsideControls.Vertical/10+(bikeSpeed*50);//need to make reverse really slow
    
    			// debug - rear wheel is green when accelerate
    			meshRearWheel.GetComponent<Renderer>().material.color = Color.green;
    			
    			// when normal accelerating CoM z is averaged
    			var tmp_cs6 = CoM.localPosition;
                tmp_cs6.z = 0.0f + tmpMassShift;
                tmp_cs6.y = normalCoM;
                CoM.localPosition = tmp_cs6;
    			m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		
    //////////////////////////////////// ACCELERATE full throttle ///////////////////////////////////////////////////////
    		if (!crashed && outsideControls.Vertical >0.9f && !isReverseOn){// acceleration >0.9 throttle for wheelie	
    			coll_frontWheel.brakeTorque = 0.0f;//we need that to fix strange unity bug when bike stucks if you press "accelerate" just after "brake".
    			coll_rearWheel.motorTorque = EngineTorque * 1.2f;//1.2 mean it's full throttle

                meshRearWheel.GetComponent<Renderer>().material.color = Color.green;
    			m_body.angularDrag  = 20;//for wheelie stability
    			
    		
    			if (!ESP){// when ESP we turn off wheelie

                CoM.localPosition = new Vector3(CoM.localPosition.z, CoM.localPosition.y, -(2 - baseDistance / 1.4f) + tmpMassShift);// we got 1 meter in case of sportbike: 2-1.4/1.4 = 1; When we got chopper we'll get ~0.8 as result
                                                                                                                                     //still working on best wheelie code
                float stoppieEmpower = (bikeSpeed/3)/100;
    				// need to supress wheelie when leaning because it's always fall and it't not fun at all
    				float angleLeanCompensate = 0.0f;
                    if (this.transform.localEulerAngles.z < 70){	
    					angleLeanCompensate = this.transform.localEulerAngles.z/30;
    						if (angleLeanCompensate > 0.5f){
    							angleLeanCompensate = 0.5f;
    						}
    				}
    				if (this.transform.localEulerAngles.z > 290){
    					angleLeanCompensate = (360-this.transform.localEulerAngles.z)/30;
    						if (angleLeanCompensate > 0.5f){
    							angleLeanCompensate = 0.5f;
    						}
    				}
    					
    				if (stoppieEmpower + angleLeanCompensate > 0.5f){
    					stoppieEmpower = 0.5f;
    				}
                CoM.localPosition = new Vector3(CoM.localPosition.x, -(1 - baseDistance / 2.8f) - stoppieEmpower, CoM.localPosition.z);
            }

            m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		
    			//this is attenuation for rear suspension targetPosition
    			//I've made it to prevent very strange launch to sky when wheelie in new Phys3
    			if (this.transform.localEulerAngles.x > 200.0f){
    				var tmp_cs9 = coll_rearWheel.suspensionSpring;
                    tmp_cs9.spring = normalRearSuspSpring + (360-this.transform.localEulerAngles.x)*500;
                    coll_rearWheel.suspensionSpring = tmp_cs9;
    				if (coll_rearWheel.suspensionSpring.spring >= normalRearSuspSpring + 20000) {
                            var tmp_cs10 = coll_rearWheel.suspensionSpring;
                            tmp_cs10.spring = (float)(normalRearSuspSpring + 20000);
                            coll_rearWheel.suspensionSpring = tmp_cs10;
                        }
    			}
    		} else RearSuspensionRestoration();
    		
    		
    //////////////////////////////////// BRAKING /////////////////////////////////////////////////////////////
    //////////////////////////////////// front brake /////////////////////////////////////////////////////////
    		int springWeakness = 0;
            if (!crashed && outsideControls.Vertical <0 && !isFrontWheelInAir){
    
    			coll_frontWheel.brakeTorque = frontBrakePower * -outsideControls.Vertical;
    			coll_rearWheel.motorTorque = 0.0f; // you can't do accelerate and braking same time.
    			
    			//more user firendly gomeotric progession braking. But less stoppie and fun :( Boring...
    			//coll_frontWheel.brakeTorque = frontBrakePower * -outsideControls.Vertical-(1 - -outsideControls.Vertical)*-outsideControls.Vertical;
    			
    			if (!ESP)
    			{ // no stoppie when ESP is on
    			    if (bikeSpeed >1)
    			    {// no CoM pull up when speed is zero
    					
    					//when rear brake is used it helps a little to prevent stoppie. Because in real life bike "stretch" a little when you using rear brake just moment before front.
    				    float rearBrakeAddon = 0.0f;
                        if(outsideControls.rearBrakeOn)
    				    {
    						rearBrakeAddon = 0.0025f;
    				    }
    			        //@TODO uncomment has double equals?
    					var tmp_cs11 = CoM.localPosition;
                        tmp_cs11.y += (frontBrakePower/200000) + tmpMassShift / 50f - rearBrakeAddon;
                        tmp_cs11.z += 0.05f;
                        CoM.localPosition = tmp_cs11;
    				} 	
    				else if (bikeSpeed <=1 && !crashed && this.transform.localEulerAngles.z < 45 || bikeSpeed <=1 && !crashed && this.transform.localEulerAngles.z >315)
    				{
    				    if (this.transform.localEulerAngles.x < 5 || this.transform.localEulerAngles.x > 355)
    				    {
    						var tmp_cs12 = CoM.localPosition;
                            tmp_cs12.y = normalCoM;
                            CoM.localPosition = tmp_cs12;
    					}
    				}
    		
    				if (CoM.localPosition.y >= -0.1f) {
                            var tmp_cs13 = CoM.localPosition;
                            tmp_cs13.y = -0.1f;
                            CoM.localPosition = tmp_cs13;
                        }

                if (CoM.localPosition.z >= 0.2f + (m_body.mass / 1100))
                {
                    CoM.localPosition = new Vector3( CoM.localPosition.x,  0.2f + (m_body.mass / 1100), CoM.localPosition.z);
                }
    				//////////// 
    				//this is attenuation for front suspension when forge spring is compressed
    				//I've made it to prevent very strange launch to sky when wheelie in new Phys3
    				//problem is launch bike to sky when spring must expand from compressed state. In real life front forge can't create such force.
    				float maxFrontSuspConstrain = 0.0f;//temporary variable to make constrain for attenuation ususpension(need to make it always ~15% of initial force) 
    				maxFrontSuspConstrain = CoM.localPosition.z;
    				if (maxFrontSuspConstrain >= 0.5f) maxFrontSuspConstrain = 0.5f;
    				springWeakness  = (int)(normalFrontSuspSpring-(normalFrontSuspSpring*1.5f) * maxFrontSuspConstrain);
    				
    			}
    		    
                m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    			// debug - wheel is red when braking
    			meshFrontWheel.GetComponent<Renderer>().material.color = Color.red;
    			
    			//we need to mark suspension as very compressed to make it weaker
    			forgeBlocked = true;
    		} else FrontSuspensionRestoration(springWeakness);//here is function for weak front spring and return it's force slowly
    			
    		
    //////////////////////////////////// rear brake /////////////////////////////////////////////////////////
    		// rear brake - it's all about lose side stiffness more and more till rear brake is pressed
    		if (!crashed && outsideControls.rearBrakeOn){
    			coll_rearWheel.brakeTorque = frontBrakePower / 2;// rear brake is not so good as front brake
    			
    			if (this.transform.localEulerAngles.x > 180 && this.transform.localEulerAngles.x < 350){
    				var tmp_cs14 = CoM.localPosition;
                    tmp_cs14.z = 0.0f + tmpMassShift;
                    CoM.localPosition = tmp_cs14;
    			}
    			
    			var tmp_cs15 = coll_frontWheel.sidewaysFriction;
                tmp_cs15.stiffness = 1.0f - ((stiffPowerGain/2)-tmpMassShift*3);
                coll_frontWheel.sidewaysFriction = tmp_cs15;
    			
    		    //@TODO weirdness
    			stiffPowerGain += 0.025f - (bikeSpeed/10000);
    				if (stiffPowerGain > 0.9f - bikeSpeed/300){ //orig 0.90
    					stiffPowerGain = 0.9f - bikeSpeed/300;
    				}
    				var tmp_cs16 = coll_rearWheel.sidewaysFriction;
                    tmp_cs16.stiffness = 1.0f - stiffPowerGain;
                    coll_rearWheel.sidewaysFriction = tmp_cs16;

    			meshRearWheel.GetComponent<Renderer>().material.color = Color.red;
    			
    		} else{
    
    			coll_rearWheel.brakeTorque = 0.0f;
    		    //@TODO weirdness
    			stiffPowerGain -= 0.05f;
    				if (stiffPowerGain < 0){
    					stiffPowerGain = 0.0f;
    				}
    			var tmp_cs17 = coll_rearWheel.sidewaysFriction;
                tmp_cs17.stiffness = 1.0f - stiffPowerGain;
                coll_rearWheel.sidewaysFriction = tmp_cs17;// side stiffness is back to 2
    			var tmp_cs18 = coll_frontWheel.sidewaysFriction;
                tmp_cs18.stiffness = 1.0f - stiffPowerGain;
                coll_frontWheel.sidewaysFriction = tmp_cs18;// side stiffness is back to 1
    			
    		}
    		
    //////////////////////////////////// reverse /////////////////////////////////////////////////////////
    		if (!crashed && outsideControls.reverse && bikeSpeed <=0){
    				outsideControls.reverse = false;
    				if(isReverseOn == false){
    				isReverseOn = true;
    				} else isReverseOn = false;
    		}
    			
    		
    //////////////////////////////////// turnning /////////////////////////////////////////////////////////////			
    			// there is MOST trick in the code
    			// the Unity physics isn't like real life. Wheel collider isn't round as real bike tyre.
    			// so, face it - you can't reach accurate and physics correct countersteering effect on wheelCollider
    			// For that and many other reasons we restrict front wheel turn angle when when speed is growing
    			//(honestly, there was a time when MotoGP bikes has restricted wheel bar rotation angle by 1.5 degree ! as we got here :)			
    			tempMaxWheelAngle = wheelbarRestrictCurve.Evaluate(bikeSpeed);//associate speed with curve which you've tuned in Editor
    		
    		if (!crashed && outsideControls.Horizontal !=0){	
    		//if (!crashed && Input.GetAxis("Horizontal") !=0){//DEL OLD
    			// while speed is high, wheelbar is restricted 
    			
    			coll_frontWheel.steerAngle = tempMaxWheelAngle * outsideControls.Horizontal;
    			//coll_frontWheel.steerAngle = tempMaxWheelAngle * Input.GetAxis("Horizontal");//DEL OLD
    			steeringWheel.rotation = coll_frontWheel.transform.rotation * Quaternion.Euler (0.0f, coll_frontWheel.steerAngle, coll_frontWheel.transform.rotation.z);
    		} else coll_frontWheel.steerAngle = 0.0f;
    		
    		
    /////////////////////////////////////////////////// PILOT'S MASS //////////////////////////////////////////////////////////
    // it's part about moving of pilot's center of mass. It can be used for wheelie or stoppie control and for motocross section in future
    		//not polished yet. For mobile version it should back pilot's mass smooth not in one tick
    		if (outsideControls.VerticalMassShift >0){
    			tmpMassShift = outsideControls.VerticalMassShift/12.5f;//12.5 to get 0.08m at final
    			var tmp_cs19 = CoM.localPosition;
                tmp_cs19.z = tmpMassShift;
                CoM.localPosition = tmp_cs19;	
    		    m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		if (outsideControls.VerticalMassShift <0){
    			tmpMassShift = outsideControls.VerticalMassShift/12.5f;//12.5 to get 0.08m at final
    			var tmp_cs20 = CoM.localPosition;
                tmp_cs20.z = tmpMassShift;
                CoM.localPosition = tmp_cs20;
    		    m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		if (outsideControls.HorizontalMassShift <0){
    		    var tmp_cs21 = CoM.localPosition;
                tmp_cs21.x = outsideControls.HorizontalMassShift/40;
                CoM.localPosition = tmp_cs21;//40 to get 0.025m at final
    		    m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    			
    		}
    		if (outsideControls.HorizontalMassShift >0){
    		    var tmp_cs22 = CoM.localPosition;
                tmp_cs22.x = outsideControls.HorizontalMassShift/40;
                CoM.localPosition = tmp_cs22;//40 to get 0.025m at final
    		    m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		
    		
    		//auto back CoM when any key not pressed
    		if (!crashed && outsideControls.Vertical == 0 && !outsideControls.rearBrakeOn || (outsideControls.Vertical < 0 && isFrontWheelInAir)){
    			var tmp_cs23 = CoM.localPosition;
                tmp_cs23.y = normalCoM;
                tmp_cs23.z = 0.0f + tmpMassShift;
                CoM.localPosition = tmp_cs23;
    			coll_frontWheel.motorTorque = 0.0f;
    			coll_frontWheel.brakeTorque = 0.0f;
    			coll_rearWheel.motorTorque = 0.0f;
    			coll_rearWheel.brakeTorque = 0.0f;
    		    m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		//autoback pilot's CoM along
    		if (outsideControls.VerticalMassShift == 0){
    			var tmp_cs24 = CoM.localPosition;
                tmp_cs24.z = 0.0f;
                CoM.localPosition = tmp_cs24;
    			tmpMassShift = 0.0f;
    		}
    		//autoback pilot's CoM across
    		if (outsideControls.HorizontalMassShift == 0){
    			var tmp_cs25 = CoM.localPosition;
                tmp_cs25.x = 0.0f;
                CoM.localPosition = tmp_cs25;
    		}
    		
    /////////////////////////////////////////////////////// RESTART KEY ///////////////////////////////////////////////////////////
    		// Restart key - recreate bike few meters above current place
    		if (outsideControls.restartBike){
    			if (outsideControls.fullRestartBike){
    				transform.position = new Vector3(0.0f,1.0f,-11.0f);
    				transform.rotation=Quaternion.Euler( 0.0f, 0.0f, 0.0f );
    			}
    			crashed = false;
    			transform.position += new Vector3(0.0f,0.1f,0.0f);
    			transform.rotation = Quaternion.Euler( 0.0f, transform.localEulerAngles.y, 0.0f );
    		    m_body.velocity = Vector3.zero;
    			m_body.angularVelocity = Vector3.zero;
    			var tmp_cs26 = CoM.localPosition;
                tmp_cs26.x = 0.0f;
                tmp_cs26.y = normalCoM;
                tmp_cs26.z = 0.0f;
                CoM.localPosition = tmp_cs26;
    			//for fix bug when front wheel IN ground after restart(sorry, I really don't understand why it happens);
    			coll_frontWheel.motorTorque = 0.0f;
    			coll_frontWheel.brakeTorque = 0.0f;
    			coll_rearWheel.motorTorque = 0.0f;
    			coll_rearWheel.brakeTorque = 0.0f;
    		    
    			m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		
    		
    		
    ///////////////////////////////////////// CRASH happens /////////////////////////////////////////////////////////
    	// conditions when crash is happen
    		if ((this.transform.localEulerAngles.z >=crashAngle01 && this.transform.localEulerAngles.z <=crashAngle02) || (this.transform.localEulerAngles.x >=crashAngle03 && this.transform.localEulerAngles.x <=crashAngle04))
        {
    		
            m_body.drag = 0.1f; // when 250 bike can easy beat 200km/h // ~55 m/s
    		m_body.angularDrag = 0.01f;
    		crashed = true;
    		var tmp_cs27 = CoM.localPosition;
            tmp_cs27.x = 0.0f;
            tmp_cs27.y = CoMWhenCrahsed;
            tmp_cs27.z = 0.0f;
            CoM.localPosition = tmp_cs27;
            m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    	}
    	
    	if (crashed) coll_rearWheel.motorTorque = 0.0f;//to prevent some bug when bike crashed but still accelerating
    }
    
    //function Update () {
    	//not use that because everything here is about physics
    //}
    ///////////////////////////////////////////// FUNCTIONS /////////////////////////////////////////////////////////
    public void ShiftGears() {
    	int AppropriateGear = 0;
        if ( EngineRPM >= MaxEngineRPM ) {
    		AppropriateGear = CurrentGear;
    		
    		for(int i = 0; i < GearRatio.Length; i ++ ) {
    			if (coll_rearWheel.rpm * GearRatio[i] < MaxEngineRPM ) {
    				AppropriateGear = i;
    				break;
    			}
    		}
    		
    		CurrentGear = AppropriateGear;
    	}
    	
    	if ( EngineRPM <= MinEngineRPM ) {
    		AppropriateGear = CurrentGear;
    		
    		for(int j = GearRatio.Length-1; j >= 0; j -- ) {
    			if (coll_rearWheel.rpm * GearRatio[j] > MinEngineRPM ) {
    				AppropriateGear = j;
    				break;
    			}
    		}
    		CurrentGear = AppropriateGear;
    	}
    }
    	
    public void ApplyLocalPositionToVisuals(WheelCollider collider) {
    		if (collider.transform.childCount == 0) {
    			return;
    		}
    		
    		Transform visualWheel = collider.transform.GetChild (0);
    		wheelCCenter = collider.transform.TransformPoint (collider.center);	
    		if (Physics.Raycast (wheelCCenter, -collider.transform.up, out hit, collider.suspensionDistance + collider.radius)) {
    			visualWheel.transform.position = hit.point + (collider.transform.up * collider.radius);
    			if (collider.name == "coll_front_wheel") isFrontWheelInAir = false;
    			
    		} else {
    			visualWheel.transform.position = wheelCCenter - (collider.transform.up * collider.suspensionDistance);
    			if (collider.name == "coll_front_wheel") isFrontWheelInAir = true;
    		}
    		Vector3 position = Vector3.zero;
    		Quaternion rotation = Quaternion.identity;
    		collider.GetWorldPose (out position, out rotation);
    
    		visualWheel.localEulerAngles = new Vector3(visualWheel.localEulerAngles.x, collider.steerAngle - visualWheel.localEulerAngles.z, visualWheel.localEulerAngles.z);
    		visualWheel.Rotate (collider.rpm / 60 * 360 * Time.deltaTime, 0.0f, 0.0f);
    
    }
    //need to restore spring power for rear suspension after make it harder for wheelie
    public void RearSuspensionRestoration(){
        if (coll_rearWheel.suspensionSpring.spring > normalRearSuspSpring)
        {
    		var tmp_cs28 = coll_rearWheel.suspensionSpring;
            tmp_cs28.spring -= 500.0f;
            coll_rearWheel.suspensionSpring = tmp_cs28;
    	}
    }
    //need to restore spring power for front suspension after make it weaker for stoppie
    public void FrontSuspensionRestoration(int sprWeakness){
    	if (forgeBlocked) {//supress front spring power to avoid too much force back
    		var tmp_cs29 = coll_frontWheel.suspensionSpring;
            tmp_cs29.spring = (float)sprWeakness;
            coll_frontWheel.suspensionSpring = tmp_cs29;
    		forgeBlocked = false;
    	}
    	if (coll_frontWheel.suspensionSpring.spring < normalFrontSuspSpring){//slowly returning force to front spring
    		var tmp_cs30 = coll_frontWheel.suspensionSpring;
            tmp_cs30.spring += 500.0f;
            coll_frontWheel.suspensionSpring = tmp_cs30;
    	}
    }
}