using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    //whether I am using my keyboard in testing mode or whether the inputs are for the oculus
    public bool oculusInputs;

    private OVRInput.Button selectionButton = OVRInput.Button.PrimaryIndexTrigger;
    private OVRInput.Button responseButton = OVRInput.Button.SecondaryIndexTrigger;
    private OVRInput.Button left = OVRInput.Button.PrimaryThumbstickLeft;
    private OVRInput.Button right = OVRInput.Button.PrimaryThumbstickRight;
    private OVRInput.Button up = OVRInput.Button.PrimaryThumbstickUp;

    private bool isTrackingOn;
    private bool isOverMenuItem;

    private SatNav satnav;
    

    // Start is called before the first frame update
    void Start()
    {
        isTrackingOn = false;
        isOverMenuItem = false;
        satnav = GameObject.FindGameObjectWithTag("SatNav").GetComponent<SatNav>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!oculusInputs)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Car>().stop();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                //left
                satnav.checkInputDirectionCorrect(0);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                //right
                satnav.checkInputDirectionCorrect(1);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                //up
                satnav.checkInputDirectionCorrect(2);
            }
        }
        else
        {
            //right trigger pressed
            if (OVRInput.GetDown(responseButton))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Car>().stop();
            }

            //left trigger pressed
            if (OVRInput.GetDown(selectionButton))
            {
                if (isTrackingOn & isOverMenuItem)
                {
                    //TODO: interact with menu
                }
            }

            if (OVRInput.GetDown(left))
            {
                //left
                satnav.checkInputDirectionCorrect(0);
            }

            if (OVRInput.GetDown(right))
            {
                //right
                satnav.checkInputDirectionCorrect(1);
            }

            if (OVRInput.GetDown(up))
            {
                //up
                satnav.checkInputDirectionCorrect(2);
            }
        }

    }


}
