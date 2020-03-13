using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Main : MonoBehaviour
{

    private Car player;
    //private Car incidentCar;
    private static bool isTiming;
    private static float timer;
    private static bool done;
    private bool inMenu;
    private int attempts;
    //public static FileManager fm = new FileManager();


    void Start()
    {
        isTiming = false;
        timer = 0;
        done = false;

    }

    void Update()
    {
        if (!done)
        {
            if (isTiming)
            {
                timer += Time.deltaTime;
            }
        }
       
    }

    public static void startTimer()
    {
        isTiming = true;
    }

    public static float stopTimer()
    {
        isTiming = false;
        float finalTime = timer;
        timer = 0;
        return finalTime;
    }

    public static float getTimer()
    {
        return timer;
    }

    public static bool getState()
    {
        return done;
    }

    public static void stopScene()
    {

        done = true;
        print("SCENE OVER");

        TextMeshProUGUI resultsText = GameObject.FindGameObjectWithTag("EndScreen").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        //TODO: Display result
        resultsText.SetText("DONE.\nHold both triggers to continue.");

    }

    public static void restartScene()
    {
        Time.timeScale = 1f;
    }

    public static void quit()
    {
        Application.Quit();
    }

    /*
     * Method for loading the scenes from the main menu
     */
    public static void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

    }

    public static void loadNextScene()
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

    public static void addResult(Result result)
    {

    }

    /**
    public static void saveTestResultToFile(Result result)
    {
        fm.createResultFile(result, 0);
    }

    public static void saveSceneResultToFile(Result result)
    { 
        fm.createResultFile(result, 1);
    }
    */
}
