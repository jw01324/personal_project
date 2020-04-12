using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    private string userID = "";

    public TextMeshProUGUI idtext;
    public Slider slider;

    private void Start()
    {
        //if no user id given yet, then generate one.
        if (userID.Equals("") || userID == "")
        {
            userID = generateRandomUserID();
        }

        SceneData.userID = userID;

        idtext.SetText("User ID: " + userID);

        SceneData.satNavOrder = generateSatNavOrder();

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
        string id = "";

        for (int i = 0; i < 10; i++)
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
