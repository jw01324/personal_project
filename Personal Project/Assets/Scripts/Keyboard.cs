using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Keyboard : MonoBehaviour
{
    public AudioSource click;
    public TextMeshProUGUI textUI;
    private SatNav satnav;

    private void Start()
    {
        textUI.text = "";
        satnav = GameObject.FindGameObjectWithTag("SatNav").GetComponent<SatNav>();
    }

    public void buttonPressed(Button button)
    {
        if (button != null)
        {
            if (button.gameObject.name == "Space")
            {
                //add a space
                textUI.text += " ";
            }
            else if (button.gameObject.name == "Submit")
            {
                //call the submit method of the satnav script
                satnav.submitWord();

            }
            else if (button.gameObject.name == "Backspace")
            {
                //checking if the length of the text already written is greater than zero (hence no substring available)
                if (textUI.text.Length > 0)
                {
                    //set the string to a substring of the text which is the exact same but not including the last character
                    textUI.text = textUI.text.Substring(0, textUI.text.Length - 1);
                }

            }
            else
            {
                //add the letter to the string
                textUI.text += button.gameObject.name.ToString();
            }
        }

        click.Play();
        
    }
}
