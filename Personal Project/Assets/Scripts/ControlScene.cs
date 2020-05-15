using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Class for scripting the events of the control scene.
 */
public class ControlScene : MonoBehaviour
{
    //Variables
    public static int ROUNDS = 5;
    private int finalTime;
    private int attempt;
    private int[] times;
    private float countdownTimer;
    private float reactionTimer;
    private bool lightOn;
    public static bool done;
    private bool sceneStarted;
    private string output;

    //GameObjects
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
        //only render objects in the UI Layer at the start of the scene (only the start UI)
        mainCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");

        //initialise variable
        sceneStarted = false;
        times = new int[ROUNDS];
        done = false;
        lightOn = false;
        countdownTimer = 5;
        reactionTimer = 0;
        finalTime = 0;
        attempt = 1;
        subText.SetText("");
        errorText.SetText("");
        output = "";
        results.SetText("");
        slider.maxValue = Controller.heldTime;

        //disable the UI for the end screen, this will be enabled at the end of the scene
        for (int i = 0; i < endScreen.transform.childCount; i++)
        {
            endScreen.transform.GetChild(i).gameObject.SetActive(false);
        }

        //disable the UI for the main screen., this will be enabled at the start of the scene
        for (int i = 0; i < textCanvas.transform.childCount; i++)
        {
            textCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //once inputs are enabled, display the text that shows the user that they can start the scene
        if (Controller.inputsEnabled)
        {
            continueText.gameObject.SetActive(true);
        }

        //checks if the scene has started
        if (sceneStarted)
        {
            //checks if the scene has finished
            if (!done)
            {
                //countdown for timer decreases by the time elapsed since the last frame
                countdownTimer -= Time.deltaTime;

                //checks if the timer reaches zero
                if (countdownTimer <= 0)
                {
                    //turn light on & start the reaction timer
                    changeLightState(0);
                    reactionTimer += Time.deltaTime;
                }
            }
            else
            {

                //adjust the UI slider to how long the user has held down the response button to continue to the next scene.
                if (Controller.timer > 0)
                {
                    slider.gameObject.SetActive(true);
                    slider.value = Controller.timer;

                }
                else
                {
                    //if timer is zero or below zero then don't display the slider
                    slider.gameObject.SetActive(false);
                }
            }
        }
    }

    /*
     * Method that runs when the user reacts
     */
    public void userReacts()
    {
        //checks if the scene has started
        if (!sceneStarted)
        {
            //if scene hasn't started, the user input will start the scene

            //render all the other layers in the camera (the UI layer has already been set to render from the start screen UI)
            for (int i = 0; i < 5; i++)
            {
                mainCamera.cullingMask += 1 << i;
            }

            //disable the start screen UI
            startCanvas.gameObject.SetActive(false);

            //enable the UI for the main screen
            for (int i = 0; i < textCanvas.transform.childCount; i++)
            {
                textCanvas.transform.GetChild(i).gameObject.SetActive(true);
            }

            //set scenestarted to true
            sceneStarted = true;
        }
        else
        {
            //if scene has already started, the user input will be used as a reaction

            //checks if the light that the user is reacting to is on
            if (lightOn)
            {
                //the light is on, so the reaction is valid

                //stop reaction timer & convert seconds to milliseconds
                float milliseconds = reactionTimer * 1000;
                finalTime = (int)milliseconds;

                //format the string of the output & set the UI text to this value
                output += ("Attempt " + attempt + ": Reaction time = " + finalTime + "ms\n");
                subText.SetText(output);

                //store reaction time in array
                times[attempt - 1] = finalTime;

                //turn off the light
                changeLightState(1);

                //increment attempts
                attempt += 1;

                //play audio when the user reacts (for input feedback to the user)
                inputAudio.PlayOneShot(inputAudio.clip);

                //checks if the number of attempts completed are less than or equal to the max number of rounds (5)
                if (attempt <= ROUNDS)
                {
                    //if so, reset timers (everything else continues as normal)
                    resetTimers();
                }
                else
                {
                    //if everything has been completed, save the array of reaction times in the scenedata class
                    SceneData.controlTimes = times;

                    //format the string of the output to add the mean and median of all the times 
                    output += ("Average = " + getAverageControlTime() + "ms\n" + "Median = " + getMedianControlTime() + "ms\n");

                    // only render objects in the UI Layer
                    mainCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");

                    //scene is done, so disable the current UI from the scene
                    textCanvas.gameObject.SetActive(false);

                    //enabled the end screen UI
                    for (int i = 0; i < endScreen.transform.childCount; i++)
                    {
                        endScreen.transform.GetChild(i).gameObject.SetActive(true);
                    }

                    //set the end screen results text to the value of output (all results written down and the mean and median)
                    results.SetText(output);

                    //set done to true so the countdown timer doesn't go down again (no more lights to respond to)
                    done = true;
                }

            }
            else
            {
                //pre-emptive press of the button, show the error (fail) text on the screen (run the method that does this) & reset countdown timer
                StartCoroutine(ErrorText());
                countdownTimer = randomCountdown();
            }
        }
    }

