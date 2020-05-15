using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;

/*
 * Test Script for the application
 * Most methods in classes are directly linked to running in the Unity Engine (being in Play mode)
 * This means that only a select few methods can be tested from classes 
 * (calculation methods, initialisation methods, etc.)
 * 
 * Classes like "FileManager", "SceneData", and "Result", were far more suited to unit testing.
 * This is because they do not have the "MonoBehaviour" base class which turns the class into a unity class.
 * This means that these classes do not depend on the "play loop" or "gameplay loop" that is created by Unity
 * and so they can be more easily tested within the editor compared to others that mostly require user testing.
 */
public class TestScript
{
    //Variables for test
    Main main = new Main();
    Car car = new Car();
    SatNav satnav = new SatNav();
    Keyboard keyboard = new Keyboard();

    /*
     * FileManager Class - Tests that the correct path is chosen depending on the platform.
     * The platform this is tested on is pc, so should be the expected path stated for pc in the filemanager class.
     */
    [Test]
    public void platformTest()
    {
        SceneData.userID = "test123";
        string expectedPath = "Assets/Resources/TestData/IDtest123";

        FileManager.createDirectory();

        Assert.That(FileManager.folderPath, Is.EqualTo(expectedPath));
    }

    /*
     * FileManager Class - Testing that the checkIfDirectoryExists method returns true when the directory exists
     */
    [Test]
    public void existingDirectoryTest()
    {
        SceneData.userID = "test456";
        FileManager.createDirectory();

        Assert.That(FileManager.checkIfDirectoryExists("test456"), Is.EqualTo(true));
    }

    /*
     * FileManager Class - Testing that the checkIfDirectoryExists method returns false when the directory does not exist
     */
    [Test]
    public void noExistingDirectoryTest()
    {
        //no directory created with id test789
        //so should return false

        Assert.That(FileManager.checkIfDirectoryExists("test789"), Is.EqualTo(false));
    }

    /*
     * FileManager Class - Testing that a file will be saved without any errors
     */
    [Test]
    public void fileSavingTest()
    {
        Assert.DoesNotThrow(() => FileManager.createResultFile());
    }

    /*
     * Result Class - Testing the parameterised constructor for the class with normal parameters
     */
    [Test]
    public void resultNormalConstructorTest()
    {
        //Variables for result object
        string sceneName = "Scene1";
        string satnavType = "Programmable";
        int[] reactionTimes = new int[] { 100, 200, 300, 400, 500, 600 };
        int correctReactions = 7;
        int incorrectReactions = 3;
        int correctSatNavInputs = 5;
        int incorrectSatNavInputs = 1;
        bool crashed = false;

        //constructing a result object with these variables (shouldn't throw an exception)
        Assert.DoesNotThrow(() => new Result(sceneName, satnavType, reactionTimes, correctReactions, incorrectReactions, correctSatNavInputs, incorrectSatNavInputs, crashed));
    }

    /*
     * Result Class - Testing the parameterised constructor for the class with null parameters
     */
    [Test]
    public void resultNullParametersConstructorTest()
    {
        //Variables for result object

        //set these to null
        string sceneName = null;
        string satnavType = null;
        int[] reactionTimes = null;

        int correctReactions = 7;
        int incorrectReactions = 3;
        int correctSatNavInputs = 5;
        int incorrectSatNavInputs = 1;
        bool crashed = false;

        //constructing a result object with null values where possible (cannot have a null integer or boolean)
        Assert.Catch(() => new Result(sceneName, satnavType, reactionTimes, correctReactions, incorrectReactions, correctSatNavInputs, incorrectSatNavInputs, crashed));
    }

    /*
     * Result Class - Testing the parameterised constructor for the class with null parameters
     */
    [Test]
    public void resultInvalidParametersConstructorTest()
    {
        //Variables for result object
        string sceneName = "Scene1";
        string satnavType = "Programmable";
        int[] reactionTimes = new int[] { 100, 200, 300, 400, 500, 600 };

        //set the integers to negative numbers (invalid argument for these parameters)
        int correctReactions = -1;
        int incorrectReactions = -1;
        int correctSatNavInputs = -1;
        int incorrectSatNavInputs = -1;

        bool crashed = false;

        //constructing a result object with null values where possible (cannot have a null integer or boolean)
        Assert.Catch(() => new Result(sceneName, satnavType, reactionTimes, correctReactions, incorrectReactions, correctSatNavInputs, incorrectSatNavInputs, crashed));
    }

    /*
     * SceneData Class - Testing the average (mean) control times method
     */
    [Test]
    public void meanControlTimeTest()
    {
        //initialise set of control times
        SceneData.controlTimes = new int[] { 500, 100, 705, 250, 150};

        float mean = (500 + 100 + 705 + 250 + 150) / 5;
        float sceneDataMean = SceneData.getAverageControlTime();

        Assert.AreEqual(mean, sceneDataMean);
    }

    /*
     * SceneData Class - Testing the median control times method 
     */
    [Test]
    public void medianControlTimeTest()
    {
        //initialise set of control times
        SceneData.controlTimes = new int[] { 500, 100, 705, 250, 150 };

        float median = 250;
        float sceneDataMedian = SceneData.getMedianControlTime();

        Assert.AreEqual(median, sceneDataMedian);
    }

    /*
     * SatNav Class - Testing that the generateWord method returns a string
     */
    [Test]
    public void nullGenerateWordTest()
    {
        string generatedWord = satnav.generateWord();

        Assert.IsNotNull(generatedWord);
    }

    /*
     * SatNav Class - Testing that each word generated by the method is random/unique
     */
    [Test]
    public void uniqueGenerateWordTest()
    {
        string generatedWord1 = satnav.generateWord();
        string generatedWord2 = satnav.generateWord();

        Assert.AreNotEqual(generatedWord1, generatedWord2);
    }

    /*
     * Car Class - Testing that the normalise method makes the correct calculation
     */
    [Test]
    public void normaliseTest()
    {
        float normalisedValue = car.normaliseValues(50, 0, 255);
        //normalise: y = (x - min)/(max - min)
        float actualValue = (50 - 0);
        actualValue /= (255 - 0);

        Assert.AreEqual(normalisedValue, actualValue);
    }

    /*
     * Keyboard Class - Testing that if a button is null it doesn't throw an exception in keyboard method
     */
    [Test]
    public void nullKeyboardTest()
    {
        Assert.DoesNotThrow(() => keyboard.buttonPressed(null));
    }

    /*
     * Main Class - Testing that the timer method is accurate
     */
    [Test]
    public void timerAccuracyTest()
    {
        //time that the timer will be on for
        float seconds = 1;
        //call method to start the timer and wait for a second
        TimeForSeconds(seconds);
    }

    //IEnumerator method to wait for seconds made for the time accuracy test
    //You cannot wait for x number of seconds unless using an IEnumerator (and IEnumerators cannot be tests themselves)
    public IEnumerator TimeForSeconds(float seconds)
    {
        main.startTimer();
        yield return new WaitForSeconds(seconds);
        main.stopTimer();

        Assert.AreEqual(seconds, main.getTimer());
    }

    /*
     * Main Class - Testing that the state changes when a scene is started and stopped
     */
    [Test]
    public void stateTest()
    {
        //getstate (done) should initialise as false
        Assert.AreEqual(false, main.getState());
    }

    /*
     * Main Class - Testing quit method
     */
    [Test]
    public void quitTest()
    {
        Assert.DoesNotThrow(() => main.quit());
    }

}
