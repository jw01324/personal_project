using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData
{

    /*
     * All scene data is persistant when kept in a traditional c# class, and not a unity object class.
     * Hence, to keep track of data that will be used or added to over different scenes, they will be variables in this class.
     * This also means that read and writing of data doesn't need to be done when each scene is loaded in.
     */

    public static string userID;
    //setting default satnav order to do audiovisual for all scenes as that's best for testing (if i start the app from scene 1 without generating an order then this is handy)
    public static int[] satNavOrder = { 3, 2, 2, 2 };
    public static int[] controlTimes = new int[5];
    public static List<Result> results = new List<Result>();

    public static float getAverageControlTime()
    {
        //checking if any control times have been recorded, otherwise will be 0
        if (controlTimes.Length > 0)
        {
            int total = 0;

            foreach (int time in controlTimes)
            {
                total += time;
            }

            //number of control times is 5, so the average will be the total divided by the number of values (5).
            return total / 5;
        }
        else
        {
            return 0;
        }
    }

    public static int getMedianControlTime()
    {
        //checking if any control times have been recorded, otherwise will be 0
        if (controlTimes.Length > 0)
        {
            bool sorted = false;
            int tempValue;

            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < controlTimes.Length - 1; i++)
                {
                    if (controlTimes[i] > controlTimes[i + 1])
                    {
                        tempValue = controlTimes[i];
                        controlTimes[i] = controlTimes[i + 1];
                        controlTimes[i + 1] = tempValue;
                        sorted = false;
                    }
                }
            }

            //number of control times is 5, so the median will be the 3rd number. Hence, second index.
            return controlTimes[2];
        }
        else
        {
            return 0;
        }

    }

    public static string dataToString()
    {

        string s = "";

        //TODO: convert all the scene data to a string that can be written to a file and be readable.

        s += ("ID: " + userID + "\n" + "Control Times: " + controlTimesToString() + ", Average = " + getAverageControlTime() + ", Median = " + getMedianControlTime() + "\n\n"
            + "Order:\n" + orderToString() + "\n" + resultsToString());

        return s;
    }

    public static string orderToString()
    {
        string s = "";

        for (int i = 0; i < satNavOrder.Length; i++)
        {
            SatNav.SatNavType type = (SatNav.SatNavType)satNavOrder[i];
            s += "Scene " + (i + 1) + " = " + type + "\n";
        }

        return s;
    }

    public static string controlTimesToString()
    {
        string s = "[";

        for (int i = 0; i < controlTimes.Length - 1; i++)
        {
            s += (controlTimes[i] + ", ");
        }

        s += (controlTimes[controlTimes.Length - 1] + "]");

        return s;
    }

    public static string resultsToString()
    {
        string s = "";

        for (int i = 0; i < results.Count; i++)
        {
            s += ("Result" + (i + 1) + ": " + results[i].toString(0) + "\n\n");
        }

        return s;
    }

}
