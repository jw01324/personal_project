using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/*
 * Class for the SatNav GameObject. 
 * This has 4 types: audio-only, visual-only, audio-visual, and programmable.
 */ 
public class SatNav : MonoBehaviour
{
    //Enumerators
    public enum SatNavType { AUDIO, VISUAL, AUDIOVISUAL, PROGRAMMABLE };
    public enum Direction { LEFT, RIGHT, UP };

    //Variables
    private SatNavType type;
    private Direction direction;
    public int intType;
    public int correctAnswers;
    public int incorrectAnswers;
    private float countdownTimer;
    public static float countdownTime = 2f;
    private bool arrowVisable;
    public static bool isActive;
    private bool startCountdown;

    //Rotation vectors for the arrow on the satnav
    private Quaternion right = Quaternion.Euler(new Vector3(0, 0, 90));
    private Quaternion left = Quaternion.Euler(new Vector3(0, 0, 270));
    private Quaternion up = Quaternion.Euler(new Vector3(0, 0, 180));

    //GameObjects
    private Main main;
    public AudioClip leftAudio;
    public AudioClip rightAudio;
    public AudioClip forwardAudio;
    public AudioSource satnavVoice;
    public AudioSource inputAudio;

    //UI
    public Canvas arrowCanvas;
    public Canvas wordCanvas;
    public Image panel;
    private Transform arrow;
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI userText;
    public RawImage audioImage;
    public RawImage muteImage;
    public RawImage hiddenImage;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        //initialising common variables for all types
        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
        isActive = true;
        correctAnswers = 0;
        incorrectAnswers = 0;
    
        //method that sets inttype to the satnav type chosen for the current scene
        getSatNavType();

