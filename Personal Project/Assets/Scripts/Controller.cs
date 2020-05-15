using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Class for recognising inputs, and telling the classes what that input means depending on the circumstances currently in the scene (whether the user is at a menu or if they are in the middle of the scene).
 */
public class Controller : MonoBehaviour
{
    //Controls
    //either A or B can be used to respond to events
    private OVRInput.Button responseButtonA = OVRInput.Button.One;
    private OVRInput.Button responseButtonB = OVRInput.Button.Two;
    //thumbstick directions for the directional satnav inputs
    private OVRInput.Button left = OVRInput.Button.PrimaryThumbstickLeft;
    private OVRInput.Button right = OVRInput.Button.PrimaryThumbstickRight;
    private OVRInput.Button up = OVRInput.Button.PrimaryThumbstickUp;

    //Variables
    private bool isLoadingScene;
    public static bool inputsEnabled;
    public static float timer;
    private float developerTimer;
    public static float heldTime = 2f;

    //GameObjects
    private Main main;
    private SatNav satnav;

    // Start is called before the first frame update
    void Start()
    {
        //initialise the main gameobject (that controls the scene state)
        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

        //initialise variables
        inputsEnabled = false;
        developerTimer = 0;
        timer = 0;
        isLoadingScene = false;

        //checks if there is a satnav in the scene, and if so initialises that variable
        if (GameObject.FindGameObjectWithTag("SatNav") != null)
        {
            satnav = GameObject.FindGameObjectWithTag("SatNav").GetComponent<SatNav>();
        }

        //starts the concurrent method for enabling the users inputs
        StartCoroutine(EnableInputs());
    }

    // Update is called once per frame
    void Update()
    {
        //This checks if the application is playing on the oculus (on android) rather than in the editor, then checks for a specific button combination in order to open the developer menu for quick testing on the device
        if (SceneData.isOnOculus)
        {
            //holding down C, left trigger, and right hand trigger all at once (an input that is very unlikely for a person to do) to open the developer menu
            if (OVRInput.Get(OVRInput.Button.Three) & OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) & OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                //increase timer by the time elapsed since the last frame (this a different timer variable so that the menu can be opened from the end menu which uses the other timer to keep track of when the user is ready)
                developerTimer += Time.deltaTime;

                //if this sequence of buttons has been held for the ammount of time needed, then load the developer menu scene (this gives the developer access to starting any scene with any satnav type)
                if (developerTimer >= heldTime)
                {
                    //load the scene specified by the string
                    main.loadScene("DeveloperMenu");
                }

            }
            else
            {
                //if the user lets go of the specific input then return the timer to zero 
                developerTimer = 0;
            }
        }

