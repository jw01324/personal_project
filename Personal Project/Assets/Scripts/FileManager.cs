using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileManager 
{
    string path = Application.persistentDataPath + "./Data/" + StartScene.userID + "/";

    public bool createResultFile(Result result, int type)
    {

        try
        {
            string filepath = path + result.toString(2);

            if (!File.Exists(filepath))
            {
                File.WriteAllText(filepath, "");
            }

            File.AppendAllText(filepath, result.toString(type));

            return true;

        }catch(Exception e)
        {

            return false;
        }
 
    }
}
