using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class for the Coordinate Object.
 * Holds a positional value, and a boolean value that tells the car that collides with the coordinate whether it should start braking or not
 */
public class Coordinate : MonoBehaviour
{

    //The brake variable is set to public so that it can be adjusted quickly in the UI (it is a checkbox)
    public bool brake;

    //Positional vector of the coordinate
    private Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        //initialising the position vector to to the position of the object in the scene
        position = gameObject.transform.position;
    }

    /*
     * Getter for the position of the coordinate
     */
    public Vector3 getPosition()
    {
        return position;
    }

}
