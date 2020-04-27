using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileManager
{ 
   
    public static string path =  "/sdcard/" + SceneData.userID + "/results.txt";

    public static bool createResultFile()
    {

        try
        {

            if (!File.Exists(path))
            {
                File.WriteAllText(path, "");
            }

            File.AppendAllText(path, SceneData.dataToString());

            return true;

        }catch(Exception e)
        {

            return false;
        }
 
    }

    
}
