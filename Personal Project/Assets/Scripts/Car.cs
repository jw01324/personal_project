using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    private Waypoint path;
    private Coordinate currentCoordinate;
    private Rigidbody rb;
    private bool crashed;
    private float speed;
    public bool isFollowingPath;
    private bool islastCoordinate;
    private bool isStopping;
    private bool renderingRearLights;
    public GameObject rearLights;

    // Start is called before the first frame update
    void Start()
    {
        crashed = false;
        isStopping = false;
        speed = 0;
        islastCoordinate = false;
        rb = gameObject.GetComponent<Rigidbody>();

        GameObject waypoint = GameObject.FindGameObjectWithTag("Waypoint");
        int childCount = waypoint.transform.childCount;
        path = new Waypoint(childCount);

        for (int i = 0; i < childCount; i++)
        {
            //adding each coordinate to the waypoint
            path.addCoordinate(i, waypoint.transform.GetChild(i).GetComponent<Coordinate>());
        }

        //don't want to waste computational power on rendering lights on the back of the users car that they cannot see
        if (rearLights != null)
        {
            renderingRearLights = true;
        }
        else
        {
            renderingRearLights = false;
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

        if (isFollowingPath)
        {
            if (!isStopping)
            {

                if (speed < currentCoordinate.speed)
                {

                    speed += 0.05f;

                    if (speed > currentCoordinate.speed)
                    {
                        speed = currentCoordinate.speed;
                    }

                }

                if (speed > currentCoordinate.speed)
                {
                    if (renderingRearLights)
                    {
                        rearLights.GetComponent<Light>().intensity = 5;
                    }

                    speed -= 0.05f;

                    if (speed < currentCoordinate.speed)
                    {
                        speed = currentCoordinate.speed;

                        if (renderingRearLights)
                        {
                            rearLights.GetComponent<Light>().intensity = 0;
                        }
                    }
                }
            }
            else
            {
                if (speed > 0)
                {
                    speed -= 0.1f;
                }
                else
                {
                    speed = 0;
                }
            }

            var q = Quaternion.LookRotation(pos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 100f * Time.deltaTime);

            // move towards the target
            transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
        }

    }

    public void move(float speed, Vector3 direction)
    {
        transform.position += direction * Time.deltaTime * speed;
    }

    public void stop()
    {
        //TODO: Stop timer
        isStopping = true;
        
    }


    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Coordinate")
        {

            print("collided");
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

    }

    IEnumerator StopCar()
    {
        int sec = Random.Range(1, 10);
        print("Seconds: " + sec);

        yield return new WaitForSeconds(sec);

        print("STOPPING");
        if (renderingRearLights)
        {
            rearLights.GetComponent<Light>().intensity = 5;
        }
        isStopping = true;
        //TODO: start reaction timer

    }

}
