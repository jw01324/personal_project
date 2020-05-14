using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Class for the Keyboard GameObject.
 * A virtual keyboard operated by the user to input characters when the satnav type is programmable
 */
public class Keyboard : MonoBehaviour
{
    //GameObjects
    public AudioSource click;
    public TextMeshProUGUI textUI;
    private SatNav satnav;

    // Start is called before the first frame update
    void Start()
    {
        //initialise the variables
        textUI.text = "";
        satnav = GameObject.FindGameObjectWithTag("SatNav").GetComponent<SatNav>();
    }

    /*
     * Method for converting a button press on the virtual keyboard to the respective letter/number/action that it represents.
     * Note: All button objects for the keyboard were named the same as the character or string on the button.
     * For example: the 'A' button is a button object named "A", the '2' button is a button object named "2", the spacebar is a button object named "Space", etc.
     */
    public void buttonPressed(Button button)
    {
        //checks if the button parameter is null
        if (button != null)
        {
            //checks which button has been pressed, and acts accordingly
            if (button.gameObject.name == "Space")
            {
                //if the button pressed is space (the spacebar), add a space to the string
                textUI.text += " ";
            }
            else if (button.gameObject.name == "Submit")
            {
                //if the button pressed is submit (the submit button), call the submit method of the satnav script
                satnav.submitWord();

            }
            else if (button.gameObject.name == "Backspace")
            {
                //if the button pressed is backspace (the backspace button), check if the length of the current text is greater than zero (hence no substring available)
                if (textUI.text.Length > 0)
                {
                    //if current text is greater than zero, then set the string to a substring of the text (which is the exact same just without the last character)
                    textUI.text = textUI.text.Substring(0, textUI.text.Length - 1);
                }
            }
            else
            {
                //if the button pressed is anything else, it must be a singular character (letter, or number)
                //so add the name of the button to the string (as the name of the button is the same as the name of the character)
                textUI.text += button.gameObject.name.ToString();
            }
        }

        //play the click sound effect (keyboard click) once
        click.PlayOneShot(click.clip);
    }

}
