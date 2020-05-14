using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/*
 * Class for is used to control the start and stop of the scene, and many general variables that are used.
 */
public class Main : MonoBehaviour
{
    //Variables
    public static int currentScene;
    private bool hasStarted;
    public bool isTiming;
    private static float timer;
    private bool done;

    //GameObjects
    private Camera mainCamera;
    private Car car;
    private SatNav satnav;
    private AudioSource backgroundNoise;

    //UI
    private GameObject startScreen;
    private GameObject endScreen;
    private TextMeshProUGUI startText;
    private TextMeshProUGUI resultsText;
    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        //initialising general variables
        currentScene = SceneManager.GetActiveScene().buildIndex;
        hasStarted = false;
        isTiming = false;
        timer = 0;
        done = false;

        //checks if the current scene is 2,3,4, or 5 which are the scenes which involve the cars and the satnav, therefore need to initialise the variables needed for those scenes.
        if (currentScene > 1 & currentScene < 6)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            // only render objects in the UI Layer
            mainCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");

            satnav = GameObject.FindGameObjectWithTag("SatNav").GetComponent<SatNav>();
            car = GameObject.FindGameObjectWithTag("Player").GetComponent<Car>();
            backgroundNoise = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
            startScreen = GameObject.FindGameObjectWithTag("StartScreen");
            endScreen = GameObject.FindGameObjectWithTag("EndScreen");

            startScreen.transform.GetChild(4).GetComponent<TextMeshProUGUI>().SetText("Scene: " + SceneManager.GetActiveScene().name + "\nSatNav Type: " + satnav.getSatNavType());

            //start screen text that changes depending on the satnav type
            string introduction = "This is a car driving simulation. The car will drive by itself, following the car in front, all you need to do is:" +
                "\n- Press the A or B button on the Right Controller when you spot the car in front braking. This will happen on multiple occasions during the scene.\n";

            switch (satnav.intType)
            {
                case (0): //audio type
                    introduction += "- Move the left thumbstick in the direction that the SatNav says. This will be an audio cue, you will have 3 seconds to react to each cue, there will be no visual indicator.\n";
                    break;
                case (1): //visual type
                    introduction += "- Move the left thumbstick in the direction that the SatNav displays. This will be a visual cue on the SatNav display in the car, you will have 3 seconds to react to each cue, there will be no audio indicator.\n";
                    break;
                case (2): //audiovisual type
                    introduction += "- Move the left thumbstick in the direction that the SatNav displays. This will be a visual and audio cue from the SatNav display in the car, you will have 3 seconds to react to each cue.\n";
                    break;
                case (3): //programmable type
                    introduction += "- Aim the left controller at the virtual keyboard and press the left trigger or the right trigger to type in characters on the keyboard. Once you have typed the postcode displayed on the SatNav, click the 'submit' button on the keyboard to submit the word. If you are correct, another word will be displayed. If you are incorrect, please retype the word. You can use the backspace key to delete characters.\n";
                    break;
            }

            //show the correct controls depending on the satnav type
            if(satnav.intType != 3)
            {
                startScreen.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                startScreen.transform.GetChild(0).gameObject.SetActive(false);
            }

            //set values created to the UI
            startText = startScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            startText.SetText(introduction);
            resultsText = endScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            slider = endScreen.transform.GetChild(1).GetComponent<Slider>();
            slider.maxValue = Controller.heldTime;

            //disable the end screen UI for now, it will be enabled when the scene ends
            for (int i = 0; i < endScreen.transform.childCount; i++)
            {
                endScreen.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //checks if a car scene is the current scene
        if (currentScene > 1 & currentScene < 6)
        {
            //once the controller inputs are enabled (2 seconds after the start screen is shown) enable the text that tells the user they can continue
            if (Controller.inputsEnabled)
            {
                startScreen.transform.GetChild(3).gameObject.SetActive(true);
            }
        }

        //checks if the scene is done
        if (!done)
        {
            //if scene not done, and timer is active, then increase the value of the timer by the time that has occurred since the last frame (same as time passed in real life in seconds)
            if (isTiming)
            {
                timer += Time.deltaTime;
            }
        }
        else
        {
            //if the scene is done and the timer has increased then the user is holding the button to continue to the next scene (controls change when done = true)
            if (Controller.timer > 0)
            {
                //displays the held time using a slider in the UI
                slider.gameObject.SetActive(true);
                slider.value = Controller.timer;

            }
            else
            {
                //disables the slider that shows the progress of the users button press when timer = 0 (or less than 0)
                slider.gameObject.SetActive(false);
            }
        }
    }

