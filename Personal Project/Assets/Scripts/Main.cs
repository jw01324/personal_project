using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{

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
}
