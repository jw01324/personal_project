using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
   
    public void loadControlScene()
    {
        Main.loadScene("ControlScene");
    }

    public void startTest()
    {
        //TODO: add method for starting test (scenes 1 - 4 with mixed variables)
        Main.loadScene("Scene1");
    }

    public void sceneSelection()
    {
        //TODO: add canvas for scene selection and make this method change the current canvas to that one
    }

    public void quit()
    {
        Main.quit();
    }
}
