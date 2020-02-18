using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint
{
    //arraylist holding coordinates
    private Coordinate[] coordinates;
    private int currentIndex = 0;
    bool setupDone;

    public Waypoint(int childCount)
    {
        coordinates = new Coordinate[childCount];
    }


    public void addCoordinate(int index, Coordinate c)
    {
        coordinates[index] = c;
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
