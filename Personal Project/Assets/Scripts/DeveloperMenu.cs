using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DeveloperMenu : MonoBehaviour
{
    private Toggle scene;
    private Toggle satnav;
    public ToggleGroup scenes;
    public ToggleGroup satnavs;
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
        //setting the toggle to the active toggle in the group for scenes and satnavs

        foreach (var toggle in scenes.ActiveToggles())
        {
            scene = toggle;
            break;
        }

        foreach (var toggle in satnavs.ActiveToggles())
        {
            satnav = toggle;
            break;
        }

        //set satnav order to the type selected for each scene
        switch (satnav.name)
        {
            case ("0"):
                SceneData.satNavOrder = new int[] { 0, 0, 0, 0 };
                break;
            case ("1"):
                SceneData.satNavOrder = new int[] { 1, 1, 1, 1 };
                break;
            case ("2"):
                SceneData.satNavOrder = new int[] { 2, 2, 2, 2 };
                break;
            case ("3"):
                SceneData.satNavOrder = new int[] { 3, 3, 3, 3 };
                break;
        }

        //start the selected scene
        switch (scene.name)
        {
            case ("0"):
                main.loadScene("Scene1");
                break;
            case ("1"):
                main.loadScene("Scene2");
                break;
            case ("2"):
                main.loadScene("Scene3");
                break;
            case ("3"):
                main.loadScene("Scene4");
                break;
        }
    }

    public void loadEndScene()
    {
        main.loadScene("EndScene");
    }

    public void quit()
    {
        main.quit();
    }
}
