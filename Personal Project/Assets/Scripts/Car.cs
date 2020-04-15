using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private Main main;
    private float accelerationVariable;
    private float acceleration;
    private float fullDeceleration;
    private Waypoint path;
    private Coordinate currentCoordinate;
    private Rigidbody rb;
    public static float lightIntensity = 5f;
    private float currentSpeed;
    private float maxSpeed;
    private float minSpeed;
    public bool isFollowingPath;
    private int currentIndex;
    private bool isStopping;
    private bool renderingRearLights;
    private bool hasBraked;
    private bool nearEnd;
    private static bool needToStop;
    public GameObject[] rearLights;
    public int incorrectReactions;
    public int correctReactions;
    public bool crashed;
    private Vector3 previousPosition;
    private int frameCount;
    private float elapsedTime;
    public List<int> reactionTimes = new List<int>();

    public WheelCollider FL;
    public WheelCollider FR;
    public WheelCollider RL;
    public WheelCollider RR;
    private float maxSteeringAngle;
    private float maxTorque;
    private float maxBrakingTorque;
    private bool isBraking;
    public Transform centerOfMass;


    // Start is called before the first frame update
    void Start()
    {
        //acceleration rate variable
        crashed = false;
        accelerationVariable = 0.0075f;
        acceleration = 0.175f;
        fullDeceleration = 0.4f;
        elapsedTime = 0;
        frameCount = 0;
        incorrectReactions = 0;
        correctReactions = 0;
        hasBraked = false;
        isStopping = false;
        needToStop = false;

        float mphConversion = 2.23694f;
        minSpeed = 15 / mphConversion;
        maxSpeed = 30 / mphConversion;
        currentSpeed = 0;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;

        maxSteeringAngle = 45; //degrees
        maxTorque = 150f;
        maxBrakingTorque = 100000f;
        isBraking = false;

        previousPosition = transform.position;

        path = GameObject.FindGameObjectWithTag("Waypoint").GetComponent<Waypoint>();
        print(gameObject.tag + ":" + path.getLength());
        currentIndex = 0;

        if (isFollowingPath)
        {
            currentCoordinate = path.getCoordinate(currentIndex);
        }

        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

    }

    void FixedUpdate()
    {

        if (!main.getState())
        {
            //calculating current speed
            float angularVelocity = 2 * Mathf.PI * FL.radius * (FL.rpm / 60);
            currentSpeed = (transform.position - previousPosition).magnitude / Time.deltaTime;
            //print(gameObject.tag + ": " + currentSpeed + "m/s, " + angularVelocity + "rad/s");
            previousPosition = transform.position;

            if (isFollowingPath)
            {
                if (!isStopping)
                {
                    calculateSteeringAngle();
                    move();
                }
                else
                {
                    //resetting motor torque to zero
                    FL.motorTorque = 0;
                    FR.motorTorque = 0;

                    FL.brakeTorque = maxBrakingTorque;
                    FR.brakeTorque = maxBrakingTorque;
                    RL.brakeTorque = maxBrakingTorque;
                    RR.brakeTorque = maxBrakingTorque;

                   
                    if (currentSpeed <= 0)
                    {
                        if (gameObject.tag == "Player")
                        {
                            print("Didn't crash");
                            main.stopScene();
                        }
                        else
                        {
                            //this code can be used to measure the time it takes for the car to come to a complete stop
                            //(need to disable the car script on the player car to avoid collision)
                            if (main.getTimer() > 0)
                            {
                                print("STOPPING TIME: " + main.stopTimer());
                            }
                        }


                    }
                   
                }

            }
        }
        else
        {
            FL.motorTorque = 0;
            FR.motorTorque = 0;

            FL.brakeTorque = maxBrakingTorque;
            FR.brakeTorque = maxBrakingTorque;
            RL.brakeTorque = maxBrakingTorque;
            RR.brakeTorque = maxBrakingTorque;
        }

    }

    /*
     * Front wheels going in the direction of the coordinate
     */

    public void calculateSteeringAngle()
    {
        Vector3 relativePos = transform.InverseTransformPoint(path.getCoordinate(currentIndex).transform.position);
        float steeringAngle = (relativePos.x / relativePos.magnitude) * maxSteeringAngle;
        float turnSpeed = 8f;
        FL.steerAngle = Mathf.Lerp(FL.steerAngle, steeringAngle, Time.deltaTime * turnSpeed);
        FR.steerAngle = Mathf.Lerp(FR.steerAngle, steeringAngle, Time.deltaTime * turnSpeed);
    }

    public void move()
    {


        //turning brake lights on/off
        if (isBraking)
        {
            foreach (GameObject light in rearLights)
            {
                light.GetComponent<Light>().intensity = lightIntensity;
            }
        }
        else
        {
            foreach (GameObject light in rearLights)
            {
                light.GetComponent<Light>().intensity = 0;
            }
        }

        //apply torque to the wheels until at the coordinates set speed
        if (currentSpeed < maxSpeed & !isBraking)
        {
            FL.motorTorque = maxTorque;
            FR.motorTorque = maxTorque;

            //resetting brake torque to zero
            RL.brakeTorque = 0;
            RR.brakeTorque = 0;

        }
        else if (currentSpeed > maxSpeed & !isBraking)
        {
            //once at the speed required, do not add any more torque
            FL.motorTorque = 0;
            FR.motorTorque = 0;


        }
        else if (currentSpeed > minSpeed & isBraking)
        {
            //resetting motor torque to zero
            FL.motorTorque = 0;
            FR.motorTorque = 0;

            RL.brakeTorque = maxBrakingTorque;
            RR.brakeTorque = maxBrakingTorque;
        }
        else if (currentSpeed < minSpeed & isBraking)
        {
            FL.motorTorque = maxTorque;
            FR.motorTorque = maxTorque;
            RL.brakeTorque = 0;
            RR.brakeTorque = 0;
        }

    }

    public void readyToBrake()
    {
        hasBraked = false;
    }

    public void stop()
    {

        if (needToStop & !hasBraked)
        {
            if (nearEnd)
            {
                isStopping = true;
            }

            correctReactions++;
            int time = (int) (main.stopTimer() * 1000);
            print("Reaction Time (ms): " + time);
            reactionTimes.Add(time);
            hasBraked = true;

        }
        else if (needToStop & hasBraked)
        {
            //do nothing as always inputted brakes, but not incorrect as the car in front is still braking
        }
        else
        {
            //no indication to stop, hence incorrect
            incorrectReactions++;
        }


        print("Correct: " + correctReactions + ", Incorrect: " + incorrectReactions);
        //TODO: Stop timer

    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Car")
        {
            print("crashed");
            crashed = true;
            main.stopScene();

        }
    }

    private void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Coordinate")
        {
            if (currentCoordinate.brake)
            {

                print("BRAKING");
                if (gameObject.tag != "Player" & !needToStop)
                {
                    needToStop = true;
                    main.startTimer();
                }

                if (gameObject.tag == "Player" & needToStop & !hasBraked)
                {
                    hasBraked = false;
                }

                isBraking = true;
            }
            else
            {
                if (isBraking)
                {
                    if (gameObject.tag != "Player")
                    {
                        needToStop = false;
                        if (main.getTimer() != 0)
                        {
                            main.stopTimer();
                            incorrectReactions++;
                            print("Correct: " + correctReactions + ", Incorrect: " + incorrectReactions);
                        }
                    }

                    if (gameObject.tag == "Player")
                    {
                        hasBraked = false;
                    }

                    isBraking = false;
                }

            }

            currentIndex++;

            if (currentIndex + 1 >= path.getLength())
            {
                //last coordinate, no need to get more
                isFollowingPath = false;

            }
            else
            {
                //reached coordinate, now get next one
                currentCoordinate = path.getCoordinate(currentIndex);


            }

            print(gameObject.tag + " : " + currentIndex);


        }


        if (col.tag == "Stop" & gameObject.tag == "Car")
        {

            StartCoroutine(StopCar());
        }

        if (col.tag == "Stop" & gameObject.tag == "Player")
        {
            nearEnd = true;
        }

    }

    IEnumerator StopCar()
    {
        needToStop = false;

        foreach (GameObject light in rearLights)
        {
            light.GetComponent<Light>().intensity = 0;
        }

        int sec = Random.Range(2, 5);
        print("Seconds: " + sec);

        yield return new WaitForSeconds(sec);

        print("STOPPING");

        foreach (GameObject light in rearLights)
        {
            light.GetComponent<Light>().intensity = lightIntensity;
        }

        isStopping = true;
        needToStop = true;
        main.startTimer();

    }

}
