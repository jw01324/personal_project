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
    public static int[] satNavOrder;
    public static int[] controlTimes;
    public static Result[] results = new Result[4];
    private int currentNumberOfResults = 0;

    public float getAverageControlTime()
    {
        int total = 0;

        foreach(int time in controlTimes)
        {
            total += time;
        }

        //number of control times is 5, so the average will be the total divided by the number of values (5).
        return total / 5;
    }

    public int getMedianControlTime()
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

    public void addResult(Result result)
    {
        results[currentNumberOfResults] = result;
        currentNumberOfResults += 1;
    }

    public string dataToString()
    {

        string s = "";

        //TODO: convert all the scene data to a string that can be written to a file and be readable.

        return s;
    }

}
