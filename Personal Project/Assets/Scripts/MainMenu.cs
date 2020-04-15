using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{

    private Main main;

    private void Start()
    {
        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
    }

    public void loadControlScene()
    {
        main.loadScene("ControlScene");
    }

    public void startTest()
    {
        //TODO: add method for starting test (scenes 1 - 4 with mixed variables)
        main.loadScene("Scene1");
    }

    public void sceneSelection()
    {
        //TODO: add canvas for scene selection and make this method change the current canvas to that one
    }

    public void quit()
    {
        main.quit();
    }
}
