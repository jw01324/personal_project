using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    //arraylist holding coordinates
    private Coordinate[] coordinates;
    public Color colour;

    void Start()
    {

        coordinates = new Coordinate[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            addCoordinate(i, transform.GetChild(i).GetComponent<Coordinate>());
        }

    }

    /*
     * Method for drawing the path (just to help me visually see the path in the editor when I'm creating it or debugging)
     */ 
    void OnDrawGizmos()
    {
        Gizmos.color = colour;

        for(int i = 1; i < transform.childCount; i++)
        {
            Vector3 currentCoordinate = transform.GetChild(i).position;
            Vector3 previousCoordinate = transform.GetChild(i-1).position;

            Gizmos.DrawLine(previousCoordinate, currentCoordinate);
            Gizmos.DrawWireSphere(previousCoordinate, 1);

            if(i == transform.childCount - 1)
            {
                Gizmos.DrawWireSphere(currentCoordinate, 1);
            }
        }
    }

    public void addCoordinate(int index, Coordinate c)
    {
        coordinates[index] = c;
    }


    public Coordinate getCoordinate(int index)
    {
        return coordinates[index];
    }

    public Coordinate[] getList()
    {
        return coordinates;
    }

    public int getLength()
    {
        return coordinates.Length;
    }
    
}
