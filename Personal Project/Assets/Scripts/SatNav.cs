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


    private Transform arrow;
    public TextMeshProUGUI text;

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

        startCountdown = false;
        countdownTimer = countdownTime;

        arrow = GameObject.Find("Arrow").transform;
        arrow.GetComponent<Image>().enabled = false;

        text.SetText("");

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

        StartCoroutine(pickDirection());

    }

    void Update()
    {
        if (!main.getState())
        {
            arrowVisable = arrow.GetComponent<Image>().enabled;

            if (startCountdown)
            {
                countdownTimer -= Time.deltaTime;

                if (type == SatNavType.AUDIOVISUAL | type == SatNavType.VISUAL)
                {

                    int viewtime = (int)countdownTimer + 1;
                    text.SetText("Time left: " + viewtime.ToString());

                    if (countdownTimer < 0)
                    {
                        //failed to interact in time
                        text.SetText("");
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
                    StartCoroutine(pickDirection());
                }

            }
            else
            {
                countdownTimer = countdownTime;

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

    IEnumerator pickDirection()
    {
        startCountdown = false;

        int sec = Random.Range(2, 8);

        yield return new WaitForSeconds(sec);

        if (!main.getState())
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
            //do nothing as it is over
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
                text.SetText("");

                //reset arrow
                StartCoroutine(pickDirection());


                return false;
            }
            else
            {
                //correct input
                correctAnswers++;
                text.SetText("");

                //reset arrow
                StartCoroutine(pickDirection());

                return true;
            }
        }
        else
        {
            //incorrect input (no arrow to follow)
            incorrectAnswers++;
            text.SetText("");
            return false;
        }

    }

    /*
     * Assigning the sat nav type (audio, audiovisual, etc) to the correct scene that it's in depending on the random order generated at the start 
     */
    public void assignSatNavTypeToScene()
    {

        intType = SceneData.satNavOrder[SceneManager.GetActiveScene().buildIndex - 2];

        /*
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case (2):
                break;
            case (3):
                break:

        }
        */
        
    }

    public string getSatNavType()
    {
        return "" + type;
    }

}
