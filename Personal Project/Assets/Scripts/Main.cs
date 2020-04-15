using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Main : MonoBehaviour
{
    public static int currentScene;
    private Car car;
    //private Car incidentCar;
    private static bool isTiming;
    private static float timer;
    private static bool done;
    private bool inMenu;
    private int attempts;
    private SatNav satnav;

    //public static FileManager fm = new FileManager();


    void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        isTiming = false;
        timer = 0;
        done = false;

        satnav = GameObject.FindGameObjectWithTag("SatNav").GetComponent<SatNav>();
        car = GameObject.FindGameObjectWithTag("Player").GetComponent<Car>();

        print(SceneData.dataToString());
    }

    void Update()
    {
        // checks if the scene has changed in which case to make sure that the scene is not set to done so that it runs correctly
        if(currentScene < SceneManager.GetActiveScene().buildIndex)
        {
            done = false;
        }

        currentScene = SceneManager.GetActiveScene().buildIndex;

        if (!done)
        {
            if (isTiming)
            {
                timer += Time.deltaTime;
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

        Result result = new Result(SceneManager.GetActiveScene().name, satnav.getSatNavType(), car.reactionTimes.ToArray(), car.correctReactions, car.incorrectReactions, satnav.correctAnswers, satnav.incorrectAnswers, car.crashed);
        print(result.toString(0));

        SceneData.results.Add(result);

        print(SceneData.dataToString());

        TextMeshProUGUI resultsText = GameObject.FindGameObjectWithTag("EndScreen").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        //TODO: Display result
        resultsText.SetText("DONE.\nHold both triggers to continue.");

    }

    public void restartScene()
    {
        Time.timeScale = 1f;
    }

    public void quit()
    {
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

    public void addResult(Result result)
    {

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
