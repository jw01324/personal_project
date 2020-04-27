using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SatNav : MonoBehaviour
{
    private Main main;
    public int intType;
    private bool arrowVisable;
    private float countdownTimer;
    public static float countdownTime = 3f;
    private bool startCountdown;
    public int correctAnswers;
    public int incorrectAnswers;
    public enum SatNavType { AUDIO, VISUAL, AUDIOVISUAL, PROGRAMMABLE };
    public enum Direction { LEFT, RIGHT, UP};
    private SatNavType type;
    private Direction direction;
    public AudioSource leftAudio;
    public AudioSource rightAudio;
    public AudioSource forwardAudio;

    public Canvas arrowCanvas;
    public Canvas wordCanvas;
    private Transform arrow;
    public TextMeshProUGUI arrowText;
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI userText;

    //rotation vectors for the arrow on the satnav
    private Quaternion right = Quaternion.Euler(new Vector3(0, 0, 90));
    private Quaternion left = Quaternion.Euler(new Vector3(0, 0, 270));
    private Quaternion up = Quaternion.Euler(new Vector3(0, 0, 180));

    // Start is called before the first frame update
    void Start()
    {
        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

        correctAnswers = 0;
        incorrectAnswers = 0;

        getSatNavType();

        //if not programmable type
        if (intType != 3)
        {
            //disable the programmable satnav view
            wordCanvas.gameObject.SetActive(false);
            startCountdown = false;
            countdownTimer = countdownTime;

            arrow = GameObject.Find("Arrow").transform;
            arrow.GetComponent<Image>().enabled = false;

            arrowText.SetText("");

        }
        else
        {
            //disable the directional satnav view
            arrowCanvas.gameObject.SetActive(false);
            wordText.text = "";
            wordText.text = generateWord();
            userText.text = "";
        }

    }

    void Update()
    {
        if (main.getHasStarted())
        {
            if (!main.getState())
            {
                if (intType != 3)
                {
                    arrowVisable = arrow.GetComponent<Image>().enabled;

                    if (startCountdown)
                    {
                        countdownTimer -= Time.deltaTime;

                        if (type == SatNavType.AUDIOVISUAL | type == SatNavType.VISUAL)
                        {

                            int viewtime = (int)countdownTimer + 1;
                            arrowText.SetText("Time left: " + viewtime.ToString());

                            if (countdownTimer < 0)
                            {
                                //failed to interact in time
                                arrowText.SetText("");
                                arrow.GetComponent<Image>().enabled = false;

                            }

                        }

                        if (type == SatNavType.AUDIO | type == SatNavType.AUDIOVISUAL)
                        {
                            if (countdownTimer < 0)
                            {
                                //play fail audio
                            }
                        }

                        if (countdownTimer < 0)
                        {
                            //failed to interact in time
                            incorrectAnswers++;
                            countdownTimer = countdownTime;
                            StartCoroutine(PickDirection());
                        }

                    }
                    else
                    {
                        countdownTimer = countdownTime;
                        arrowText.SetText("");
                    }
                }

            }
            else
            {
                //don't want to disable the main model but everything else needs to be disabled (screen, text, etc.)
                for (int i = 1; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    public void onStart()
    {
        if (intType != 3)
        {
            StartCoroutine(PickDirection());
        }
    }

    IEnumerator PickDirection()
    {
        startCountdown = false;

        int sec = Random.Range(2, 8);

        yield return new WaitForSeconds(sec);

        if (!main.getState() | !main.isTiming)
        {

            int r = Random.Range(0, 3);

            direction = (Direction)r;

            print(direction);


            if (type == SatNavType.AUDIO)
            {
                //audio

                /*
                 * PLAY AUDIO
                 */

                switch (r)
                {
                    case 0:
                        leftAudio.Play();
                        //play "left" voice
                        break;
                    case 1:
                        rightAudio.Play();
                        //play "right" voice
                        break;
                    case 2:
                        forwardAudio.Play();
                        //play "up" voice
                        break;
                }

                print("play audio");

            }else if (type == SatNavType.VISUAL)
            {
                //visual 

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

                print("display arrow");
                arrow.GetComponent<Image>().enabled = true;

            }else if (type == SatNavType.AUDIOVISUAL)
            {
                //audiovisual 

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

                print("display arrow");
                arrow.GetComponent<Image>().enabled = true;

                /*
                 * PLAY AUDIO
                 */

                switch (r)
                {
                    case 0:
                        leftAudio.Play();
                        //play "left" voice
                        break;
                    case 1:
                        rightAudio.Play();
                        //play "right" voice
                        break;
                    case 2:
                        forwardAudio.Play();
                        //play "up" voice
                        break;
                }

                print("play audio");
            }      

            startCountdown = true;

        }
        else
        {
            //do nothing as the test is ending or is over already
        }
    }

    public bool checkInputDirectionCorrect(int input)
    {
        if (startCountdown)
        {

            if(type == SatNavType.AUDIOVISUAL | type == SatNavType.VISUAL)
            {
                arrow.GetComponent<Image>().enabled = false;
            }

            if (input != (int)direction)
            {
                //incorrect input
                incorrectAnswers++;
                arrowText.SetText("");

                //reset arrow
                StartCoroutine(PickDirection());


                return false;
            }
            else
            {
                //correct input
                correctAnswers++;
                arrowText.SetText("");

                //reset arrow
                StartCoroutine(PickDirection());

                return true;
            }
        }
        else
        {
            //incorrect input (no arrow to follow)
            incorrectAnswers++;
            arrowText.SetText("");
            return false;
        }

    }

    /*
     * method for generating a random word for the programmable satnav that the user can type in
     */ 
    public string generateWord()
    {
        char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        string s = "";

        for(int i = 0; i < 2; i++)
        {
            //choose random letter to add to the string
            s += letters[Random.Range(0, letters.Length)];
        }

        for (int i = 0; i < 3; i++)
        {
            if(i == 1)
            {
                //add the space in the postcode
                s += " ";
            }
            else
            {
                //add a random number between 0 and 10 (integers ranging from 0 to 9)
                s += (int)Random.Range(0, 10);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            //choose random letter to add to the string
            s += letters[Random.Range(0, letters.Length)];
        }

        return s;
    }

    public void submitWord()
    {
        //converting user input and generated word to the same format (so the user doesn't have to type it exactly the same)
        string input = userText.text.Replace(" ", string.Empty);
        input = input.ToLower();
        string word = wordText.text.Replace(" ", string.Empty);
        word = word.ToLower();

        if (string.Equals(input, word))
        {
            correctAnswers++;
            print("correct");
            wordText.text = generateWord();
            userText.text = "";
        }
        else
        {
            incorrectAnswers++;
            print("wrong");
        }

        //userText.text = "[" + input + "-" + word + "]";
    }

    /*
     * Assigning the sat nav type (audio, audiovisual, etc) to the correct scene that it's in depending on the random order generated at the start 
     */
    public void assignSatNavTypeToScene()
    {

        intType = SceneData.satNavOrder[SceneManager.GetActiveScene().buildIndex - 2];
        
    }

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
