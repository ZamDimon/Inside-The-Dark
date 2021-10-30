using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    const float INFINITY = 100500f;

    public GameObject textObject, exclamationObject, buttonObject;
    private float timer;
    public float timeToShow;
    public float fadingSpeed;
    public AudioClip hintSound;

    #region Triggers
    [HideInInspector]
    public bool movementCharacterTrigger = false;
    [HideInInspector]
    public bool OpenTheDoorTrigger = false;
    [HideInInspector]
    public bool FightTrigger = false;
    [HideInInspector]
    public bool AfterFirstEnemyAttack = false;
    [HideInInspector]
    public bool UpgradingSkills = false;
    [HideInInspector]
    public bool TrapTrigger = false;
    [HideInInspector]
    public bool BestiaryHint = false;
    #endregion

    public void Start() {
        timer = 0f;
    }

    public void ShowHint (string text) {
        textObject.GetComponent<Text>().text = text;
        AudioSource.PlayClipAtPoint(hintSound, GameObject.FindGameObjectWithTag("MainCamera").transform.position);
        timer = 0f;
    }

    public void CloseHint () {
        timer = INFINITY;
    }

    private void Update() {
        if (timer <= timeToShow && transform.GetComponent<Image>().color.a < 1f) {
            transform.GetComponent<Image>().color = new Color(transform.GetComponent<Image>().color.r, transform.GetComponent<Image>().color.g, 
                transform.GetComponent<Image>().color.b, transform.GetComponent<Image>().color.a + Time.deltaTime * fadingSpeed);
            textObject.GetComponent<Text>().color = new Color(textObject.GetComponent<Text>().color.r, textObject.GetComponent<Text>().color.g,
                textObject.GetComponent<Text>().color.b, textObject.GetComponent<Text>().color.a + Time.deltaTime * fadingSpeed);
            exclamationObject.GetComponent<Text>().color = new Color(exclamationObject.GetComponent<Text>().color.r, exclamationObject.GetComponent<Text>().color.g,
                exclamationObject.GetComponent<Text>().color.b, exclamationObject.GetComponent<Text>().color.a + Time.deltaTime * fadingSpeed);
            buttonObject.GetComponent<Image>().color = new Color(buttonObject.GetComponent<Image>().color.r, buttonObject.GetComponent<Image>().color.g,
                buttonObject.GetComponent<Image>().color.b, buttonObject.GetComponent<Image>().color.a + Time.deltaTime * fadingSpeed);
        }

        if (transform.GetComponent<Image>().color.a > 0f && timer >= timeToShow) {
            transform.GetComponent<Image>().color = new Color(transform.GetComponent<Image>().color.r, transform.GetComponent<Image>().color.g,
                transform.GetComponent<Image>().color.b, transform.GetComponent<Image>().color.a - Time.deltaTime * fadingSpeed);
            textObject.GetComponent<Text>().color = new Color(textObject.GetComponent<Text>().color.r, textObject.GetComponent<Text>().color.g,
                textObject.GetComponent<Text>().color.b, textObject.GetComponent<Text>().color.a - Time.deltaTime * fadingSpeed);
            exclamationObject.GetComponent<Text>().color = new Color(exclamationObject.GetComponent<Text>().color.r, exclamationObject.GetComponent<Text>().color.g,
                exclamationObject.GetComponent<Text>().color.b, exclamationObject.GetComponent<Text>().color.a - Time.deltaTime * fadingSpeed);
            buttonObject.GetComponent<Image>().color = new Color(buttonObject.GetComponent<Image>().color.r, buttonObject.GetComponent<Image>().color.g,
                buttonObject.GetComponent<Image>().color.b, buttonObject.GetComponent<Image>().color.a - Time.deltaTime * fadingSpeed);
        }
    }
}
