using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ControlScene : MonoBehaviour
{
    //variables
    public static int ROUNDS = 5;
    private bool lightOn;
    public static bool done;
    private bool sceneStarted;
    private double countdownTimer;
    private double reactionTimer;
    private int finalTime;
    private int attempt;
    private int[] times;
    private string output;

    //objects
    public Camera mainCamera;
    public GameObject sphere;
    public GameObject lighting;
    public Material white;
    public Material red;
    public TextMeshPro subText;
    public TextMeshPro errorText;
    public TextMeshPro results;
    public TextMeshPro continueText;
    public Canvas startCanvas;
    public Canvas textCanvas;
    public Canvas endScreen;
    public Slider slider;
    public AudioSource inputAudio;


    // Start is called before the first frame update
    void Start()
    {
        // only render objects in the UI Layer
        mainCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
        sceneStarted = false;

        times = new int[ROUNDS];
        done = false;
        lightOn = false;
        countdownTimer = 5;
        reactionTimer = 0.0;
        finalTime = 0;
        attempt = 1;
        subText.SetText("");
        errorText.SetText("");
        output = "";
        results.SetText("");

        slider.maxValue = Controller.heldTime;

        // disable the UI for the end screen
        for (int i = 0; i < endScreen.transform.childCount; i++)
        {
            endScreen.transform.GetChild(i).gameObject.SetActive(false);
        }

        // disable the UI for the main screen
        for (int i = 0; i < textCanvas.transform.childCount; i++)
        {
            textCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.inputsEnabled)
        {
            continueText.gameObject.SetActive(true);
        }

        if (sceneStarted)
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

            }
            else
            {
                textCanvas.gameObject.SetActive(false);

                for (int i = 0; i < endScreen.transform.childCount; i++)
                {
                    endScreen.transform.GetChild(i).gameObject.SetActive(true);
                }

                results.SetText(output);

                if (Controller.timer > 0)
                {
                    slider.gameObject.SetActive(true);
                    slider.value = Controller.timer;

                }
                else
                {
                    slider.gameObject.SetActive(false);
                }
            }
        }


    }

    public void userReacts()
    {
        if (!sceneStarted)
        {
            // render all the other layers in the camera
            for (int i = 0; i < 5; i++)
            {
                mainCamera.cullingMask += 1 << i;
            }

            // disable the UI for the start screen
            startCanvas.gameObject.SetActive(false);

            // disable the UI for the main screen
            for (int i = 0; i < textCanvas.transform.childCount; i++)
            {
                textCanvas.transform.GetChild(i).gameObject.SetActive(true);
            }

            sceneStarted = true;
        }
        else
        {
            if (lightOn)
            {
                //stop reaction timer & convert seconds to milliseconds
                double milliseconds = reactionTimer * 1000;
                finalTime = (int)milliseconds;

                output += ("Attempt " + attempt + ": Reaction time = " + finalTime + "ms\n");
                subText.SetText(output);

                //store time in array
                times[attempt - 1] = finalTime;

                //turn off
                changeLightState(1);

                //increment attempts
                attempt += 1;

                inputAudio.Play();

                if (attempt <= ROUNDS)
                {
                    //reset timers
                    resetTimers();
                }
                else
                {
                    SceneData.controlTimes = times;
                    output += ("Average = " + getAverageControlTime() + "ms\n" + "Median = " + getMedianControlTime() + "ms\n");

                    // only render objects in the UI Layer
                    mainCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");

                    done = true;
                }

            }
            else
            {
                //pre-emptive press of the button
                StartCoroutine(ErrorText());
                countdownTimer = randomCountdown();
            }
        }
    }

    public IEnumerator ErrorText()
    {
        errorText.SetText("Wait until the light turns red before pressing any button! Timer restarted for this attempt.");

        yield return new WaitForSeconds(2);

        errorText.SetText("");

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
        return Random.Range(3, 10);
    }

    public void resetTimers()
    {
        countdownTimer = randomCountdown();
        reactionTimer = 0.0;
    }

    public float getAverageControlTime()
    {
        int total = 0;

        foreach (int time in times)
        {
            total += time;
        }

        //number of control times is 5, so the average will be the total divided by the number of values (5).
        return total / 5;
    }

    public int getMedianControlTime()
    {
        bool sorted = false;
        int tempValue;

        while (!sorted)
        {
            sorted = true;
            for (int i = 0; i < times.Length - 1; i++)
            {
                if (times[i] > times[i + 1])
                {
                    tempValue = times[i];
                    times[i] = times[i + 1];
                    times[i + 1] = tempValue;
                    sorted = false;
                }
            }
        }

        //number of control times is 5, so the median will be the 3rd number. Hence, second index.
        return times[2];

    }


}
