using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueWriting : MonoBehaviour
{
    #region Public Variables
    public GameObject textObject;
    public float delay_textTyping;
    public float delay_betweenLines;
    public AudioClip[] typingSounds;
    public string[] whatToWrite;
    #endregion

    #region Private Variables
    private float timer;
    private int currIndex_line;
    private int currIndex_letter;
    private int currIndex_sound;
    #endregion

    int getArrayLength(string[] lines) {
        int counter = 0;

        for (int i = 0; i < lines.Length; ++i) {
            if (lines[i] != "")
                ++counter;
        }

        return counter;
    }

    public void AddToArray(string lineToAdd) {
        for (int i = 0; i < whatToWrite.Length; ++i) {
            if (whatToWrite[i] == "") {
                whatToWrite[i] = lineToAdd;
                return;
            }
        }
    }

    private void Start() {
        //For private variables
        timer = 0f;
        currIndex_line = 0;
        currIndex_letter = 0;
        currIndex_sound = 0;

        textObject.GetComponent<Text>().text = "";
    }

    private void Update() {
        timer += Time.deltaTime;

        if (timer >= delay_textTyping && currIndex_letter < whatToWrite[currIndex_line].Length) {
            AudioSource.PlayClipAtPoint(typingSounds[currIndex_sound], GameObject.FindGameObjectWithTag("MainCamera").transform.position);
            textObject.GetComponent<Text>().text += whatToWrite[currIndex_line][currIndex_letter];

            currIndex_sound = Random.Range(0, typingSounds.Length);
            ++currIndex_letter;
            timer = 0f;
        }

        if (currIndex_letter == whatToWrite[currIndex_line].Length && currIndex_line < getArrayLength(whatToWrite) - 1 && timer >= delay_betweenLines) {
            textObject.GetComponent<Text>().text = "";
            ++currIndex_line;
            currIndex_letter = 0;

            timer = 0f;
        }
    }
}
