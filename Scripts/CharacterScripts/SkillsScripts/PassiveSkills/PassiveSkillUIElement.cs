using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassiveSkillUIElement : MonoBehaviour {
    [SerializeField] private GameObject spriteObject;
    [SerializeField] private GameObject textObject;
    [SerializeField] private GameObject circleObject;

    public void UpdateInformation(Sprite skillSprite, int cooldown, int currentTurn, bool isUsed) {
        spriteObject.GetComponent<Image>().sprite = skillSprite;

        if (currentTurn % cooldown == 0) {
            textObject.GetComponent<TextMeshProUGUI>().text = "";
            circleObject.GetComponent<Image>().fillAmount = (isUsed? 0 : 1);
        } else {
            int turnsLeft = cooldown - (currentTurn % cooldown);
            textObject.GetComponent<TextMeshProUGUI>().text = "" + turnsLeft;
            circleObject.GetComponent<Image>().fillAmount = (float)((float)turnsLeft / (float)cooldown);
        }
    }

    public void UpdateInformation_MakeActiveTillUsage(Sprite skillSprite, int lastUsed, int currentTurn, int cooldown) {
        spriteObject.GetComponent<Image>().sprite = skillSprite;

        if (currentTurn - lastUsed >= cooldown) {
            textObject.GetComponent<TextMeshProUGUI>().text = "";
            circleObject.GetComponent<Image>().fillAmount = 0;
        } else {
            int turnsLeft = cooldown - (currentTurn - lastUsed);
            textObject.GetComponent<TextMeshProUGUI>().text = "" + turnsLeft;
            circleObject.GetComponent<Image>().fillAmount = (float)((float)turnsLeft / (float)cooldown);
        }
    }
}
