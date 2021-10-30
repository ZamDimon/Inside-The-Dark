using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSystem : MonoBehaviour
{
    public Skill[] skillBase;

    [System.Serializable]
    public struct skillCell {
        [HideInInspector] public Skill skill;
        public GameObject skillObject;
        public GameObject chosenObject;
    }

    private int chosenObject = 0;
    public float fadingSpeed = 1f;
    public skillCell[] skillCells = new skillCell[4];

    public skillCell GetCurrentSkillCell() {
        return skillCells[chosenObject];
    }

    private void InitializeStartSkills() {
        SetSkill(0, 1);
        SetSkill(1, 4);
        SetSkill(2, 0);
        SetSkill(3, 2);
    }

    private void Start() {
        InitializeStartSkills();
    }

    public void SetSkill (int cellNumber, int skillNumber) {
        if (cellNumber < 0 || cellNumber > 3) {
            Debug.LogError("Cell number must have the value between 0 and 3");
            return;
        } 

        if (skillNumber < 0 || skillNumber >= skillBase.Length) {
            Debug.LogError("Out of range");
            return;
        }

        skillCells[cellNumber].skill = skillBase[skillNumber];
    }
    
    private void SetVisibility () {
        for (int i = 0; i < skillCells.Length; ++i) {
            skillCells[i].skillObject.GetComponent<Image>().overrideSprite = skillCells[i].skill.skillSprite;

            string attackDescription = "";
            attackDescription += ((skillCells[i].skill.GetScaledDamage() > 0)? $"Deals {skillCells[i].skill.GetScaledDamage() - 1}-{skillCells[i].skill.GetScaledDamage() + 1} damage \n" : "");
            attackDescription += ((skillCells[i].skill.GetManaCost() > 0)? $"Costs {skillCells[i].skill.GetManaCost()} mana points \n" : "");

            skillCells[i].skillObject.GetComponent<SkillCell>().SetInfoSettings(skillCells[i].skill.skillName, skillCells[i].skill.skillDescription, skillCells[i].skill.skillSprite, attackDescription);

            if (chosenObject == i) {
                if (skillCells[i].chosenObject.GetComponent<Image>().fillAmount < 1)
                    skillCells[i].chosenObject.GetComponent<Image>().fillAmount += Time.deltaTime*fadingSpeed;
            }
            else if (skillCells[i].chosenObject.GetComponent<Image>().fillAmount > 0)
                skillCells[i].chosenObject.GetComponent<Image>().fillAmount -= Time.deltaTime*fadingSpeed;
        }
    }

    public void SetCurrentSkill(int number) {
        chosenObject = number;
    }

    private void ActivateSkillsUsingKeys() {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetCurrentSkill(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetCurrentSkill(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetCurrentSkill(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SetCurrentSkill(3);
    }

    private void Update() {
        SetVisibility();
        ActivateSkillsUsingKeys();
    }
}
