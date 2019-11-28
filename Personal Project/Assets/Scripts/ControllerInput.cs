using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControllerInput : MonoBehaviour
{
    //variables
    private bool lightOn;
    private bool done;
    private double countdownTimer;
    private double reactionTimer;
    private double finalTime;
    private int attempt;
    private double[] times;

    //objects
    public GameObject sphere;
    public GameObject lighting;
    public Material white;
    public Material red;
    public TextMeshPro mainText;
    public TextMeshPro subText;


    // Start is called before the first frame update
    void Start()
    {
        times = new double[3];
        done = false;
        lightOn = false;
        countdownTimer = 15;
        reactionTimer = 0.0;
        finalTime = 0.0;
        attempt = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!done)
        {
            //countdown for timer
            countdownTimer -= Time.deltaTime;


            //if timer reaches zero
            if (countdownTimer <= 0)
            {
                //turn light on
                changeLightState(0);
                reactionTimer += Time.deltaTime;
            }

            
            if (OVRInput.GetDown(OVRInput.Button.Any))
            {
                if (lightOn)
                {
                    //stop reaction timer
                    finalTime = reactionTimer;
                    subText.SetText("Attempt Number " + attempt + ": Reaction time = " + finalTime + " seconds");

                    //store time in array
                    times[attempt - 1] = finalTime;

                    //turn off
                    changeLightState(1);

                    //increment attempts
                    attempt += 1;

                    if (attempt < 4)
                    {
                        //reset timers
                        resetTimers();
                    }
                    else
                    {
                        //calculating average
                        double avg = 0.0;
                        for(int i = 0; i < times.Length; i++)
                        {
                            avg += times[i];
                        }
                        avg = avg / times.Length;

                        //displaying average
                        mainText.SetText("Done. Average Reaction Time = " + avg + " seconds");
                        done = true;
                    }

                }
                else
                {
                    //pre-emptive press of the button
                    subText.SetText("Wait until the light turns red! Timer restarted");
                    countdownTimer = randomCountdown();
                }
            }

        }

    }

    public void changeLightState(int i)
    {
        //turn light on
        if (i == 0)
        {
            sphere.transform.position.Set(transform.position.x, (transform.position.y + 0.2f), transform.position.z);
            sphere.GetComponent<MeshRenderer>().material = red;
            lighting.GetComponent<Light>().enabled = true;
            lightOn = true;
        }
        else //turn light off
        {
            sphere.transform.position.Set(transform.position.x, (transform.position.y - 0.2f), transform.position.z);
            sphere.GetComponent<MeshRenderer>().material = white;
            lighting.GetComponent<Light>().enabled = false;
            lightOn = false;
        }
    }

    public double randomCountdown()
    {
        return Random.Range(5, 15);
    }

    public void resetTimers()
    {
        countdownTimer = randomCountdown();
        reactionTimer = 0.0;
    }


}
