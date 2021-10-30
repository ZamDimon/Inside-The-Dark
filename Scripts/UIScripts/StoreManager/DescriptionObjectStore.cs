using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionObjectStore : MonoBehaviour
{
    private bool Checker () {
        GameObject[] cells = GameObject.FindGameObjectsWithTag("StoreCell");
        foreach (GameObject cell in cells) {
            if (cell.GetComponent<StoreButton>().IsOnHover) {
                return true;
            }
        }

        return false;
    }

    public bool IsText;

    private void Update() {
        if (IsText) 
            GetComponent<Text>().color = Checker() ? new Color(GetComponent<Text>().color.r, GetComponent<Text>().color.g, GetComponent<Text>().color.b, 1) :
                new Color(GetComponent<Text>().color.r, GetComponent<Text>().color.g, GetComponent<Text>().color.b, 0);
        else
            GetComponent<Image>().color = Checker() ? new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, 1) :
                            new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, 0);
    }
}
