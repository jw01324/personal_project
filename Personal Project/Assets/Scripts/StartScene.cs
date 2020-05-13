using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Android;

public class StartScene : MonoBehaviour
{
    private string userID = "";

    public TextMeshProUGUI idtext;
    public Slider slider;

    private void Start()
    {

        //code to detect if I am testing the app on computer or on the oculus device (which is android)
        #if UNITY_EDITOR
            SceneData.isOnOculus = false;
            print("Editor");
        #elif UNITY_ANDROID
            SceneData.isOnOculus = true;
            print("Oculus");
        #endif

        //if no user id given yet, then generate one.
        if (userID.Equals("") || userID == "")
        {
            userID = generateRandomUserID();
        }

        SceneData.userID = userID;

        idtext.SetText("User ID: " + userID);

        SceneData.satNavOrder = generateSatNavOrder();

        //create a text file for test purposes
        FileManager.createDirectory();
        FileManager.createResultFile();


    }

    private void Update()
    {
        if(Controller.timer > 0)
        {
            slider.gameObject.SetActive(true);
            slider.maxValue = Controller.heldTime;
            slider.value = Controller.timer;

        }
        else
        {
            slider.gameObject.SetActive(false);
        }
        
    }


    public string generateRandomUserID()
    {
        string id = "SUR";

        for (int i = 0; i < 4; i++)
        {
            id += Random.Range(0, 10);
        }

        return id;
    }

    public int[] generateSatNavOrder()
    {
       
        int[] order = new int[4];

        //initialising the array so that a random number can replace it with (didn't initialise as 0 because one of the options is 0, hence 0 would always end up in the last slot in that case.
        order[0] = 10;
        order[1] = 10;
        order[2] = 10;
        order[3] = 10;

        for (int i = 0; i < 4; i++)
        {
            bool isNew = false;

            while (!isNew)
            {
                int number = Random.Range(0, 4);

                isNew = true;

                foreach (int n in order)
                {
                    if (number == n)
                    {
                        isNew = false;
                    }
                   
                }

                if (isNew)
                {
                    order[i] = number;
                }
            }

            print(order[i]);
        }

        return order;
    }
}
