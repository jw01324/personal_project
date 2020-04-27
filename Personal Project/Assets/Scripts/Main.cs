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
    private bool hasStarted;
    private static bool isTiming;
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
            startScreen = GameObject.FindGameObjectWithTag("StartScreen");
            endScreen = GameObject.FindGameObjectWithTag("EndScreen");

            startScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("Scene: " + SceneManager.GetActiveScene().name + "\nSatNav Type: " + satnav.getSatNavType());

            string introduction = "This is a car driving simulation. The car will drive by itself, following the car in front, all you need to do is:" +
                "\n- Press the right trigger when you spot the car in front braking. This will happen on multiple occasions during the scene.\n";

            switch (satnav.intType)
            {
                case (0): //audio type
                    introduction += "- Move the left trigger in the direction that the SatNav says. This will be an audio cue, you will have 3 seconds to react to each cue, there will be no visual indicator.\n";
                    break;
                case (1): //visual type
                    introduction += "- Move the left trigger in the direction that the SatNav displays. This will be a visual cue on the SatNav display in the car, you will have 3 seconds to react to each cue, there will be no audio indicator.\n";
                    break;
                case (2): //audiovisual type
                    introduction += "- Move the left trigger in the direction that the SatNav displays. This will be a visual and audio cue on the SatNav display in the car, you will have 3 seconds to react to each cue.\n";
                    break;
                case (3): //programmable type
                    introduction += "- Aim the left controller at the virtual keyboard and type in the word displayed on the SatNav, clicking the 'enter' button on the keyboard to submit the word. If you are correct, another word will be displayed. If you are incorrect, please retype the word.\n";
                    break;
            }

            startText = startScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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
                startScreen.transform.GetChild(1).gameObject.SetActive(true);
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

        Result result = new Result(SceneManager.GetActiveScene().name, satnav.getSatNavType(), car.reactionTimes.ToArray(), car.correctReactions, car.incorrectReactions, satnav.correctAnswers, satnav.incorrectAnswers, car.crashed);
        print(result.toString(0));

        SceneData.results.Add(result);

        print(SceneData.dataToString());

        mainCamera.clearFlags = CameraClearFlags.SolidColor;

        // only render objects in the UI Layer
        mainCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");

        for (int i = 0; i < endScreen.transform.childCount; i++)
        {
            endScreen.transform.GetChild(i).gameObject.SetActive(true);
        }

        resultsText.SetText(result.toString(0));

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
        else
        {
            //TODO: add an end scene? graphic with results and option to restart?
            print("no more scenes");
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

        satnav.onStart();
        hasStarted = true;
    }

    public bool getHasStarted()
    {
        return hasStarted;
    }


    /**
    public void saveTestResultToFile(Result result)
    {
        fm.createResultFile(result, 0);
    }

    public void saveSceneResultToFile(Result result)
    { 
        fm.createResultFile(result, 1);
    }
    */
}
