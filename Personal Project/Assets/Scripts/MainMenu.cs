using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public static string userID = "";

    public TextMeshProUGUI idtext;

    private void Start()
    {
        //if no user id given yet, then generate one.
        if (userID.Equals("") || userID == "")
        {
            userID = generateRandomUserID();
        }

        idtext.SetText("User ID: " + userID);
    }


    public string generateRandomUserID()
    {
        string id = "";

        for(int i = 0; i < 10; i++){
            id  += Random.Range(0, 10);
        }

        return id;
    }
   
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
