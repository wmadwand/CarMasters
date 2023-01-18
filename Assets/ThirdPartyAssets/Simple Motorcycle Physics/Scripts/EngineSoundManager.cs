using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSoundManager : MonoBehaviour
{
    public float MasterVolume; //in dB
    AudioSource audioSource, audioSource2, audioSourceWind;
    public AudioClip[] Samples;
    MotorbikeController motorbikeController;
    public AnimationCurve EngineRpm, CrossFade, CrossFade2, EngineReleaseRpm;
    int changed;
    float prevPitch, prevPitch2, prevVol, prevVol2;
    public bool revLimiter;
    [Range(0, 1)]
    public float revValue;
    public float EngineFlow = 1;

    void Start()
    {
        audioSource = GetComponents<AudioSource>()[0];
        audioSource2 = GetComponents<AudioSource>()[1];
        audioSourceWind = GetComponents<AudioSource>()[2];
        ChangeGearSound(0);
        motorbikeController = FindObjectOfType<MotorbikeController>();

    }
    void Update()
    {
        
        if(revLimiter)
        {if (motorbikeController.revValue > 0.8f && Input.GetKey(KeyCode.W))
        {
            revValue += Time.deltaTime * Random.Range(1, 4);
            revValue %= 1;
            if (revValue > 0.1f && revValue < 0.2f)
                revValue = 0.85f;
        }
        else
            revValue = motorbikeController.revValue;}
        else
        {
            revValue = motorbikeController.revValue;
        }



        if (changed != motorbikeController.currentGear)
        {
            changed = motorbikeController.currentGear;
        if (Input.GetKey(KeyCode.W)||motorbikeController.currentGear == 0)
            ChangeGearSound(motorbikeController.currentGear);
        }

        

        if (Input.GetKey(KeyCode.W))
        {
            audioSource.pitch = (EngineRpm.Evaluate(revValue) + 1) - motorbikeController.currentGear / (Samples.Length-1);
            audioSource2.pitch = (EngineRpm.Evaluate(revValue) + 1) - motorbikeController.currentGear / (Samples.Length-1);
            audioSource.volume = CrossFade.Evaluate(revValue);
            audioSource2.volume = CrossFade2.Evaluate(revValue);
        }
        else
        {
            audioSource.pitch = (EngineReleaseRpm.Evaluate(revValue) + 1) - motorbikeController.currentGear / (Samples.Length-1);
            audioSource2.pitch = (EngineReleaseRpm.Evaluate(revValue) + 1) - motorbikeController.currentGear / (Samples.Length-1);
        }

        audioSource.pitch = Mathf.Lerp(prevPitch, audioSource.pitch, Time.deltaTime * EngineFlow);
        prevPitch = audioSource.pitch;

        audioSource.outputAudioMixerGroup.audioMixer.SetFloat("VolumeCompensation", MasterVolume - motorbikeController.GetComponent<Rigidbody>().velocity.magnitude / motorbikeController.highSpeed / 1);
        audioSource.outputAudioMixerGroup.audioMixer.SetFloat("Distortion", (motorbikeController.GetComponent<Rigidbody>().velocity.magnitude / motorbikeController.highSpeed) / 3 + 0.4f);


        audioSource2.pitch = Mathf.Lerp(prevPitch2, audioSource2.pitch, Time.deltaTime * EngineFlow);
        prevPitch2 = audioSource2.pitch;

        audioSource2.outputAudioMixerGroup.audioMixer.SetFloat("VolumeCompensation", MasterVolume - motorbikeController.GetComponent<Rigidbody>().velocity.magnitude / motorbikeController.highSpeed / 1);
        audioSource2.outputAudioMixerGroup.audioMixer.SetFloat("Distortion", (motorbikeController.GetComponent<Rigidbody>().velocity.magnitude / motorbikeController.highSpeed) / 3 + 0.4f);

        //Wind
        audioSourceWind.volume = motorbikeController.GetComponent<Rigidbody>().velocity.magnitude / motorbikeController.highSpeed + MasterVolume / 10;
    }

    void ChangeGearSound(int gear)
    {
        audioSource.Stop();
        audioSource.clip = Samples[gear];
        audioSource.Play();
        audioSource2.Stop();
        audioSource2.clip = Samples[gear + 1];
        audioSource2.Play();
    }
}
