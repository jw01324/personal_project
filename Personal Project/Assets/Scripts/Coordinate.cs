using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate : MonoBehaviour
{
    //public so that it can be adjusted quickly in the UI
    public float mph;
    private Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        //setting position value to the individual gameobjects position in the scene
        position = gameObject.transform.position;
    }

    public Vector3 getPosition()
    {
        return position;
    }

    public void deleteObject()
    {
        Destroy(gameObject);
    }
}
