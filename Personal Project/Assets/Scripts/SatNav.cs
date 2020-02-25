using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SatNav : MonoBehaviour
{
    public int intType;
    private bool arrowVisable;
    private float countdownTimer;
    public static float countdownTime = 2f;
    private bool startCountdown;
    public int correctAnswers;
    public int incorrectAnswers;
    public enum Type { AUDIO, VISUAL, AUDIOVISUAL, PROGRAMMABLE };
    public enum Direction { LEFT, RIGHT, UP};
    private Type type;
    private Direction direction;


    private Transform arrow;
    public TextMeshProUGUI text;

    //rotation vectors for the arrow on the satnav
    private Quaternion right = Quaternion.Euler(new Vector3(0, 0, 90));
    private Quaternion left = Quaternion.Euler(new Vector3(0, 0, 270));
    private Quaternion up = Quaternion.Euler(new Vector3(0, 0, 180));

    // Start is called before the first frame update
    void Start()
    {

        correctAnswers = 0;
        incorrectAnswers = 0;

        startCountdown = false;
        countdownTimer = countdownTime;

        arrow = GameObject.Find("Arrow").transform;
        arrow.GetComponent<Image>().enabled = false;

        text.SetText("");

        if (intType >= 0 & intType < 4)
        {
            type = (Type)intType;
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
        arrowVisable = arrow.GetComponent<Image>().enabled;

        if (startCountdown)
        {
            countdownTimer -= Time.deltaTime;

            if (type == Type.AUDIOVISUAL | type == Type.VISUAL)
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

            if (type == Type.AUDIO | type == Type.AUDIOVISUAL)
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

    IEnumerator pickDirection()
    {
        startCountdown = false;

        int sec = Random.Range(2, 8);

        yield return new WaitForSeconds(sec);

        int r = Random.Range(0, 3);

        direction = (Direction)r;

        print(direction);

        //visual OR audiovisual

        if (type == Type.AUDIOVISUAL | type == Type.VISUAL)
        {
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
            

        }else if(type == Type.AUDIOVISUAL | type == Type.AUDIO)
        {
            //audio or audiovisual

            /*
             * PLAY AUDIO
             */

            switch (r)
            {
                case 0:
                    //play "left" voice
                    break;
                case 1:
                    //play "right" voice
                    break;
                case 2:
                    //play "up" voice
                    break;
            }

            print("play audio");
        
        }

        startCountdown = true;

    }

    public bool checkInputDirectionCorrect(int input)
    {
        if (startCountdown)
        {

            if(type == Type.AUDIOVISUAL | type == Type.VISUAL)
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

}
