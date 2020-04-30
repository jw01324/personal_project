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
    public static float countdownTime = 2f;
    public static bool isActive;
    private bool startCountdown;
    public int correctAnswers;
    public int incorrectAnswers;
    public enum SatNavType { AUDIO, VISUAL, AUDIOVISUAL, PROGRAMMABLE };
    public enum Direction { LEFT, RIGHT, UP };
    private SatNavType type;
    private Direction direction;
    public AudioSource leftAudio;
    public AudioSource rightAudio;
    public AudioSource forwardAudio;
    public AudioSource inputAudio;

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

    //rotation vectors for the arrow on the satnav
    private Quaternion right = Quaternion.Euler(new Vector3(0, 0, 90));
    private Quaternion left = Quaternion.Euler(new Vector3(0, 0, 270));
    private Quaternion up = Quaternion.Euler(new Vector3(0, 0, 180));

    // Start is called before the first frame update
    void Start()
    {
        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
        isActive = true;
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

                            slider.gameObject.SetActive(true);
                            slider.value = countdownTimer;

                            if (countdownTimer < 0)
                            {
                                //failed to interact in time

                                StartCoroutine(ScreenLight(1));
                            }

                        }

                        if (type == SatNavType.AUDIO | type == SatNavType.AUDIOVISUAL)
                        {
                            if (countdownTimer < 0)
                            {
                                //play fail audio
                            }
                        }

                        //global between all types
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
                        //reset countdown timer to the max time
                        countdownTimer = countdownTime;

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

    IEnumerator ScreenLight(int i)
    {
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

    IEnumerator PickDirection()
    {
        startCountdown = false;

        int sec = Random.Range(2, 8);

        yield return new WaitForSeconds(sec);

        //if the satnav is still active in the scene (if the scene isn't about to end / has ended already)
        if (isActive)
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

            }
            else if (type == SatNavType.VISUAL)
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

            }
            else if (type == SatNavType.AUDIOVISUAL)
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
            print("satnav stopped");
        }
    }

    public void checkInputDirectionCorrect(int input)
    {
        if (startCountdown)
        {

            if (input != (int)direction)
            {
                //incorrect input
                incorrectAnswers++;

                if (type == SatNavType.AUDIOVISUAL | type == SatNavType.VISUAL)
                {
                    StartCoroutine(ScreenLight(1));
                }
                //reset arrow
                StartCoroutine(PickDirection());

                inputAudio.pitch = 0.5f;

            }
            else
            {
                //correct input
                correctAnswers++;

                if (type == SatNavType.AUDIOVISUAL | type == SatNavType.VISUAL)
                {
                    StartCoroutine(ScreenLight(0));
                }

                //reset arrow
                StartCoroutine(PickDirection());
                inputAudio.pitch = 1f;

            }

            inputAudio.Play();
        }

    }

    /*
     * method for generating a random word for the programmable satnav that the user can type in
     */
    public string generateWord()
    {
        char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        string s = "";

        for (int i = 0; i < 2; i++)
        {
            //choose random letter to add to the string
            s += letters[Random.Range(0, letters.Length)];
        }

        for (int i = 0; i < 3; i++)
        {
            if (i == 1)
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

            inputAudio.pitch = 1f;
        }
        else
        {
            incorrectAnswers++;
            print("wrong");

            inputAudio.pitch = 0.5f;
        }

        inputAudio.Play();

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
