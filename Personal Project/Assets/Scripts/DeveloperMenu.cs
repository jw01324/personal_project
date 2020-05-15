using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/*
 * Class for a secret in-app menu that can be accessed for testing on the device, named DeveloperMenu
 */
public class DeveloperMenu : MonoBehaviour
{
    //GameObjects
    private Main main;
    private Toggle scene;
    private Toggle satnav;
    public ToggleGroup scenes;
    public ToggleGroup satnavs;

    // Start is called before the first frame update
    void Start()
    {
        //initialise variables
        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
    }

    /*
     * Method for loading the control scene, activated by button press using the onClick parameter of the button object
     */
    public void loadControlScene()
    {
        main.loadScene("ControlScene");
    }

    /*
     * Method for starting the test with the parameters selected, activated by button press using the onClick parameter of the button object
     */
    public void startTest()
    {    
        //setting the scene selected to the active toggle in the group for scenes
        foreach (var toggle in scenes.ActiveToggles())
        {
            scene = toggle;
            break;
        }

        //setting the satnav selected to the active toggle in the group for scenes
        foreach (var toggle in satnavs.ActiveToggles())
        {
            satnav = toggle;
            break;
        }

        //switch statement that sets the satnav type chosen for all scenes in the order array (incase the user wants to continue testing that type for all other scenes)
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

        //switch statement that converts the toggle number of the scene to the scene name and then loads + starts the selected scene
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

    /*
     * Method for loading the end scene, activated by button press using the onClick parameter of the button object
     */
    public void loadEndScene()
    {
        main.loadScene("EndScene");
    }

    /*
     * Method for quitting the application, activated by button press using the onClick parameter of the button object
     */
    public void quit()
    {
        main.quit();
    }

}
