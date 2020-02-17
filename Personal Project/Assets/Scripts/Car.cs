using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    public Waypoint path;
    private Coordinate currentCoordinate;
    private Rigidbody rb;
    private float speed;
    private bool isFollowingPath;
    private bool islastCoordinate;

    // Start is called before the first frame update
    void Start()
    {
        speed = 0;
        isFollowingPath = false;
        islastCoordinate = false;
        rb = gameObject.GetComponent<Rigidbody>();

        if (gameObject.tag == "Player")
        {
            isFollowingPath = true;
            currentCoordinate = path.getCurrentCoordinate();

        }
    }

    // Update is called once per frame
    void Update()
    {

        if (isFollowingPath)
        {
            Vector3 pos = currentCoordinate.getPosition();

            print(speed);

            if(speed < currentCoordinate.speed)
            {
             
                speed += 0.05f;

                if(speed > currentCoordinate.speed)
                {
                    speed = currentCoordinate.speed;
                }
               
            } 

            if(speed > currentCoordinate.speed)
            {
              
                speed -= 0.05f;

                if (speed < currentCoordinate.speed)
                {
                    speed = currentCoordinate.speed;
                }
            }

            print(speed);

            var q = Quaternion.LookRotation(pos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 5f * Time.deltaTime);

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
        //while object is still moving
        while (rb.velocity.magnitude > 0)
        {
            //slow car down
            rb.AddForce(-rb.GetPointVelocity(gameObject.transform.position) * 2);
        }

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
            else
            {
                //do nothing
            }
        }


    }

}
