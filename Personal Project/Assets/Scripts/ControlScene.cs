using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ControlScene : MonoBehaviour
{
    //variables
    public static int ROUNDS = 5;
    private bool lightOn;
    private bool done;
    private double countdownTimer;
    private double reactionTimer;
    private int finalTime;
    private int attempt;
    private int[] times;
    private string output;

    //objects
    public GameObject sphere;
    public GameObject lighting;
    public Material white;
    public Material red;
    public TextMeshPro mainText;
    public TextMeshPro subText;
    public TextMeshPro results;
    public Canvas textCanvas;
    public Canvas menu;
    public GameObject uiHelpers;


    // Start is called before the first frame update
    void Start()
    {
        times = new int[ROUNDS];
        done = false;
        lightOn = false;
        countdownTimer = 15;
        reactionTimer = 0.0;
        finalTime = 0;
        attempt = 1;
        mainText.SetText("Wait for the light to turn red, when it does press any button. Try to react as quickly as you can!");
        subText.SetText("");
        output = "";
        results.SetText("");
        menu.enabled = false;
        uiHelpers.SetActive(false);
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
                    //stop reaction timer & convert seconds to milliseconds
                    double milliseconds = reactionTimer * 1000;
                    finalTime = (int) milliseconds;
                    
                    output += ("Attempt Number " + attempt + ": Reaction time = " + finalTime + " ms\n");
                    subText.SetText(output);

                    //store time in array
                    times[attempt - 1] = finalTime;

                    //turn off
                    changeLightState(1);

                    //increment attempts
                    attempt += 1;

                    if (attempt <= ROUNDS)
                    {
                        //reset timers
                        resetTimers();
                    }
                    else
                    {
                        //calculating average
                        double avg = 0.0;
                        for (int i = 0; i < times.Length; i++)
                        {
                            avg += times[i];
                        }
                        avg = avg / times.Length;

                        //calculating median
                        sortArray();
                        int halfPoint = (int)Mathf.Floor(times.Length / 2);
                        double median = times[halfPoint];

                        //displaying average
                        results.SetText("Average: " + avg + " ms" + " \nMedian: " + median + "ms");

                        Result result = new Result(output, avg, median);
                        FileManager fm = new FileManager();

                        bool isFileComplete = fm.createResultFile(result, 0);

                        switch (isFileComplete) {
                            case(true):
                                subText.SetText("Data successfully sent to file.");
                                break;
                            case (false):
                                subText.SetText("File saving failed.");
                                break;
                            default:
                                //do nothing
                                break;
                        }

                        done = true;
                    }

                }
                else
                {
                    //pre-emptive press of the button
                    subText.SetText("Wait until the light turns red before pressing any button! Timer restarted for this attempt.");
                    countdownTimer = randomCountdown();
                }
            }

        }
        else
        {
            textCanvas.enabled = false;
            menu.enabled = true;
            uiHelpers.SetActive(true);
        }

    }

    public void sortArray()
    {
        bool sorted = false;
        int temp;
        while (!sorted)
        {
            sorted = true;
            for (int i = 0; i < times.Length - 1; i++)
            {
                if (times[i] > times[i + 1])
                {
                    temp = times[i];
                    times[i] = times[i + 1];
                    times[i + 1] = temp;
                    sorted = false;
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
        return Random.Range(3, 15);
    }

    public void resetTimers()
    {
        countdownTimer = randomCountdown();
        reactionTimer = 0.0;
    }

    public void restart()
    {
        Main.loadScene("ControlScene");
    }

    public void mainMenu()
    {
        Main.loadScene("MainMenu");
    }


}
