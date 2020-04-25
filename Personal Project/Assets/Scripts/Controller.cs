using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    //whether I am using my keyboard in testing mode or whether the inputs are for the oculus
    private bool oculusInputs;

    private OVRInput.Button selectionButton = OVRInput.Button.PrimaryIndexTrigger;
    private OVRInput.Button responseButton = OVRInput.Button.SecondaryIndexTrigger;
    private OVRInput.Button left = OVRInput.Button.PrimaryThumbstickLeft;
    private OVRInput.Button right = OVRInput.Button.PrimaryThumbstickRight;
    private OVRInput.Button up = OVRInput.Button.PrimaryThumbstickUp;

    private Main main;

    private bool isTrackingOn;
    private bool isOverItem;
    private bool isLoadingScene;
    public static bool inputsEnabled;

    private SatNav satnav;
    public static float timer;
    private float developerTimer;
    public static float heldTime = 2f;


    // Start is called before the first frame update
    void Start()
    {
        //code to detect if I am testing the app on computer or on the oculus device (which is android)
        #if UNITY_EDITOR
                oculusInputs = false;
                print("Editor");
        #elif UNITY_ANDROID
                oculusInputs = true;
                print("Oculus");
        #endif

        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

        inputsEnabled = false;
        developerTimer = 0;
        timer = 0;
        isTrackingOn = false;
        isOverItem = false;
        isLoadingScene = false;

        if (GameObject.FindGameObjectWithTag("SatNav") != null)
        {
            satnav = GameObject.FindGameObjectWithTag("SatNav").GetComponent<SatNav>();
        }

        StartCoroutine(EnableInputs());
    }

    // Update is called once per frame
    void Update()
    {
        //the developer menu needs to be accessible from any scene so it goes outside of the switch statement
        if (oculusInputs)
        {
            //holding down B, C, left trigger, and right hand trigger all at once (an input that is very unlikely for a person to do)
            if (OVRInput.Get(OVRInput.Button.Three) & OVRInput.Get(OVRInput.Button.Two) & OVRInput.Get(selectionButton) & OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                developerTimer += Time.deltaTime;

                if (developerTimer >= heldTime)
                {
                    //take to the secret menu (used only by the developer for testing purposes)
                    main.loadScene("DeveloperMenu");
                }

            }
            else
            {
                developerTimer = 0;
            }
        }

        switch (Main.currentScene)
        {
            // start screen
            case (0):
                
                if (!oculusInputs)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {

                        timer += Time.deltaTime;

                        if (timer >= heldTime)
                        {
                            isLoadingScene = true;
                            //load next scene
                            main.loadNextScene();
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
                            main.loadNextScene();
                        }
                    }
                    else
                    {
                        timer = 0;
                    }

                }
                break;
            
            // control scene
            case (1):

                if (inputsEnabled)
                {
                    if (!oculusInputs)
                    {
                        if (ControlScene.done)
                        {
                            if (Input.GetKey(KeyCode.Space))
                            {
                                timer += Time.deltaTime;

                                if (timer >= heldTime)
                                {
                                    isLoadingScene = true;
                                    //load next scene
                                    main.loadNextScene();
                                }
                            }
                            else
                            {
                                timer = 0;
                            }
                        }
                        else
                        {
                            if (Input.GetKeyDown(KeyCode.Space))
                            {
                                GameObject.FindGameObjectWithTag("Player").GetComponent<ControlScene>().userReacts();
                            }

                        }


                    }
                    else
                    {
                        if (ControlScene.done)
                        {
                            //require specific input to continue to next scene (both triggers held down)
                            if (OVRInput.Get(responseButton) & OVRInput.Get(selectionButton))
                            {

                                timer += Time.deltaTime;

                                if (timer >= heldTime)
                                {
                                    //load next scene
                                    main.loadNextScene();
                                }
                            }
                            else
                            {
                                timer = 0;
                            }
                        }
                        else
                        {
                            if (OVRInput.GetDown(responseButton))
                            {
                                GameObject.FindGameObjectWithTag("Player").GetComponent<ControlScene>().userReacts();
                            }
                        }

                    }
                }
                break;

            // scene 1, 2, 3, or 4
            case (2):
            case (3):
            case (4):
            case (5):

                if (!main.getState() & inputsEnabled)
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
                            if (!main.getHasStarted())
                            {
                                main.startScene();
                            }
                            else
                            {
                                GameObject.FindGameObjectWithTag("Player").GetComponent<Car>().stop();
                            }
                        }

                        //only use the directions as inputs if the satnav is directional (otherwise uses mouse for programmable satnav type in testing)
                        if (satnav.intType != 3)
                        {
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
                    }
                    else
                    {
                        /*
                         * Inputs for scene
                         */

                        //right trigger pressed
                        if (OVRInput.GetDown(responseButton))
                        {
                            if (!main.getHasStarted())
                            {
                                main.startScene();
                            }
                            else
                            {
                                GameObject.FindGameObjectWithTag("Player").GetComponent<Car>().stop();
                            }
                        }


                        //only use the directions as inputs if the satnav is directional (otherwise uses left controller input for programmable satnav type)
                        if (satnav.intType != 3)
                        {
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
                        else
                        {
                            //left trigger pressed
                            if (OVRInput.GetDown(selectionButton))
                            {
                                if (isOverItem)
                                {
                                    //TODO: interact with keyboard
                                }
                            }
                        }
                    }
                }
                else if (main.getState())
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
                                main.loadNextScene();
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
                                main.loadNextScene();
                            }
                        }
                        else
                        {
                            timer = 0;
                        }
                    }

                }
                break;
            
            //case 6 which is the end screen
            case (6):
                if (oculusInputs)
                {
                    //left trigger pressed
                    if (OVRInput.GetDown(selectionButton))
                    {
                        if (isOverItem)
                        {
                            //TODO: interact with button
                        }
                    }
                }
                break;

        }

    }
       

    /*
     * method for enabling inputs after a second, so that when the scene changes it doesn't automatically pick up your input to change scene
     */
    IEnumerator EnableInputs()
    {
        yield return new WaitForSeconds(2);
        inputsEnabled = true;
        print("inputs now enabled");
    }


}
