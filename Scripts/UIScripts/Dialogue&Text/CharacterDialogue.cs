using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogue : MonoBehaviour
{
    [System.Serializable]
    public struct dialogueLine {
        public string text;
        [HideInInspector]
        public bool IsUsed;

        public dialogueLine (string text) {
            this.text = text;
            IsUsed = false;
        }
    }

    public dialogueLine[] lines;

    public float linesDelay;
    public float speechDurancy;
    public float fadingSpeed;
    public GameObject background;
    public GameObject textObject;

    private bool IsDialogue;
    private float timer;
    private Color startColor;

    private void Start() {
        startColor = background.GetComponent<Image>().color;
        background.GetComponent<Image>().color = new Color(startColor.r, startColor.g, startColor.b, 0);
    }

    private void InterfaceVisibility() {
        //For text
        textObject.SetActive(IsDialogue);

        //For text's background
        if (IsDialogue && background.GetComponent<Image>().color.a < startColor.a) {
            background.GetComponent<Image>().color = new Color(startColor.r, startColor.g, startColor.b, background.GetComponent<Image>().color.a + Time.deltaTime * fadingSpeed);
        }

        if (!IsDialogue && background.GetComponent<Image>().color.a > 0) {
            background.GetComponent<Image>().color = new Color(startColor.r, startColor.g, startColor.b, background.GetComponent<Image>().color.a - Time.deltaTime * fadingSpeed);
        }
    }

    private int amountOfUsed () {
        int amount = 0;

        for (int i = 0; i < lines.Length; ++i)
            if (lines[i].IsUsed)
                ++amount;

        return amount;
    }

    private string lineToAdd() {
        if (amountOfUsed() == lines.Length)
            return "";

        int randomID = Random.Range(0, lines.Length);

        while (lines[randomID].IsUsed)
            randomID = Random.Range(0, lines.Length);

        lines[randomID].IsUsed = true;
        return lines[randomID].text;
    }

    public void AddLine(string text) {
        textObject.GetComponent<DialogueWriting>().AddToArray(text);

        IsDialogue = true;
        timer = 0f;
    }

    private void Update() {
        InterfaceVisibility();
        
        timer += Time.deltaTime;

        if (timer >= linesDelay && !GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().IsFighting && lineToAdd() != "") {
            textObject.GetComponent<DialogueWriting>().AddToArray(lineToAdd());

            IsDialogue = true;
            timer = 0f;
        }
        
        if (IsDialogue && timer >= speechDurancy) {
            IsDialogue = false;
            timer = 0f;
        }
    }
}
