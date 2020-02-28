using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Main : MonoBehaviour
{

    private Car player;
    //private Car incidentCar;
    public TextMeshProUGUI feedbackText;
    private static bool isTiming;
    private static float timer;
    private bool done;
    private bool inMenu;
    private int attempts;
    //public static FileManager fm = new FileManager();


    void Start()
    {
        isTiming = false;
        timer = 0;
    }

    void Update()
    {
        if (isTiming)
        {
            timer += Time.deltaTime;
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

    public static void stopScene()
    {
        Time.timeScale = 0;
        //TODO: bring up menu
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
