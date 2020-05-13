using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Main : MonoBehaviour
{
    private Camera mainCamera;
    public static int currentScene;
    private Car car;
    private AudioSource carBackground;
    private AudioSource carEngine;
    private bool hasStarted;
    public bool isTiming;
    private static float timer;
    private bool done;
    private bool inMenu;
    private int attempts;
    private SatNav satnav;
    private GameObject startScreen;
    private GameObject endScreen;
    private TextMeshProUGUI startText;
    private TextMeshProUGUI resultsText;
    private Slider slider;

    //public static FileManager fm = new FileManager();


    void Start()
    {

        currentScene = SceneManager.GetActiveScene().buildIndex;
        hasStarted = false;
        isTiming = false;
        timer = 0;
        done = false;

        if (currentScene > 1 & currentScene < 6)
        {

            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            // only render objects in the UI Layer
            mainCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");

            satnav = GameObject.FindGameObjectWithTag("SatNav").GetComponent<SatNav>();
            car = GameObject.FindGameObjectWithTag("Player").GetComponent<Car>();
            carBackground = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
            carEngine = GameObject.FindGameObjectWithTag("Engine_SFX").GetComponent<AudioSource>();
            startScreen = GameObject.FindGameObjectWithTag("StartScreen");
            endScreen = GameObject.FindGameObjectWithTag("EndScreen");

            startScreen.transform.GetChild(4).GetComponent<TextMeshProUGUI>().SetText("Scene: " + SceneManager.GetActiveScene().name + "\nSatNav Type: " + satnav.getSatNavType());

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

            startText = startScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            startText.SetText(introduction);
            resultsText = endScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            slider = endScreen.transform.GetChild(1).GetComponent<Slider>();
            slider.maxValue = Controller.heldTime;

            for (int i = 0; i < endScreen.transform.childCount; i++)
            {
                endScreen.transform.GetChild(i).gameObject.SetActive(false);
            }

            print(SceneData.dataToString());
        }
    }

    void Update()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene > 1 & currentScene < 6)
        {
            if (Controller.inputsEnabled)
            {
                startScreen.transform.GetChild(3).gameObject.SetActive(true);
            }
        }

        if (!done)
        {
            if (isTiming)
            {
                timer += Time.deltaTime;
            }
        }
        else
        {
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

    public void startTimer()
    {
        isTiming = true;
    }

    public float stopTimer()
    {
        isTiming = false;
        float finalTime = timer;
        timer = 0;
        return finalTime;
    }

    public float getTimer()
    {
        return timer;
    }

    public bool getState()
    {
        return done;
    }

    public void stopScene()
    {

        done = true;
        print("SCENE OVER");

        Time.timeScale = 1;

        carBackground.Stop();
        carEngine.Stop();

        Result result = new Result(SceneManager.GetActiveScene().name, satnav.getSatNavType(), car.reactionTimes.ToArray(), car.correctReactions, car.incorrectReactions, satnav.correctAnswers, satnav.incorrectAnswers, car.crashed);
        print(result.toString());

        SceneData.results.Add(result);

        print(SceneData.dataToString());

        mainCamera.clearFlags = CameraClearFlags.SolidColor;

        // only render objects in the UI Layer
        mainCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");

        for (int i = 0; i < endScreen.transform.childCount; i++)
        {
            endScreen.transform.GetChild(i).gameObject.SetActive(true);
        }

        resultsText.SetText(result.toString());

    }

    public void restartScene()
    {
        Time.timeScale = 1f;
    }

    public void quit()
    {
        print("quitting application");
        Application.Quit();
    }

    /*
     * Method for loading the scenes from the main menu
     */
    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

    }

    public void loadNextScene()
    {
        //value of the next scenes index
        int index = SceneManager.GetActiveScene().buildIndex + 1;

        print(index);
        print(SceneManager.sceneCountInBuildSettings);

        //if there are more scenes left then load the next one
        if (index < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(index);
        }

    }

    public void startScene()
    {
        // render all the other layers in the camera
        for (int i = 0; i < 5; i++)
        {
            mainCamera.cullingMask += 1 << i;
        }

        mainCamera.clearFlags = CameraClearFlags.Skybox;

        startScreen.SetActive(false);
        carBackground.Play();
        carEngine.Play();

        satnav.onStart();
        hasStarted = true;
    }

    public bool getHasStarted()
    {
        return hasStarted;
    }

}
