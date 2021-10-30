using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnteringGameLabel : MonoBehaviour
{
    public GameObject backgroundObject;
    public GameObject textObject;

    public bool isAnimating_entering = false;
    public bool isAnimating_closing = true;

    public float closeDelay = 0.8f;
    public float fadingSpeed = 0.1f;
    float timer = 0f;

    public void SetAnimation(string text) {
        timer = 0f;
        textObject.GetComponent<Text>().text = text;

        isAnimating_entering = true;
        isAnimating_closing = false;
        backgroundObject.GetComponent<Image>().color = new Color(backgroundObject.GetComponent<Image>().color.r, backgroundObject.GetComponent<Image>().color.g, backgroundObject.GetComponent<Image>().color.b, 1);
        textObject.GetComponent<Text>().color = new Color(textObject.GetComponent<Text>().color.r, textObject.GetComponent<Text>().color.g, textObject.GetComponent<Text>().color.b, 0);
    }

    void Update() {
        timer += Time.deltaTime;

        if (isAnimating_entering) {
            if (textObject.GetComponent<Text>().color.a < 1) 
                textObject.GetComponent<Text>().color = new Color(textObject.GetComponent<Text>().color.r, textObject.GetComponent<Text>().color.g, textObject.GetComponent<Text>().color.b, textObject.GetComponent<Text>().color.a + Time.deltaTime * fadingSpeed);
        }

        if (isAnimating_closing) {
            if (backgroundObject.GetComponent<Image>().color.a > 0) {
                backgroundObject.GetComponent<Image>().color = new Color(backgroundObject.GetComponent<Image>().color.r, backgroundObject.GetComponent<Image>().color.g, backgroundObject.GetComponent<Image>().color.b, backgroundObject.GetComponent<Image>().color.a - Time.deltaTime * fadingSpeed);
                textObject.GetComponent<Text>().color = new Color(textObject.GetComponent<Text>().color.r, textObject.GetComponent<Text>().color.g, textObject.GetComponent<Text>().color.b, textObject.GetComponent<Text>().color.a - Time.deltaTime * fadingSpeed);
            }
        }

        if (timer >= closeDelay) {
            isAnimating_entering = false;
            isAnimating_closing = true;
        }
    }

    void Start() {
        SetAnimation("Chapter I: Dungeon");
    }
}
