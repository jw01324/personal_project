using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Controller : MonoBehaviour
{
    public TextMeshProUGUI text;

    private OVRInput.Button selectionButton = OVRInput.Button.PrimaryIndexTrigger;
    private OVRInput.Button responseButton = OVRInput.Button.SecondaryIndexTrigger;
    private OVRInput.Button left = OVRInput.Button.PrimaryThumbstickLeft;
    private OVRInput.Button right = OVRInput.Button.PrimaryThumbstickRight;
    private OVRInput.Button up = OVRInput.Button.PrimaryThumbstickUp;

    private bool isTrackingOn;
    private bool isOverMenuItem;

    // Start is called before the first frame update
    void Start()
    {
        isTrackingOn = false;
        isOverMenuItem = false;
    }

    // Update is called once per frame
    void Update()
    {
        //right trigger pressed
        if (OVRInput.GetDown(responseButton))
        {
            text.SetText("right trigger");

            GameObject.FindGameObjectWithTag("Player").GetComponent<Car>().stop();
        }

        //left trigger pressed
        if (OVRInput.GetDown(selectionButton))
        {
            text.SetText("left trigger");

            if (isTrackingOn & isOverMenuItem)
            {
                //TODO: interact with menu
            }
        }

        if (OVRInput.GetDown(up))
        {
            text.SetText("up on thumbstick");
        }

        if (OVRInput.GetDown(left))
        {
            text.SetText("left on thumbstick");
        }

        if (OVRInput.GetDown(right))
        {
            text.SetText("right on thumbstick");
        }

    }


}
