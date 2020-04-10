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
    private bool isOverItem;
    private bool isLoadingScene;
    private bool inputsEnabled;

    private SatNav satnav;
    public static float timer;
    public static float heldTime = 2f;


    // Start is called before the first frame update
    void Start()
    {
        inputsEnabled = false;
        timer = 0;
        isTrackingOn = false;
        isOverItem = false;
        isLoadingScene = false;
        satnav = GameObject.FindGameObjectWithTag("SatNav").GetComponent<SatNav>();

        StartCoroutine(EnableInputs());
    }

    // Update is called once per frame
    void Update()
    {
        if (Main.currentScene == 0)
        {
            if (!oculusInputs)
            {
                if (Input.GetKey(KeyCode.Space))
                {

                    timer += Time.deltaTime;

                    if (timer >= heldTime)
                    {
                        isLoadingScene = true;
                        //load next scene
                        Main.loadNextScene();
                    }
                }
                else
                {
                    timer = 0;
                }


            }
            else
            {
                //require specific input to continue to next scene (both triggers held down)
                if (OVRInput.Get(responseButton) & OVRInput.Get(selectionButton))
                {

                    timer += Time.deltaTime;

                    if (timer >= heldTime)
                    {
                        //load next scene
                        Main.loadNextScene();
                    }
                }
                else
                {
                    timer = 0;
                }

                //holding down A, B, and the left trigger all at once
                if (OVRInput.Get(OVRInput.Button.One) & OVRInput.Get(OVRInput.Button.Two) & OVRInput.Get(selectionButton))
                {
                    //take to the secret menu (used only by the developer for testing purposes)
                    Main.loadScene("MainMenu");
                }
            }
        }
        else
        {
            if (!Main.getState() & inputsEnabled)
            {
                if (!oculusInputs)
                {
                    /*
                     * Controls for changing the time scale in testing (to quickly get to a specific point)
                     */

                    if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        Time.timeScale = Time.timeScale * 2f;
                        print(Time.timeScale);
                    }

                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        Time.timeScale = Time.timeScale / 2f;
                        print(Time.timeScale);
                    }



                    /*
                     * Inputs for scene
                     */

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
                    /*
                     * Inputs for scene
                     */

                    //right trigger pressed
                    if (OVRInput.GetDown(responseButton))
                    {
                        GameObject.FindGameObjectWithTag("Player").GetComponent<Car>().stop();
                    }

                    //left trigger pressed
                    if (OVRInput.GetDown(selectionButton))
                    {
                        if (isTrackingOn & isOverItem)
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

            else if (Main.getState())
            {
                if (!oculusInputs)
                {
                    if (Input.GetKey(KeyCode.Space) & !isLoadingScene)
                    {

                        timer += Time.deltaTime;

                        if (timer >= heldTime)
                        {
                            isLoadingScene = true;
                            //load next scene
                            Main.loadNextScene();
                        }
                    }
                    else
                    {
                        timer = 0;
                    }
                }
                else
                {
                    //require specific input to continue to next scene (both triggers held down)
                    if (OVRInput.Get(responseButton) & OVRInput.Get(selectionButton))
                    {

                        timer += Time.deltaTime;

                        if (timer >= heldTime)
                        {
                            //load next scene
                            Main.loadNextScene();
                        }
                    }
                    else
                    {
                        timer = 0;
                    }
                }

            }
        }

    }

    /*
     * method for enabling inputs after a second, so that when the scene changes it doesn't automatically pick up your input to change scene
     */
    IEnumerator EnableInputs()
    {
        yield return new WaitForSeconds(1);
        inputsEnabled = true;
        print("inputs now enabled");
    }


}
