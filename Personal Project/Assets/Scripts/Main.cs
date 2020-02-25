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
    private bool isTiming;
    private float timer;
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

    public void startTimer()
    {
        isTiming = true;
    }

    public void stopTimer()
    {
        isTiming = false;

        //Result result = new Result(SceneManager.GetActiveScene().name, );
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
