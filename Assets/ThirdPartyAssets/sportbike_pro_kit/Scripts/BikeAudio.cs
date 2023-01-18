/// Writen by Boris Chuprin smokerr@mail.ru
/// Great gratitude to everyone who helps me to convert it to C#
/// Thank you so much !!
using UnityEngine;
using System.Collections;

public class BikeAudio : MonoBehaviour
{
    /// Writen by Boris Chuprin smokerr@mail.ru
    //public ` linkToBike;// making a link to corresponding bike's script
    //why it's here ? because you might use this script with many bikes in one scene
    //it's reasonable for multiplayer games
    public BikeController linkToBike;// making a link to corresponding bike's script

    public int lastGear;//we need to know what gear is now
	private AudioSource highRPMAudio;// makeing second audioSource for mixing idle and high RPMs
	private AudioSource skidSound;// makeing another audioSource for skidding sound

	// creating sounds(Link it to real sound files at editor)
	public AudioClip engineStartSound;
	public AudioClip gearingSound;
	public AudioClip IdleRPM;
	public AudioClip highRPM;
	public AudioClip skid;

	//we need to know is any wheel skidding
	public bool isSkidingFront = false;
	public bool isSkidingRear = false;

	private AudioSource engineSource;

	void Start () 
	{
		//assign sound to audioSource
		highRPMAudio = gameObject.AddComponent<AudioSource>();
		highRPMAudio.loop = true;
		highRPMAudio.playOnAwake = false;
		highRPMAudio.clip = highRPM;
		highRPMAudio.pitch = 0;
		highRPMAudio.volume = 0.0f;
		//same assign for skid sound
		skidSound = gameObject.AddComponent<AudioSource>();
		skidSound.loop = false;
		skidSound.playOnAwake = false;
		skidSound.clip = skid;
		skidSound.pitch = 1.0f;
		skidSound.volume = 1.0f;

		//real-time linking to current bike
		//linkToBike = GetComponent<BikeControllerV2>();
        linkToBike = GetComponent<BikeController>();
        engineSource = GetComponent<AudioSource>();
		engineSource.PlayOneShot(engineStartSound);
		StartCoroutine(playEngineWorkSound());
		lastGear = linkToBike.CurrentGear;
	}


	void Update()
	{

		//Idle plays high at slow speed and highRPM sound play silent at same time. And vice versa.
		engineSource.pitch = Mathf.Abs(linkToBike.EngineRPM  / linkToBike.MaxEngineRPM) + 1.0f;
		engineSource.volume = 1.0f - (Mathf.Abs(linkToBike.EngineRPM  / linkToBike.MaxEngineRPM));
		highRPMAudio.pitch = Mathf.Abs(linkToBike.EngineRPM  / linkToBike.MaxEngineRPM);
		highRPMAudio.volume = Mathf.Abs(linkToBike.EngineRPM  / linkToBike.MaxEngineRPM);

		// all engine sounds stop when restart
		if (Input.GetKey(KeyCode.R)){
			engineSource.Stop();
			engineSource.pitch = 1.0f;
			engineSource.PlayOneShot(engineStartSound);
			StartCoroutine(playEngineWorkSound());
		}

		//gear change sound
		if (linkToBike.CurrentGear != lastGear){
			engineSource.PlayOneShot(gearingSound);//звук переключения передач
			lastGear = linkToBike.CurrentGear;
		}
		//skids sound
		if (linkToBike.coll_rearWheel.sidewaysFriction.stiffness < 0.5 && !isSkidingRear && linkToBike.bikeSpeed >1){//почему 0.5 ? как бы лучше это обыграть ?
			skidSound.Play();
			isSkidingRear = true;
		} else if (linkToBike.coll_rearWheel.sidewaysFriction.stiffness >= 0.5 && isSkidingRear || linkToBike.bikeSpeed <=1){
			skidSound.Stop();
			isSkidingRear = false;
		}
		if (linkToBike.coll_frontWheel.brakeTorque >= linkToBike.frontBrakePower && !isSkidingFront && linkToBike.bikeSpeed >1){
			skidSound.Play();
			isSkidingFront = true;
		} else if (linkToBike.coll_frontWheel.brakeTorque < linkToBike.frontBrakePower && isSkidingFront || linkToBike.bikeSpeed <=1){
			skidSound.Stop();
			isSkidingFront = false;
		}
	}

	IEnumerator playEngineWorkSound()
	{
		yield return new WaitForSeconds(1);//need a pause to hear ingnition sound first 
		engineSource.clip = IdleRPM;
		engineSource.Play();
		highRPMAudio.Play();
	}
}