        //checks whether the satnav is directional (0,1,2) or programmable (3)
        if (intType != 3)
        {
            //if directional satnav type, disable the programmable satnav view
            wordCanvas.gameObject.SetActive(false);
            startCountdown = false;
            countdownTimer = countdownTime;

            //enable directional satnav UI
            arrow = GameObject.Find("Arrow").transform;
            arrow.GetComponent<Image>().enabled = false;

            //initialise these variables
            slider.maxValue = countdownTime;
            slider.minValue = 0;
            slider.value = 0;
            slider.gameObject.SetActive(false);

            //setting the mute symbol for visual only
            if (intType == 1)
            {
                audioImage.gameObject.SetActive(false);
            }
            else
            {
                muteImage.gameObject.SetActive(false);

            }

            //setting the hidden symbol for audio only.
            if (intType != 0)
            {
                hiddenImage.gameObject.SetActive(false);
            }

        }
        else
        {
            //if programmable satnav type, disable the directional satnav view
            arrowCanvas.gameObject.SetActive(false);
            wordText.text = "";
            wordText.text = generateWord();
            userText.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //checking if the scene has started
        if (main.getHasStarted())
        {
            //checking if the scene has ended
            if (!main.getState())
            {
                //checking the satnav type
                if (intType != 3)
                {
                    //if directional, then make sure the arrow image is visable
                    arrowVisable = arrow.GetComponent<Image>().enabled;

                    //check if the countdown has started (if a satnav query is occurring)
                    if (startCountdown)
                    {
                        //reduce countdown timer value by the time elapsed since the last frame
                        countdownTimer -= Time.deltaTime;

                        //checks if the satnav type is either audiovisual or visual
                        if (type == SatNavType.AUDIOVISUAL | type == SatNavType.VISUAL)
                        {
                            //if it is either, then make the user interface of the satnav change to show the time running out
                            slider.gameObject.SetActive(true);
                            slider.value = countdownTimer;

                            //checks if the user failed to interact in time
                            if (countdownTimer < 0)
                            {
                                //if so, make the screen turn red (for a fail) - which is denoted by the parameter 1 for the screenlight coroutine
                                StartCoroutine(ScreenLight(1));
                            }
                        }

                        //checks if the user has failed to interact in time (called for all types)
                        if (countdownTimer < 0)
                        {
                            //increment the incorrect answers variable by one
                            incorrectAnswers++;

                            //set countdown timer to the max time
                            countdownTimer = countdownTime;

                            //start concurrent method that picks a random direction in a random ammount of time
                            StartCoroutine(PickDirection());
                        }
                    }
                    else
                    {
                        //if countdown hasn't started (or has just finished), then reset countdown timer to the max time
                        countdownTimer = countdownTime;
                    }
                }
            }
            else
            {
                //if programmable, then disable the directional type UI
                for (int i = 1; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    /*
     * Method for choosing a direction for the satnav when the scene starts
     */
    public void onStart()
    {
        //checks the satnav type
        if (intType != 3)
        {
            //if directional, then start the concurrent method to pick a random direction for the satnav
            StartCoroutine(PickDirection());
        }
    }

    /*
     * Concurrent method that makes the satnav screen light up a colour for a small amount of time (0.5 seconds)
     * The colour is green if the user inputted a correct response, otherwise for incorrect the screen turns red.
     */
    IEnumerator ScreenLight(int i)
    {
        //switch statement that takes the parameter for the input, the parameter is 0 for green (correct), 1 for red (incorrect)
        switch (i)
        {
            case (0): //correct input
                panel.color = Color.green;
                yield return new WaitForSeconds(0.5f);
                panel.color = Color.white;
                arrow.GetComponent<Image>().enabled = false;
                slider.value = 0;
                slider.gameObject.SetActive(false);
                break;
            case (1): //incorrect input
                panel.color = Color.red;
                yield return new WaitForSeconds(0.5f);
                panel.color = Color.white;
                arrow.GetComponent<Image>().enabled = false;
                slider.value = 0;
                slider.gameObject.SetActive(false);
                break;
        }
    }

    /*
     * Concurrent method used to pick a random direction, after a random amount of time, for the directional satnav to query the user with
     * Possible directions are: left, right, forward.
     */
    IEnumerator PickDirection()
    {
        //make sure the countdown boolean is false
        startCountdown = false;

        //choose a random length of time to wait after the last query, before choosing the next query
        int sec = Random.Range(3, 8);

        //wait for the random length of time
        yield return new WaitForSeconds(sec);

        //checks if the satnav is still active in the scene (if the scene isn't about to end / has ended already)
        if (isActive)
        {
            //choose random integer from 0 to 2 (0 = left, 1 = right, 2 = forward)
            int r = Random.Range(0, 3);
            direction = (Direction)r;

            //checks the satnav type and tells the user which direction using the format of the satnav type
            if (type == SatNavType.AUDIO)
            {
                //audio satnav type - only play the audio for the direction

                /*
                 * Switch statement to set the correct clip to the audiosource
                 */
                switch (r)
                {
                    case 0:
                        satnavVoice.clip = leftAudio;
                        //set clip to "left" voice
                        break;
                    case 1:
                        satnavVoice.clip = rightAudio;
                        //set clip to "right" voice
                        break;
                    case 2:
                        satnavVoice.clip = forwardAudio;
                        //set clip to "forward" voice
                        break;
                }

                //play the chosen audioclip once
                satnavVoice.PlayOneShot(satnavVoice.clip);

            }
            else if (type == SatNavType.VISUAL)
            {
                //visual satnav type - only show the arrow on the screen

                /*
                 * Switch statement to set the arrow to the correct rotation to reflect the direction it is meant to show
                 */
                switch (r)
                {
                    case 0:
                        arrow.localRotation = left;
                        break;
                    case 1:
                        arrow.localRotation = right;
                        break;
                    case 2:
                        arrow.localRotation = up;
                        break;
                }

                //display the arrow
                arrow.GetComponent<Image>().enabled = true;

            }
            else if (type == SatNavType.AUDIOVISUAL)
            {
                //audiovisual satnav type - play the audio and show the arrow

                /*
                 * Switch statement to set the arrow to the correct rotation to reflect the direction it is meant to show
                 */
                switch (r)
                {
                    case 0:
                        arrow.localRotation = left;
                        break;
                    case 1:
                        arrow.localRotation = right;
                        break;
                    case 2:
                        arrow.localRotation = up;
                        break;
                }

                //display the arrow
                arrow.GetComponent<Image>().enabled = true;

                /*
                 * Switch statement to set the correct clip to the audiosource
                 */
                switch (r)
                {
                    case 0:
                        satnavVoice.clip = leftAudio;
                        //set clip to "left" voice
                        break;
                    case 1:
                        satnavVoice.clip = rightAudio;
                        //set clip to "right" voice
                        break;
                    case 2:
                        satnavVoice.clip = forwardAudio;
                        //set clip to "forward" voice
                        break;
                }

                //play the chosen audioclip once
                satnavVoice.PlayOneShot(satnavVoice.clip);

            }

            //set the value of start countdown to true (so the countdown will start)
            startCountdown = true;
        }
    }

    /*
     * Method for checking if the direction the user inputs is correct.
     * This method is strictly for directional satnav type responses.
     */
    public void checkInputDirectionCorrect(int input)
    {
        //checks if the countdown has started (if a query has been sent to the user)
        if (startCountdown)
        {
            //checks if the directional input from the user matches to expected directional input
            if (input != (int)direction)
            {
                //if it doesn't match, then it is an incorrect input

                //the incorrect answers variable is incremented by one
                incorrectAnswers++;

                //checks if the satnav type is visual in some way
                if (type == SatNavType.AUDIOVISUAL | type == SatNavType.VISUAL)
                {
                    //if so, make the screen light turn red (for incorrect)
                    StartCoroutine(ScreenLight(1));
                }

                //reset the directional satnav and start the concurrent method to choose a new random direction
                StartCoroutine(PickDirection());

                //set the input audio pitch to 0.5 (lower pitch to differentiate the correct and incorrect sounds)
                inputAudio.pitch = 0.5f;

            }
            else
            {
                //if it matches, then it is a correct input

                //increment the correct answers variable by one
                correctAnswers++;

                //checks if the satnav type is visual in some way
                if (type == SatNavType.AUDIOVISUAL | type == SatNavType.VISUAL)
                {
                    //if so, make the screen light turn green (for correct)
                    StartCoroutine(ScreenLight(0));
                }

                //reset the directional satnav and start the concurrent method to choose a new random direction
                StartCoroutine(PickDirection());

                //set the input audio pitch to 1 (higher pitch to differentiate the correct and incorrect sounds)
                inputAudio.pitch = 1f;

            }

            //play the audioclip (input feedback)
            inputAudio.PlayOneShot(inputAudio.clip);
        }
    }

    /*
     * Method for generating a random word for the programmable satnavs
     * This word is a postcode in the format:
     * {A-Z}(2) {0-9}2 {A-Z}2
     * 
     * For example:
     * GU2 9EJ
     */
    public string generateWord()
    {
        //array of letters
        char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        //initialise string s as an empty string
        string s = "";

        //add two random letters to the string
        for (int i = 0; i < 2; i++)
        {
            s += letters[Random.Range(0, letters.Length)];
        }

        //add two random integers to the string (between 0 to 9)
        for (int i = 0; i < 3; i++)
        {
            if (i == 1)
            {
                //add the space in the postcode when i = 1, to keep the format like a postcode (GU2 9EJ)
                s += " ";
            }
            else
            {
                //add a random number between 0 and 10 (integers ranging from 0 to 9)
                s += (int)Random.Range(0, 10);
            }
        }

        //add two random letters to the string
        for (int i = 0; i < 2; i++)
        {
            //choose random letter to add to the string
            s += letters[Random.Range(0, letters.Length)];
        }

        //return the full string
        return s;
    }

    /*
     * Method for checking if the word (postcode) the user submitted on the keyboard is correct
     */
    public void submitWord()
    {
        //Converting user input and generated word to the same format
        //This is so the user doesn't have to type it in exactly the same format
        string input = userText.text.Replace(" ", string.Empty);
        input = input.ToLower();
        string word = wordText.text.Replace(" ", string.Empty);
        word = word.ToLower();

        //checks if the strings are the same
        if (string.Equals(input, word))
        {
            //if they are the same, increment the correct answers variable by one
            correctAnswers++;

            //generate a new word, and set the user text field to an empty string (so they don't have to erase the previous string)
            wordText.text = generateWord();
            userText.text = "";

            //set the audioclip pitch to 1 (higher pitch for correct)
            inputAudio.pitch = 1f;
        }
        else
        {
            //if they are not the same then increment the correct answers variable by one
            incorrectAnswers++;

            //set the audioclip pitch to 0.5 (lower pitch for incorrect)
            inputAudio.pitch = 0.5f;
        }

        //play the audioclip for the user reaction (input feedback)
        inputAudio.PlayOneShot(inputAudio.clip);
    }

    /*
     * Method for assigning the sat nav type (audio, audiovisual, etc) to the correct scene that it's in depending on the random order generated at the start 
     */
    public void assignSatNavTypeToScene()
    {
        intType = SceneData.satNavOrder[SceneManager.GetActiveScene().buildIndex - 2];
    }

    /*
     * Getter for the satnav type, but in string form (instead of integer or enum form)
     */
    public string getSatNavType()
    {
        assignSatNavTypeToScene();

        if (intType >= 0 & intType < 4)
        {
            type = (SatNavType)intType;
            print(type.ToString());
        }
        else
        {
            print("choose other number of type");
        }

        return "" + type;
    }

}