    //concurrent method for displaying the error (fail) text and then disabling it after 2 seconds of showing
    public IEnumerator ErrorText()
    {
        //set error text
        errorText.SetText("Wait until the light turns red before pressing any button! Timer restarted for this attempt.");

        //wait 2 seconds
        yield return new WaitForSeconds(2);

        //reset error text to nothing
        errorText.SetText("");
    }

    /*
     * Method for turning the light on and off
     */ 
    public void changeLightState(int i)
    {
        //turn light on
        if (i == 0)
        {
            //make sphere more visible
            sphere.transform.position.Set(transform.position.x, (transform.position.y + 0.2f), transform.position.z);

            //change material of object from white to red
            sphere.GetComponent<MeshRenderer>().material = red;

            //enable the red lighting object
            lighting.GetComponent<Light>().enabled = true;

            //set boolean lighton to true
            lightOn = true;
        }
        else //turn light off
        {
            //make sphere less visible
            sphere.transform.position.Set(transform.position.x, (transform.position.y - 0.2f), transform.position.z);

            //change material of object from red to white
            sphere.GetComponent<MeshRenderer>().material = white;

            //disable the red lighting object
            lighting.GetComponent<Light>().enabled = false;

            //set boolean lighton to false
            lightOn = false;
        }
    }

    /*
     * Method for choosing a random number for the countdown - returns a float between 5 & 10 seconds (countdown until light turns on)
     */
    public float randomCountdown()
    {
        return Random.Range(5, 10);
    }

    /*
     * Method for resetting the countdown timer to a random ammount and the reaction timer to zero
     */ 
    public void resetTimers()
    {
        countdownTimer = randomCountdown();
        reactionTimer = 0f;
    }

    /*
     * Method for calculating the average (mean) of all the reaction times
     */
    public float getAverageControlTime()
    {
        //initialise total to zero
        int total = 0;

        //loops through all values and adds each time value to the total
        foreach (int time in times)
        {
            total += time;
        }

        //number of control times is 5, so the average returned will be the total divided by the number of values (5).
        return total / 5;
    }

    /*
     * Method for calculating the median of all the reaction times
     */
    public int getMedianControlTime()
    {
        //set variable sorted to false
        bool sorted = false;
        //create variable for holding a temporary integer
        int tempValue;

        //while loop that sorts the times in order of smallest to largest, it will keep looping until no unsorted values are found
        while (!sorted)
        {
            //set sorted to true, if there are unsorted values found then this will be set to false
            sorted = true;

            //loops through all the values 
            for (int i = 0; i < times.Length - 1; i++)
            {
                //checks if the current value is bigger than the next value in the array
                if (times[i] > times[i + 1])
                {
                    //if so, then switch the values of the 2 members of the array and set sorted to false
                    tempValue = times[i];
                    times[i] = times[i + 1];
                    times[i + 1] = tempValue;
                    sorted = false;
                }
            }
        }

        //number of control times is 5, so the median will be the 3rd number. Hence, the second index in the array.
        return times[2];
    }

}
