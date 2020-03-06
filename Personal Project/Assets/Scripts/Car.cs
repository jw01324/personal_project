using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private float accelerationVariable;
    private float acceleration;
    private float fullDeceleration;
    private Waypoint path;
    private Coordinate currentCoordinate;
    private Rigidbody rb;
    private bool crashed;
    public static float lightIntensity = 15f;
    private float currentSpeed;
    private float speed;
    public bool isFollowingPath;
    private int currentIndex;
    private bool isStopping;
    private bool renderingRearLights;
    private bool hasBraked;
    private bool nearEnd;
    private static bool needToStop;
    public GameObject[] rearLights;
    public static int incorrectStops;
    public static int correctStops;
    private Vector3 previousPosition;
    private int frameCount;
    private float elapsedTime;

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
        accelerationVariable = 0.0075f;
        acceleration = 0.175f;
        fullDeceleration = 0.4f;
        elapsedTime = 0;
        frameCount = 0;
        incorrectStops = 0;
        correctStops = 0;
        crashed = false;
        hasBraked = false;
        isStopping = false;
        needToStop = false;
        speed = 0;
        currentSpeed = 0;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;

        maxSteeringAngle = 45; //degrees
        maxTorque = 150f;
        maxBrakingTorque = 200f;
        isBraking = false;

        previousPosition = transform.position;
        
        path = GameObject.FindGameObjectWithTag("Waypoint").GetComponent<Waypoint>();

        if (isFollowingPath)
        {
            currentIndex = 0;
            currentCoordinate = path.getCoordinate(currentIndex);
        }


    }

    void FixedUpdate()
    {

        if (!Main.getState())
        {
            if (isFollowingPath)
            {
                calculateSteeringAngle();
                move();

            }
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
        //calculating current speed
        currentSpeed = 2 * Mathf.PI * FL.radius * FL.rpm * 60 / 1000;

        //apply torque to the wheels until at the coordinates set speed
        if (currentSpeed < speed & !isBraking)
        {
            FL.motorTorque = maxTorque;
            FR.motorTorque = maxTorque;

            foreach (GameObject light in rearLights)
            {
                light.GetComponent<Light>().intensity = 0;
            }

        }
        else if(currentSpeed > speed & !isBraking)
        {
            isBraking = true;
            //once at the speed required, do not add any more torque
            FL.motorTorque = 0;
            FR.motorTorque = 0;

            foreach (GameObject light in rearLights)
            {
                light.GetComponent<Light>().intensity = lightIntensity;
            }

        }
        else if (currentSpeed > speed & isBraking)
        {
            RL.brakeTorque = maxBrakingTorque;
            RR.brakeTorque = maxBrakingTorque;
        }
        else if(currentSpeed < speed & isBraking)
        {
            RL.brakeTorque = 0;
            RR.brakeTorque = 0;
        }

    }


    // Update is called once per frame
    void Update()
    {

        speed = currentCoordinate.mph * 2;
    }
    /***

        //Vector3 pos = currentCoordinate.getPosition();

        if (!Main.getState())
        {
            
            frameCount++;
            elapsedTime += Time.deltaTime;

            //calculating the speed over x amount of frames to increase accuracy
            if(frameCount >= 10) {

                //current speed in meters per second (1.43 is the conversion so that it is relative to the smaller scale of everything)
                float currentSpeed = ((transform.position - previousPosition).magnitude) / elapsedTime;
                //converting to miles per hour
                currentSpeed = currentSpeed * 2.2369f;
                previousPosition = transform.position;

                print(currentSpeed + "mph");

                frameCount = 0;
                elapsedTime = 0;
            }
            

            //print(gameObject.name + "Acceleration: " + acceleration);

            if (isFollowingPath)
            {
                if (!isStopping)
                {

                    //converting mph to m/s for the coordinate speed
                    float mps = currentCoordinate.mph / 2.2369f;

                    if (speed < mps)
                    {

                        speed += acceleration;

                        if (speed > mps)
                        {
                            speed = mps;
                        }

                    }

                    if (speed > mps)
                    {

                        if (gameObject.tag != "Player" & !needToStop)
                        {
                            needToStop = true;
                            Main.startTimer();
                        }
                        
                        if(gameObject.tag == "Player" & needToStop & !hasBraked)
                        {
                            hasBraked = false;
                        }
                     
                        foreach(GameObject light in rearLights)
                        {
                            light.GetComponent<Light>().intensity = lightIntensity;
                        }

                        speed -= acceleration;

                        if (speed <= mps)
                        {
                            speed = mps;

                            if (gameObject.tag != "Player")
                            {
                                needToStop = false;
                                if(Main.getTimer() != 0)
                                {
                                    Main.stopTimer();
                                    incorrectStops++;
                                    print("Correct: " + correctStops + ", Incorrect: " + incorrectStops);
                                }
                            }

                            if(gameObject.tag == "Player")
                            {
                                hasBraked = false;
                            }

                            foreach (GameObject light in rearLights)
                            {
                                light.GetComponent<Light>().intensity = 0;
                            }
                        }
                    }
                }
                else
                {

                    if (crashed)
                    {
                        print("Crashed");
                        Main.stopScene();
                       
                    }
                    else
                    {
                        if (speed > 0)
                        {
                            speed -= fullDeceleration;
                        }
                        else
                        {
                           
                            speed = 0;

                            if (gameObject.tag == "Player")
                            {
                                print("Didn't crash");
                                Main.stopScene();
                            }
                            else
                            {
                            //this code can be used to measure the time it takes for the car to come to a complete stop
                            //(need to disable the car script on the player car to avoid collision)
                                if (Main.getTimer() > 0)
                                {
                                    print("STOPPING TIME: " + Main.stopTimer());
                                }
                            }
                            
                        }
                    }
                }

                var q = Quaternion.LookRotation(pos - steeringAxis.position);
                transform.rotation = Quaternion.RotateTowards(steeringAxis.rotation, q, (speed * 8) * Time.deltaTime);

                // move towards the target
                transform.Translate(Vector3.forward * speed * Time.deltaTime);

            }
        }

    }
    */


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
            
            correctStops++;
            float time = Main.stopTimer() * 1000;
            print("Reaction Time (ms): " + time);
            hasBraked = true;

        }
        else if (needToStop & hasBraked)
        {
            //do nothing as always inputted brakes, but not incorrect as the car in front is still braking
        }
        else
        {
            //no indication to stop, hence incorrect
            incorrectStops++;
        }


        print("Correct: " + correctStops + ", Incorrect: " + incorrectStops);
        //TODO: Stop timer
        
    }


    private void OnTriggerEnter(Collider col)
    {
        
        if (col.tag == "Coordinate")
        {
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

                if (gameObject.tag == "Player" & currentCoordinate.mph < currentSpeed)
                {
                    StartCoroutine(Braking());
                }

            }

          
           
        }
        

        if (col.tag == "Car")
        {
            print("end scene");
            crashed = true;
            Main.stopScene();


        }

        if(col.tag == "Stop" & gameObject.tag == "Car")
        {

            StartCoroutine(StopCar());
        }

        if (col.tag == "Stop" & gameObject.tag == "Player")
        {
            nearEnd = true;
        }

    }

    IEnumerator Braking()
    {
        print("BRAKING");
        if (gameObject.tag != "Player" & !needToStop)
        {
            needToStop = true;
            Main.startTimer();
        }

        if (gameObject.tag == "Player" & needToStop & !hasBraked)
        {
            hasBraked = false;
        }

        foreach (GameObject light in rearLights)
        {
            light.GetComponent<Light>().intensity = lightIntensity;
        }

        yield return new WaitForSeconds(2f);

        if (gameObject.tag != "Player")
        {
            needToStop = false;
            if (Main.getTimer() != 0)
            {
                Main.stopTimer();
                incorrectStops++;
                print("Correct: " + correctStops + ", Incorrect: " + incorrectStops);
            }
        }

        if (gameObject.tag == "Player")
        {
            hasBraked = false;
        }

        isBraking = false;

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
        Main.startTimer();

    }

}
