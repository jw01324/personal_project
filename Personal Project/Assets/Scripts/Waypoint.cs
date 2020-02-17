using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    //arraylist holding coordinates
    public Coordinate[] coordinates;
    private int currentIndex = 0;
    bool setupDone;


    void Start()
    {

      
    }


    public Coordinate getNextCoordinate()
    {
        currentIndex++;
        return coordinates[currentIndex];
    }

    public Coordinate getCurrentCoordinate()
    {
        return coordinates[currentIndex];
    }

    public Coordinate[] getList()
    {
        return coordinates;
    }

    public int getLength()
    {
        return coordinates.Length;
    }

    public int getCurrent()
    {
        return currentIndex;
    }
    
}
