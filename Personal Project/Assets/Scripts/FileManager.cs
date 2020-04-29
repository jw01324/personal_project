using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileManager
{

    public static string folderPath;

    public static void createDirectory()
    {
        if (!SceneData.isOnOculus)
        {
            //pc path
            folderPath = "Assets/Resources/TestData/ID" + SceneData.userID;
        }
        else
        {
            //oculus path
            folderPath = Application.persistentDataPath + "/TestData/ID" + SceneData.userID;
        }

        //checks if the folder exists, if not then create it
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("folder created");
        }
        else
        {
            Debug.Log("folder already exists");
        }
    }

    public static void createResultFile()
    {

        try {

            //filepath
            string filePath = folderPath + "/Result_" + DateTime.Now.ToString("dd-MM-yy_HHmm") + ".txt";

            //write results to file (results.txt)
            File.WriteAllText(filePath, SceneData.dataToString(), System.Text.Encoding.ASCII);

            Debug.Log("file created");

        }catch(Exception e)
        {

            Debug.LogError(e);
        }
 
    }

    
}
