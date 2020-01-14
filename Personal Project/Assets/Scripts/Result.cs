using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Result
{
    //common variables
    private string currentTime;
    private string sceneName;

    //car scene variables
    private float reactionTime;
    private string satnavType;
    private bool crashed, music;
    private int fails;

    //control scene variables
    private string reactionTimes;
    private double average, median;

    public Result(string sceneName, string satnavType, float reactionTime, bool crashed, bool music, int fails)
    {
        currentTime = DateTime.Now.ToString("dd-MM-yyyyTHH':'mm':'ss");
        this.sceneName = sceneName;
        this.satnavType = satnavType;
        this.reactionTime = reactionTime;
        this.crashed = crashed;
        this.music = music;
        this.fails = fails;
    }

    public Result(string reactionTimes, double average, double median)
    {
        this.currentTime = DateTime.Now.ToString("dd-MM-yyyyTHH':'mm':'ss");
        this.sceneName = "ControlScene";
        this.reactionTimes = reactionTimes;
        this.average = average;
        this.median = median;

    }

    public string toString(int i)
    {
        string s = "";

        switch (i)
        {
            case 0:
                //control scene result to string
                s = sceneName + "\n" + currentTime + "\n" + reactionTimes + "\n" + "average = " + average + ", median = " + median;
                break;
            case 1:
                //car scene result to string
                s = sceneName + "\n" + currentTime + "\n" + "Satnav: " + satnavType + "\n" + "Music: " + music + "\n" + "Reaction Time: " + reactionTime + "\n" + "Fails: " + fails + "\n" + "Crashed: " + crashed;
                break;
            case 2:
                //filename
                s = sceneName + "_" + currentTime + ".txt";
                break;
            default:
                break;
        }

        return s;
    }






}
