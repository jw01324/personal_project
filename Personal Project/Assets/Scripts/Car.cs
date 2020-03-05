﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float accelerationVariable = 0.1f;
    public float acceleration = 0;
    private Waypoint path;
    private Coordinate currentCoordinate;
    private Rigidbody rb;
    private Transform steeringAxis;
    private bool crashed;
    public static float lightIntensity = 15f;
    private float speed;
    public bool isFollowingPath;
    private bool islastCoordinate;
    private bool isStopping;
    private bool renderingRearLights;
    private bool hasBraked;
    private bool nearEnd;
    private static bool needToStop;
    public GameObject[] rearLights;
    public static int incorrectStops;
    public static int correctStops;

    // Start is called before the first frame update
    void Start()
    {

        incorrectStops = 0;
        correctStops = 0;
        crashed = false;
        hasBraked = false;
        isStopping = false;
        needToStop = false;
        speed = 0;
        islastCoordinate = false;
        rb = gameObject.GetComponent<Rigidbody>();

        steeringAxis = transform.GetChild(0);

        GameObject waypoint = GameObject.FindGameObjectWithTag("Waypoint");
        int childCount = waypoint.transform.childCount;
        path = new Waypoint(childCount);

        for (int i = 0; i < childCount; i++)
        {
            //adding each coordinate to the waypoint
            path.addCoordinate(i, waypoint.transform.GetChild(i).GetComponent<Coordinate>());
        }

        if (isFollowingPath)
        {
            currentCoordinate = path.getCurrentCoordinate();
        }


    }

    // Update is called once per frame
    void Update()
    {

        Vector3 pos = currentCoordinate.getPosition();

        if (!Main.getState())
        {

            if (isFollowingPath)
            {
                if (!isStopping)
                {

                    if (speed < currentCoordinate.speed)
                    {

                        speed += accelerate(acceleration);
                        //speed += 0.05f;

                        if (speed > currentCoordinate.speed)
                        {
                            speed = currentCoordinate.speed;
                            acceleration = 0;
                        }

                    }

                    if (speed > currentCoordinate.speed)
                    {

                        if (gameObject.tag != "Player" & !needToStop)
                        {
                            needToStop = true;
                            Main.startTimer();
                        }
                        
                        if(gameObject.tag == "Player" & needToStop & !hasBraked)
                        {
                            print("test");
                            hasBraked = false;
                        }
                     
                        foreach(GameObject light in rearLights)
                        {
                            light.GetComponent<Light>().intensity = lightIntensity;
                        }

                        //speed -= 0.1f;
                        speed -= accelerate(acceleration);
                        print(gameObject.tag + ": " + speed);

                        if (speed <= currentCoordinate.speed)
                        {
                            speed = currentCoordinate.speed;
                            acceleration = 0;

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
                    print(gameObject.name + ": " + speed);

                    if (crashed)
                    {
                        Main.stopScene();
                    }
                    else
                    {
                        if (speed > 0)
                        {
                            speed -= accelerate(acceleration);
                        }
                        else
                        {
                           
                            speed = 0;

                            if (gameObject.tag == "Player")
                            {
                                Main.stopScene();
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

    public float accelerate(float currentAcceleration)
    {
        currentAcceleration += accelerationVariable;
        return currentAcceleration;
    }


    public void move(float speed, Vector3 direction)
    {
        transform.position += direction * Time.deltaTime * speed;
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

            //print(gameObject.name + " collided with " + col.name);

            if (path.getCurrent() + 1 >= path.getLength())
            {
                //last coordinate, no need to get more
                islastCoordinate = true;

            }

            if (!islastCoordinate)
            {
                //reached coordinate, now get next one
                currentCoordinate = path.getNextCoordinate();

            }

            if (islastCoordinate)
            {
                isFollowingPath = false;
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

    IEnumerator StopCar()
    {
        needToStop = false;

        foreach (GameObject light in rearLights)
        {
            light.GetComponent<Light>().intensity = 0;
        }

        int sec = Random.Range(5, 10);
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
