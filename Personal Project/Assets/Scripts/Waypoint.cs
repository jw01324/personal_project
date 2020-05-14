using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class Waypoint which is used to hold all the coordinate objects that are in the scene.
 * The coordinates are in order, set as children to the waypoint object in the scene editor, and are used to make a path from one coordinate to the other.
 */
public class Waypoint : MonoBehaviour
{
    //Variables
    private Coordinate[] coordinates;
    public Color colour;

    /*
     * Initialising the array on awake
     */
    void Awake()
    {
        //initialising the coordinates to all of the children of the waypoint object (this has been setup intentionally in the editor for this purpose)
        coordinates = new Coordinate[transform.childCount];

        //loops through the objects in order and adds to the coordinates array of the waypoint object
        for(int i = 0; i < transform.childCount; i++)
        {
            addCoordinate(i, transform.GetChild(i).GetComponent<Coordinate>());
        }
    }

    /*
     * Method for drawing the path (just to visualise the path in the editor when I'm creating it)
     * Note: I make the paths by creating coordinate objects and adding them to the waypoint object.
     * Adding the next coordinate beneath the previous one in the list, so that they are in order.
     */ 
    void OnDrawGizmos()
    {
        //setting the colour of the line drawn in the editor to the colour selected for the colour variable of this object in the editor
        Gizmos.color = colour;

        //loop through coordinate, and draw a line from the previous coordinate to the current coordinate
        for(int i = 1; i < transform.childCount; i++)
        {
            Vector3 currentCoordinate = transform.GetChild(i).position;
            Vector3 previousCoordinate = transform.GetChild(i-1).position;

            Gizmos.DrawLine(previousCoordinate, currentCoordinate);
            Gizmos.DrawWireSphere(previousCoordinate, 1);

            //draws a sphere around the coordinate (as it has no physical appearance in the scene, this shows it clearly in the editor)
            if(i == transform.childCount - 1)
            {
                Gizmos.DrawWireSphere(currentCoordinate, 1);
            }
        }
    }

    /*
     * Method for adding a cooridinate to the array (called in the awake method)
     * Takes in an integer value for the index, and a coordinate object as the parameters
     */
    public void addCoordinate(int index, Coordinate c)
    {
        coordinates[index] = c;
    }

    /*
     * Getter for a coordinate in the array
     */
    public Coordinate getCoordinate(int index)
    {
        return coordinates[index];
    }

    /*
     * Getter for the full coordinate array (all coordinates)
     */
    public Coordinate[] getList()
    {
        return coordinates;
    }

    /*
     * Getter for the length of the array of coordinates
     */ 
    public int getLength()
    {
        return coordinates.Length;
    }
    
}