    /*
     * Method for starting the timer
     */
    public void startTimer()
    {
        isTiming = true;
    }

    /*
     * Method for stopping the timer - returns the reaction time
     */
    public float stopTimer()
    {
        isTiming = false;
        float finalTime = timer;
        timer = 0;
        return finalTime;
    }

    /*
     * Method for getting the float value of the timer (time in seconds)
     */ 
    public float getTimer()
    {
        return timer;
    }

    /*
     * Method for getting the state of the scene (value of done boolean)
     */
    public bool getState()
    {
        return done;
    }

    /*
     * Method for stopping the scene and performing every function needed at the end (saving data, stopping audio, etc.)
     */
    public void stopScene()
    {
        //set done to true
        done = true;

        //set timescale to 1, this is needed because when testing in the editor I often speed up the scene in order to waste less time testing
        Time.timeScale = 1;

        //stops the background noise
        backgroundNoise.Stop();

        //creates result object with all the data collected in the scene
        Result result = new Result(SceneManager.GetActiveScene().name, satnav.getSatNavType(), car.reactionTimes.ToArray(), car.correctReactions, car.incorrectReactions, satnav.correctAnswers, satnav.incorrectAnswers, car.crashed);

        //adds result to the scenedata class (which holds data persistantly between all scenes)
        SceneData.results.Add(result);

        //if it is the last driving scene then save the results file
        if (currentScene + 1 == 6)
        {
            //create a folder to hold the results file
            FileManager.createDirectory();

            //create a text file with the results that will be saved in the folder
            FileManager.createResultFile();
        }

        //set camera flag to show solid colour instead of the skybox (this is so the end screen UI looks better)
        mainCamera.clearFlags = CameraClearFlags.SolidColor;

        //only render objects in the UI Layer so that the menu can be the only thing displayed
        mainCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");

        //enable all of the components of the end screen UI
        for (int i = 0; i < endScreen.transform.childCount; i++)
        {
            endScreen.transform.GetChild(i).gameObject.SetActive(true);
        }

        //set the text component of the UI to the result object created in string format (to show the results on screen)
        resultsText.SetText(result.toString());
    }

    /*
     * Method for quitting the application completely
     */
    public void quit()
    {
        print("quitting application");
        Application.Quit();
    }

    /*
     * Method for loading specific scenes
     */
    public void loadScene(string sceneName)
    {
        //loading scene by string
        SceneManager.LoadScene(sceneName);
    }

    /*
     * Method for loading the next scene in the scene directory (e.g: 1 -> 2, or 2 -> 3)
     */
    public void loadNextScene()
    {
        //value of the index of the next scene
        int index = SceneManager.GetActiveScene().buildIndex + 1;

        //if there are more scenes left in the build then load the next scene
        if (index < SceneManager.sceneCountInBuildSettings)
        {
            //loading scene by index number
            SceneManager.LoadScene(index);
        }
    }

    /*
     * Method for starting the car part of the scene (after the start menu has gone)
     */
    public void startScene()
    {
        //render all the other layers in the camera (UI was already loaded at the start)
        for (int i = 0; i < 5; i++)
        {
            mainCamera.cullingMask += 1 << i;
        }

        //set the flag for the camera back to skybox
        mainCamera.clearFlags = CameraClearFlags.Skybox;

        //disable the start UI
        startScreen.SetActive(false);

        //start the background noise
        backgroundNoise.Play();

        //activate the satnav
        satnav.onStart();

        //set hasstarted to true
        hasStarted = true;
    }

    /*
     * Method for getting the value of the boolean hasstarted
     */
    public bool getHasStarted()
    {
        return hasStarted;
    }

}
