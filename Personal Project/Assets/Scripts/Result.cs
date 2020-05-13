using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Result
{
    // variables
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


    public Result(string sceneName, string satnavType, int[] reactionTimes, int correctReactions, int incorrectReactions, int correctSatNavInputs, int incorrectSatNavInputs, bool crashed)
    {
        if (SceneData.userID != null)
        {
            id = SceneData.userID;
        }
        else
        {
            id = "Test ID";
        }

        currentTime = DateTime.Now.ToString("dd-MM-yyyyTHH':'mm':'ss");
        this.sceneName = sceneName;
        this.satnavType = satnavType;
        this.reactionTimes = reactionTimes;
        this.correctReactions = correctReactions;
        this.incorrectReactions = incorrectReactions;
        this.correctSatNavInputs = correctSatNavInputs;
        this.incorrectSatNavInputs = incorrectSatNavInputs;
        this.crashed = crashed;
    }


    public string toString()
    {
        string s = "";
        
        // car scene result to string
        s = "ID: " + id + "\n" + "Scene: " + sceneName + "\n" + currentTime + "\n" + "SatNav Type: " + satnavType + "\n" + "Reaction Times: " + arrayToString() + "\n"
            + "Correct Reactions/Incorrect Reactions: " + correctReactions + "/" + incorrectReactions + "\n" + "Correct SatNavInputs/Incorrect SatNavInputs: " + correctSatNavInputs
            + "/" + incorrectSatNavInputs + "\n" + "Crashed: " + crashed;


        return s;
    }


    public string arrayToString()
    {
        if (reactionTimes.Length > 0)
        {
            string s = "[";

            for (int i = 0; i < reactionTimes.Length - 1; i++)
            {
                s += (reactionTimes[i] + ", ");
            }

            s += (reactionTimes[reactionTimes.Length - 1] + "]");

            return s;
        }
        else
        {
            return "[]";
        }

    }





}
