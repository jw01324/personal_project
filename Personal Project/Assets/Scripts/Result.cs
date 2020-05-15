using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * Class for a result, used to store all the data collected in the scene into one variable (makes it easier to understand later)
 */
public class Result
{
    //Variables
    private string id;
    private string currentTime;
    private string sceneName;
    private string satnavType;
    private int[] reactionTimes;
    private int correctReactions;
    private int incorrectReactions;
    private int correctSatNavInputs;
    private int incorrectSatNavInputs;
    private bool crashed;

    /*
     * Parameterised constructor for a Result object
     */ 
    public Result(string sceneName, string satnavType, int[] reactionTimes, int correctReactions, int incorrectReactions, int correctSatNavInputs, int incorrectSatNavInputs, bool crashed)
    {
        //checks if userid hasn't been assigned, if not, probably testing in the editor so assign "Test ID" as the id
        if (SceneData.userID != null)
        {
            id = SceneData.userID;
        }
        else
        {
            id = "Test ID";
        }

        //sets current time of when the result was recorded
        currentTime = DateTime.Now.ToString("dd-MM-yyyyTHH':'mm':'ss");

        //checks that these values are null, if they are then throw an exception
        if(sceneName == null | satnavType == null | reactionTimes == null)
        {
            throw new NullReferenceException();
        }

        //checks if reaction counts are below zero, as that should be impossible.
        if(correctReactions < 0 | incorrectReactions < 0 | correctSatNavInputs < 0 | incorrectSatNavInputs < 0)
        {
            throw new ArgumentException();
        }

        //sets all global variables to the values of the respective parameters
        this.sceneName = sceneName;
        this.satnavType = satnavType;
        this.reactionTimes = reactionTimes;
        this.correctReactions = correctReactions;
        this.incorrectReactions = incorrectReactions;
        this.correctSatNavInputs = correctSatNavInputs;
        this.incorrectSatNavInputs = incorrectSatNavInputs;
        this.crashed = crashed;
    }

    /*
     * Method to convert the values held in the Result object to a singular string that contains all of the information for the result - returns a string
     */
    public string toString()
    {
        string s = "";
        
        //car scene result variables converted to string
        s = "ID: " + id + "\n" + "Scene: " + sceneName + "\n" + currentTime + "\n" + "SatNav Type: " + satnavType + "\n" + "Reaction Times: " + arrayToString() + "\n"
            + "Correct Reactions/Incorrect Reactions: " + correctReactions + "/" + incorrectReactions + "\n" + "Correct SatNavInputs/Incorrect SatNavInputs: " + correctSatNavInputs
            + "/" + incorrectSatNavInputs + "\n" + "Crashed: " + crashed;

        return s;
    }

    /*
     * Method that converts the array of reaction times into a string format that is readable - returns a string.
     * This method is called in the toString method above.
     */ 
    public string arrayToString()
    {
        //checks if the array has any members (whether its length is greater than 0)
        if (reactionTimes.Length > 0)
        {
            //if so, then initialise string s to hold the outer bracket of an array
            string s = "[";

            //loop through the array (apart from the last value), adding each value in string format with a comma to the string
            for (int i = 0; i < reactionTimes.Length - 1; i++)
            {
                s += (reactionTimes[i] + ", ");
            }

            //add the last value to the string, however in a different format (no comma, but a closed bracket for the array because it is the final value)
            s += (reactionTimes[reactionTimes.Length - 1] + "]");

            //return the full string
            return s;
        }
        else
        {
            //if array has no values then return a string that represents an empty array
            return "[]";
        }
    }

}