        //Switch statement that checks what scene the user is on (by index value), and then switches what the controls do depending on the scene and the specific moment in that scene (e.g. response button turns into the button to hold to continue to the next scene when the scene is over..)
        switch (Main.currentScene)
        {
            //start screen (index 0)
            case (0):
                
                //checks if application is running on the oculus (android) or in the editor (pc)
                if (!SceneData.isOnOculus)
                {
                    //for pc, hold the space key to continue to the next scene
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
                        //if let go of key, reset timer
                        timer = 0;
                    }
                }
                else
                {
                    //for oculus, hold either response buttons to continue
                    if (OVRInput.Get(responseButtonA) | OVRInput.Get(responseButtonB))
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
                        //if let go of button, reset timer
                        timer = 0;
                    }
                }
                break;
            
            //control scene (index 1)
            case (1):

                //checks if inputs have been enabled (start screen has finished)
                if (inputsEnabled)
                {
                    //checks if application is running on the oculus (android) or in the editor (pc)
                    if (!SceneData.isOnOculus)
                    {
                        //checks if the scene is already done
                        if (ControlScene.done)
                        {
                            //for pc, hold the space key to continue to the next scene
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
                                //if let go of key, reset timer
                                timer = 0;
                            }
                        }
                        else
                        {
                            //if scene not done, then the space button will be the react button for the user
                            if (Input.GetKeyDown(KeyCode.Space))
                            {
                                //run method for user reacting to the stimulus
                                GameObject.FindGameObjectWithTag("Player").GetComponent<ControlScene>().userReacts();
                            }

                        }
                    }
                    else
                    {
                        //checks if the scene is already done
                        if (ControlScene.done)
                        {
                            //for oculus, hold either response buttons to continue
                            if (OVRInput.Get(responseButtonA) | OVRInput.Get(responseButtonB))
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
                                //if let go of button, reset timer
                                timer = 0;
                            }
                        }
                        else
                        {
                            //if scene not done, then the response buttons will be the react button for the user
                            if (OVRInput.GetDown(responseButtonA) | OVRInput.GetDown(responseButtonB))
                            {
                                //run method for user reacting to the stimulus
                                GameObject.FindGameObjectWithTag("Player").GetComponent<ControlScene>().userReacts();
                            }
                        }

                    }
                }
                break;

            //car scenes (index 1, 2, 3, or 4)
            case (2):
            case (3):
            case (4):
            case (5):

                //if the scene isn't done, and inputs have been enabled (start screen has finished)
                if (!main.getState() & inputsEnabled)
                {
                    //checks if application is running on the oculus (android) or in the editor (pc)
                    if (!SceneData.isOnOculus)
                    {
                        /*
                         * Controls for changing the time scale in the editor (to quickly get to a specific point in the scene or to speed up troubleshooting)
                         */

                        if (Input.GetKeyDown(KeyCode.Alpha2))
                        {
                            //doubles timescale
                            Time.timeScale = Time.timeScale * 2f;
                            print(Time.timeScale);
                        }

                        if (Input.GetKeyDown(KeyCode.Alpha1))
                        {
                            //halves timescale
                            Time.timeScale = Time.timeScale / 2f;
                            print(Time.timeScale);
                        }

                        /*
                         * Inputs for scene
                         */
                        
                        //for pc, checks if the spacebar has been pressed
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            //if not already started, then start the scene
                            if (!main.getHasStarted())
                            {
                                main.startScene();
                            }
                            else
                            {
                                //if the scene has started, then this input will be used as the reaction button for the user
                                GameObject.FindGameObjectWithTag("Player").GetComponent<Car>().stop();
                            }
                        }

                        //only use the directions as inputs if the satnav is directional (0, 1, and 2 are directional - 3 is programmable)
                        if (satnav.intType != 3)
                        {
                            //for pc, 'a' button is left
                            if (Input.GetKeyDown(KeyCode.A))
                            {
                                //runs method for satnav to check whether the direction inputted was correct (0 denotes left for the method)
                                satnav.checkInputDirectionCorrect(0);
                            }

                            //for pc, 'd' button is right
                            if (Input.GetKeyDown(KeyCode.D))
                            {
                                //runs method for satnav to check whether the direction inputted was correct (1 denotes right for the method)
                                satnav.checkInputDirectionCorrect(1);
                            }

                            //for pc, 'w' button is forward
                            if (Input.GetKeyDown(KeyCode.W))
                            {
                                //runs method for satnav to check whether the direction inputted was correct (2 denotes forward for the method)
                                satnav.checkInputDirectionCorrect(2);
                            }
                        }
                    }
                    else
                    {
                        /*
                         * Inputs for scene
                         */

                        //for oculus, checks if either response button has been pressed
                        if (OVRInput.GetDown(responseButtonA) | OVRInput.GetDown(responseButtonB))
                        {
                            //if the scene hasn't started, the button press starts the scene
                            if (!main.getHasStarted())
                            {
                                main.startScene();
                            }
                            else
                            {
                                //if the scene has started then this input will act as the reaction button for the user
                                GameObject.FindGameObjectWithTag("Player").GetComponent<Car>().stop();
                            }
                        }

                        //only use the directions as inputs if the satnav is directional (0, 1, and 2 are directional - 3 is programmable)
                        if (satnav.intType != 3)
                        {
                            //for oculus, pressing the joystick to the left is classed as left
                            if (OVRInput.GetDown(left))
                            {
                                //runs method for satnav to check whether the direction inputted was correct (0 denotes left for the method)
                                satnav.checkInputDirectionCorrect(0);
                            }

                            //for oculus, pressing the joystick to the right is classed as right
                            if (OVRInput.GetDown(right))
                            {
                                //runs method for satnav to check whether the direction inputted was correct (1 denotes right for the method)
                                satnav.checkInputDirectionCorrect(1);
                            }

                            //for oculus, pressing the joystick upwards is classed as forward
                            if (OVRInput.GetDown(up))
                            {
                                //runs method for satnav to check whether the direction inputted was correct (2 denotes forward for the method)
                                satnav.checkInputDirectionCorrect(2);
                            }
                        }
                    }
                }
                else if (main.getState()) //if scene if finished
                {
                    //checks if application is running on the oculus (android) or in the editor (pc)
                    if (!SceneData.isOnOculus)
                    {
                        //for pc, hold the space key to continue to the next scene
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
                            //if let go, reset timer
                            timer = 0;
                        }
                    }
                    else
                    {
                        //for oculus, hold either response button to continue to the next scene
                        if (OVRInput.Get(responseButtonA) | OVRInput.Get(responseButtonB))
                        {
                            timer += Time.deltaTime;

                            if (timer >= heldTime)
                            {
                                //load next scene
                                main.loadNextScene();
                            }
                        }
                        else { 
                            //if let go, reset timer
                            timer = 0;
                        }
                    }
                }
                break;      
        }
    }

    /*
     * Method for enabling inputs after a few seconds, so that when the scene changes it doesn't automatically reapply the controller input (as it is still held down because it loads so fast)
     */
    IEnumerator EnableInputs()
    {
        //waits for 2 seconds
        yield return new WaitForSeconds(2);
        //after the 2 seconds the inputs are now enabled for the controllers
        inputsEnabled = true;
        print("inputs now enabled");
    }

}
