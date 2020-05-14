using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/*
 * Class for saving files to the device (pc or oculus depending on where it is running)
 */
public class FileManager
{
    //Variables
    public static string folderPath;

    /*
     * Method for creating a directory (folder)
     */ 
    public static void createDirectory()
    {
        //checks if application is running on the oculus or pc
        if (!SceneData.isOnOculus)
        {
            //if pc, set folderpath variable to a path on the pc (resources folder of the unity folder)
            folderPath = "Assets/Resources/TestData/ID" + SceneData.userID;
        }
        else
        {
            //if oculus, set folderpath variable to a path on the oculus device (persistent data path subfolder)
            folderPath = Application.persistentDataPath + "/TestData/ID" + SceneData.userID;
        }

        //checks if the folder exists, if not then create it otherwise don't
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

    /*
     * Method for checking if the directory (folder) already exists - returns a true or false value.
     * This is used when generating a random user id when the application starts, so that the application can check if the user id is unique.
     * Duplicate ids for 2 unique users would make their results harder to differentiate. 
     */ 
    public static bool checkIfDirectoryExists(string userID)
    {
        //checks if application is running on the oculus or pc
        if (!SceneData.isOnOculus)
        {
            //if pc, set folderpath variable to a path on the pc (resources folder of the unity folder)
            folderPath = "Assets/Resources/TestData/ID" + SceneData.userID;
        }
        else
        {
            //if oculus, set folderpath variable to a path on the oculus device (persistent data path subfolder)
            folderPath = Application.persistentDataPath + "/TestData/ID" + SceneData.userID;
        }

        //checks if the folder exists, return this boolean (true/false) value
        return Directory.Exists(folderPath);
    }

    /*
     * Method for creating and saving the results file to the system.
     * The folderpath variable will already be defined in "createdirectory" so the differentiation between pc and oculus is not needed as that has already taken place in the previous method.
     * [Note: createResultsFile doesn't ever run before createDirectory, so this isn't an issue]
     */ 
    public static void createResultFile()
    {
        //try block for writing the file
        try {

            //filepath - which is a text file which includes the date of creation in the name
            string filePath = folderPath + "/Result_" + DateTime.Now.ToString("dd-MM-yy_HHmm") + ".txt";

            //write the results to the file (converting all the results in the scenedata class to text using the method dataToString from that class)
            File.WriteAllText(filePath, SceneData.dataToString(), System.Text.Encoding.ASCII);

            //log that the file has successfully been created in the console.
            Debug.Log("file created");

        }catch(Exception e)
        {
            //if an error occurs in the writing of the file, then log this error in the console.
            Debug.LogError(e);
        }
    }
   
}
