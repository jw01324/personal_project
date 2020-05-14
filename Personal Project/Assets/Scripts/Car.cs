using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class for Car GameObject.
 */
public class Car : MonoBehaviour
{
    //Variables
    public static float lightIntensity = 15f;
    private float maxSteeringAngle;
    private float maxTorque;
    private float maxBrakingTorque;
    private float currentSpeed;
    private float maxSpeed;
    private float minSpeed;
    private int currentIndex;
    private bool isBraking;
    private bool isStopping;
    private bool hasBraked;
    private bool nearEnd;
    private static bool needToStop = false;
    private Vector3 previousPosition;

    //keeps track of reactions to the car braking (and crash status at the end)
    public int incorrectReactions;
    public int correctReactions;
    public List<int> reactionTimes;
    public bool crashed;

    //GameObjects
    private Main main;
    private Waypoint path;
    private Coordinate currentCoordinate;
    private AudioSource correctSound;
    private AudioSource engineSound;
    public GameObject[] rearLights;
    public WheelCollider FL;
    public WheelCollider FR;
    public WheelCollider RL;
    public WheelCollider RR;
    public Transform centerOfMass;



    // Start is called before the first frame update
    void Start()
    {
        //initialising variables
        crashed = false;
        incorrectReactions = 0;
        correctReactions = 0;
        reactionTimes = new List<int>();
        hasBraked = false;
        isBraking = false;
        isStopping = false;

        //converts meters per second to miles per hour
        float mphConversion = 2.23694f;
        minSpeed = 15 / mphConversion;
        maxSpeed = 30 / mphConversion;
        currentSpeed = 0;
        previousPosition = transform.position;

        //setting center of mass of the car to the correct location
        gameObject.GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;

        //initialising sound effects
        correctSound = GameObject.FindGameObjectWithTag("Correct_SFX").GetComponent<AudioSource>();
        engineSound = GameObject.FindGameObjectWithTag("Engine_SFX").GetComponent<AudioSource>();

        //initialising wheel variables
        maxSteeringAngle = 45; 
        maxTorque = 150f;
        maxBrakingTorque = 100000f;

        //setting variable to the object with tag "Waypoint" in the scene & initialising the first coordinate
        path = GameObject.FindGameObjectWithTag("Waypoint").GetComponent<Waypoint>();
        currentIndex = 0;
        currentCoordinate = path.getCoordinate(currentIndex);
        
        //setting variable to the object with tag "Main" in the scene
        main = GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        //if statements used to only update the scene when the user has left the menu, and when the scene has not finished already (stops when finished).
        if (main.getHasStarted())
        {
            if (!main.getState())
            {
                //calculating current speed
                currentSpeed = (transform.position - previousPosition).magnitude / Time.deltaTime;
                previousPosition = transform.position;

                //play engine noise if not already playing
                if (!engineSound.isPlaying) {
                    engineSound.Play();
                }

                //setting pitch of the engine sound to the normalised value of current speed in order to get the desired effect of a car engine sound
                engineSound.pitch = normaliseValues(currentSpeed, -maxSpeed, maxSpeed);

                    //if statement that checks if the user has started to stop the car, if not then move the car and calculate steering angle to the next coordinate
                    if (!isStopping)
                    {
                        calculateSteeringAngle();
                        move();
                    }
                    else
                    {
                        //resetting motor torque to zero for both front wheels
                        FL.motorTorque = 0;
                        FR.motorTorque = 0;

                        //setting brake torque to max for all wheels
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
                                if (main.getTimer() > 0)
                                {
                                    print("STOPPING TIME: " + main.stopTimer());
                                }
                            }

                        }

                    }
                
            }
            else
            {
                //scene has finished so make sure the car is not moving anymore

                //resetting motor torque to zero for both front wheels
                FL.motorTorque = 0;
                FR.motorTorque = 0;

                //setting brake torque to max for all wheels
                FL.brakeTorque = maxBrakingTorque;
                FR.brakeTorque = maxBrakingTorque;
                RL.brakeTorque = maxBrakingTorque;
                RR.brakeTorque = maxBrakingTorque;

                engineSound.Stop();
            }
        }

    }

    /*
     * Front wheels going in the direction of the coordinate
     */
    public void calculateSteeringAngle()
    {
        //calculates the relative position between the car and the coordinate it is going towards & adjusts the steering angle based on this
        Vector3 relativePos = transform.InverseTransformPoint(path.getCoordinate(currentIndex).transform.position);
        float steeringAngle = (relativePos.x / relativePos.magnitude) * maxSteeringAngle;
        float turnSpeed = 8f;

        //front wheels used for steering so their angles are adjusted to the steering angle over time (at the rate of the turnspeed selected, which is there to make turning smoother) 
        FL.steerAngle = Mathf.Lerp(FL.steerAngle, steeringAngle, Time.deltaTime * turnSpeed);
        FR.steerAngle = Mathf.Lerp(FR.steerAngle, steeringAngle, Time.deltaTime * turnSpeed);

    }

    /*
     * Method for moving the car (applying torque to the wheels)
     */
    public void move()
    {
        //turning brake lights on/off depending on whether the car is braking or not
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

        //apply torque to the wheels until at the coordinates set speed (depending on current speed and whether the car is braking)
        //this acts like a juggling act, where the speed will tend towards the max or min speed (depending on if the car is braking) 
        if (currentSpeed < maxSpeed & !isBraking)
        {
            //setting torque of front wheels to max
            FL.motorTorque = maxTorque;
            FR.motorTorque = maxTorque;

            //resetting brake torque to zero
            RL.brakeTorque = 0;
            RR.brakeTorque = 0;

        }
        else if (currentSpeed > maxSpeed & !isBraking)
        {
            //once at the maximum speed, do not add any more torque
            FL.motorTorque = 0;
            FR.motorTorque = 0;


        }
        else if (currentSpeed > minSpeed & isBraking)
        {
            //resetting motor torque to zero 
            FL.motorTorque = 0;
            FR.motorTorque = 0;

            //setting braking torque to max
            RL.brakeTorque = maxBrakingTorque;
            RR.brakeTorque = maxBrakingTorque;
        }
        else if (currentSpeed < minSpeed & isBraking)
        {
            //once at the minimum speed, do not add anymore braking torque
            FL.motorTorque = maxTorque;
            FR.motorTorque = maxTorque;
            RL.brakeTorque = 0;
            RR.brakeTorque = 0;
        }

    }

    /*
     * Method for checking the reactions of the user, and when the time comes to set a variable that tells the car stop
     */
    public void stop()
    {
        if (needToStop & !hasBraked)
        {
            //if the car in front is about to emergency stop
            if (nearEnd)
            {
                isStopping = true;
            }

            //increment correct reactions variable and add reaction time to the array
            correctReactions++;
            int time = (int) (main.stopTimer() * 1000);
            print("Reaction Time (ms): " + time);
            reactionTimes.Add(time);

            //play the sound that confirms the user has reacted (for input feedback)
            correctSound.Play();

            //set hasbraked to true (so the user cannot input multiple reaction times to the same incident)
            hasBraked = true;

        }
        else if (needToStop & hasBraked)
        {
            //do nothing as the user has already inputted their reaction, but not incorrect as the car in front is still braking
        }
        else
        {
            //no indication to stop, hence incorrect reaction by the user
            incorrectReactions++;
        }
    }

    /*
     * Method for checking if collisions between two objects has occurred
     */
    private void OnCollisionEnter(Collision col)
    {
        //if the collision is between a Car object and a Car object, then the user has crashed (hence set crashed to true and stop the scene)
        if (col.gameObject.tag == "Car")
        {
            print("crashed");
            crashed = true;
            main.stopScene();
        }
    }

    /*
    * Method for checking if objects have entered the radius of a trigger.
    * Triggers are specified areas in the scene, either attached to objects (cars, coordinates) or in a position in the scene (on the road)
    */
    private void OnTriggerEnter(Collider col)
    {
        //checks if the object that the car has collided with is a coordinate object
        if (col.tag == "Coordinate")
        {
            //checking if the coordinate has the brake variable set to true, which would mean the car should brake.
            if (currentCoordinate.brake)
            {
                //there are 2 car objects that use this script, if the car that collides is not the player car, then set the static bool needtostop to true so the player car knows it needs to stop.
                if (gameObject.tag != "Player" & !needToStop)
                {
                    needToStop = true;
                    //start the timer because the user needs to react to the AI car braking
                    main.startTimer();
                }

                //if the car is player, then reset the hasbraked variable to false
                if (gameObject.tag == "Player" & needToStop & !hasBraked)
                {
                    hasBraked = false;
                }

                //set isbraking to true
                isBraking = true;
            }
            else //if the coordinate is just a normal positional coordinate point
            {
                //if the car is braking then make them stop braking
                if (isBraking)
                {
                    //if the non-player car reaches the coordinate after previously braking then set needtostop to false so if the player inputs a response it will be incorrect (as the AI is not braking anymore)
                    if (gameObject.tag != "Player")
                    {
                        needToStop = false;
                        //if the timer is still going then the user didn't respond to the incident, therefore increment the players car variable for incorrect reactions by 1 and reset the timer
                        if (main.getTimer() != 0)
                        {
                            GameObject.FindGameObjectWithTag("Player").GetComponent<Car>().incorrectReactions++;
                            main.stopTimer();
                        }
                    }

                    //if the car is player, then reset the hasbraked variable to false
                    if (gameObject.tag == "Player")
                    {
                        hasBraked = false;
                    }

                    //set isbraking to false as the car is no longer braking
                    isBraking = false;
                }
            }

            //increment the current index of the coordinate array by 1, as the car has reached the coordinate and needs to know the location of the next coordinate
            currentIndex++;

            //checks if there are still more coordinates, if this is false then there are no more coordinates left
            if (currentIndex + 1 < path.getLength())
            {
                //reached coordinate, now get next one
                currentCoordinate = path.getCoordinate(currentIndex);
            }
        }

        //checks if the object that the car has collided with is the stop object (the trigger that indicates there will now be an emergency stop)
        if (col.tag == "Stop" & gameObject.tag == "Car")
        {
            //starts a coroutine (a concurrent method) for the car to be stopped in a randomised amount of time
            StartCoroutine(StopCar());
            //make the satnav no longer generate anymore queries to the user
            SatNav.isActive = false;
        }

        //checks if the player has collided with it, in which case the players car will be set to stop when the user presses the reaction button
        if (col.tag == "Stop" & gameObject.tag == "Player")
        {
            nearEnd = true;
        }
    }

    /*
     * Concurrent Method for stopping the car in front (non-player car) after a random length of time
     */
    IEnumerator StopCar()
    {
        //resets the variables to make sure that the car isn't ready for braking until after the random length of time
        needToStop = false;

        foreach (GameObject light in rearLights)
        {
            light.GetComponent<Light>().intensity = 0;
        }

        //random length of time for the car to stop from 2 to 5 seconds
        int sec = Random.Range(2, 5);

        //waits for that length of time
        yield return new WaitForSeconds(sec);

        //puts the brake lights on
        foreach (GameObject light in rearLights)
        {
            light.GetComponent<Light>().intensity = lightIntensity;
        }

        //sets the stopping variables to true and starts the timer
        isStopping = true;
        print("STOPPING");
        needToStop = true;
        main.startTimer();

    }

    /*
     * Method for normalising values. This is used for engine sound pitch, so that it can be set to a value between 0 and 1 depending on the cars current speed
     */
    public float normaliseValues(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

}
