using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main
{

    private Car player;
    //private Car incidentCar;
    private float timer;
    private bool done;
    private bool inMenu;
    private int attempts;
    public static FileManager fm = new FileManager();
    private Result result;

    

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

    public static void addResult()
    {

    }

    public static void saveTestResultToFile(Result result)
    {
        fm.createResultFile(result, 0);
    }

    public static void saveSceneResultToFile(Result result)
    { 
        fm.createResultFile(result, 1);
    }
}
