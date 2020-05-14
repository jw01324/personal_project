using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class for holding data between scenes whilst the application is running (holds temporary data before it is saved to file)
 */
public class SceneData
{

    /*
     * All scene data is persistant when kept in a traditional c# class, and not a unity object class.
     * Hence, to keep track of data that will be used or added to over different scenes, they will be variables in this class.
     * This also means that the read and writing of data doesn't need to be done when each scene is loaded in, only after all data has been recorded (at the end).
     */

    //variable to hold whether the application is playing in the pc editor or on the oculus device (used to differentiate controls and file save paths) initialised as false
    public static bool isOnOculus = false;

    //variable for user id, initialised as test123 for testing on the editor
    public static string userID = "test123";

    //setting default satnav order to do audiovisual for all scenes as that's best for testing (if i start the app from scene 1 without generating an order then this is handy)
    public static int[] satNavOrder = { 2, 2, 2, 2 };

    //initialising control time array with 5 empty members
    public static int[] controlTimes = new int[5];

    //initialising results list, ready to add results
    public static List<Result> results = new List<Result>();

    /*
     * Method to calculate the average for control times - returns a float
     */
    public static float getAverageControlTime()
    {
        //checking if any control times have been recorded
        if (controlTimes.Length > 0)
        {
            //initialise total to zero
            int total = 0;

            //loops through all values and adds each time value to the total
            foreach (int time in controlTimes)
            {
                total += time;
            }

            //number of control times is 5, so the average returned will be the total divided by the number of values (5).
            return total / 5;
        }
        else
        {
            //if no times recorded, return 0
            return 0;
        }
    }

    /*
     * Method to calculate the median for control times - returns a float
     */
    public static int getMedianControlTime()
    {
        //checking if any control times have been recorded
        if (controlTimes.Length > 0)
        {
            //set variable sorted to false
            bool sorted = false;
            //create variable for holding a temporary integer
            int tempValue;

            //while loop that sorts the times in order of smallest to largest, it will keep looping until no unsorted values are found
            while (!sorted)
            {
                //set sorted to true, if there are unsorted values found then this will be set to false
                sorted = true;

                //loops through all the values 
                for (int i = 0; i < controlTimes.Length - 1; i++)
                {
                    //checks if the current value is bigger than the next value in the array
                    if (controlTimes[i] > controlTimes[i + 1])
                    {
                        //if so, then switch the values of the 2 members of the array and set sorted to false
                        tempValue = controlTimes[i];
                        controlTimes[i] = controlTimes[i + 1];
                        controlTimes[i + 1] = tempValue;
                        sorted = false;
                    }
                }
            }

            //number of control times is 5, so the median will be the 3rd number. Hence, the second index in the array.
            return controlTimes[2];
        }
        else
        {
            //if no times recorded, return 0
            return 0;
        }
    }

    /*
     * Method for converting all data saved to a string (so it can be written to a file) - returns a string
     */
    public static string dataToString()
    {
        //initialise string s as an empty string
        string s = "";

        //convert all the scene data to a string and add it to the empty string
        s += ("ID: " + userID + "\n" + "Control Times: " + controlTimesToString() + ", Average = " + getAverageControlTime() + ", Median = " + getMedianControlTime() + "\n\n"
            + "Order:\n" + orderToString() + "\n" + resultsToString());

        //return the full string
        return s;
    }

    /*
     * Method for converting the satnav order array to a string that displays, in human readable form, which satnav was used in which scene.
     */
    public static string orderToString()
    {
        //initialise string s as an empty string
        string s = "";

        //loop through the satnav order array
        for (int i = 0; i < satNavOrder.Length; i++)
        {
            //convert the integer (index) value of the satnav to the enum that the integer represents
            SatNav.SatNavType type = (SatNav.SatNavType)satNavOrder[i];
            //add the scene number and the satnav type that was in the scene
            s += "Scene " + (i + 1) + " = " + type + "\n";
        }

        //returns the full string
        return s;
    }

    /*
     * Method for converting the control times array to a readable string - returns a string
     */
    public static string controlTimesToString()
    {
        //initialise string s as the opening bracket of an array
        string s = "[";

        //loops through array (except the last member)
        for (int i = 0; i < controlTimes.Length - 1; i++)
        {
            //adds each value to the string, with a comma seperating
            s += (controlTimes[i] + ", ");
        }

        //add the last member outside of the loop, so that the format can be changed to adding the closed brackets instead of a comma
        s += (controlTimes[controlTimes.Length - 1] + "]");

        //returns full string
        return s;
    }

    /*
     * Method for converting the results array to a string - returns a string
     */
    public static string resultsToString()
    {
        //initialises string s as an empty string
        string s = "";

        //loops through the array of results variables
        for (int i = 0; i < results.Count; i++)
        {
            //adds each result in string format (converted by the Result class method "toString")
            s += ("Result" + (i + 1) + ": " + results[i].toString() + "\n\n");
        }

        //returns the full string
        return s;
    }

}
