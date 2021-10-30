using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelUpScript : MonoBehaviour
{
    public int characterExperience;
    public int neededExperience;
    public int currentLevel = 1;

    private int levelUpPoints_stats;

    public GameObject XPBar_object;
    public GameObject XPText_object;

    public int maxLevel;

    public GameObject LevelTextObject;
    public GameObject[] buttons_stats;

    public GameObject upgradesNumberText;

    private GameObject mainObject;

    [SerializeField] private float increasingCoefficient = 1.5f;
    [SerializeField] private int startXP = 20;
    [SerializeField] private Sprite notificationSprite;

    private Notification notification;

    [Header("Tree settings")]
    [SerializeField] private GameObject treePointsTextObject;
    [SerializeField] private GameObject treePointsTextObject_treeInterface;
    private int treePoints = 1;

    public int GetTreePoints() => treePoints;
    public void AddTreePoint(int value) => treePoints += value;

    public void SaveData() {
        PlayerPrefs.SetInt("Player_XP", characterExperience);
        PlayerPrefs.SetInt("Player_neededXP", neededExperience);
        PlayerPrefs.SetInt("Player_lvl", currentLevel);
        PlayerPrefs.SetInt("Player_lvl_stats", levelUpPoints_stats);
        PlayerPrefs.SetInt("Player_maxlvl", maxLevel);
    }

    public void LoadData() {
        characterExperience = PlayerPrefs.GetInt("Player_XP");
        neededExperience = PlayerPrefs.GetInt("Player_neededXP");
        currentLevel = PlayerPrefs.GetInt("Player_lvl");
        levelUpPoints_stats = PlayerPrefs.GetInt("Player_lvl_stats");
        maxLevel = PlayerPrefs.GetInt("Player_maxlvl");
    }

    private void Start() {
        if (SceneManager.GetActiveScene().buildIndex != 1)
            LoadData();

        mainObject = GameObject.FindGameObjectWithTag("MainCamera");
        notification = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Notification>();
    }

    private void SetTextVisibility () {
        upgradesNumberText.SetActive((levelUpPoints_stats) > 0);
        upgradesNumberText.GetComponent<Text>().text = "" + (levelUpPoints_stats);

        treePointsTextObject.GetComponent<Text>().text = "" + GetTreePoints().ToString();
        treePointsTextObject.SetActive(GetTreePoints() > 0);

        treePointsTextObject_treeInterface.GetComponent<TextMeshProUGUI>().text = "Tree points available: " + GetTreePoints().ToString(); 
    }

    public void LevelUp() {
        AddXP(neededExperience - characterExperience);
    }
    public void AddXP(int value) {
        characterExperience += value;

        if (characterExperience >= neededExperience) {
            characterExperience -= neededExperience;
            neededExperience = (int)(neededExperience * 1.4f);
            notification.ShowNotification(notificationSprite, "Level up!");

            ++currentLevel;
            ++levelUpPoints_stats;
            AddTreePoint(1);
        }
    }

    private void SetXPInterface() {
        XPBar_object.GetComponent<Image>().fillAmount = (float)characterExperience / (float)neededExperience;
        XPText_object.GetComponent<TextMeshProUGUI>().text = characterExperience + "/" + neededExperience;
        LevelTextObject.GetComponent<Text>().text = "Character level: " + currentLevel;
        SetTextVisibility();
        MakeVisibleButtons();
    }

    private void Update() {
        SetXPInterface();
    }

    private void MakeVisibleButtons() {
        for (int i = 0; i < buttons_stats.Length; ++i) 
            buttons_stats[i].SetActive(levelUpPoints_stats > 0);
    }

    public void ClickOn_stats (int id) {
        if (levelUpPoints_stats <= 0)
            return;

        --levelUpPoints_stats;
        switch (id) {
            case 0:
                mainObject.GetComponent<GameManager>().SetMaxHP(mainObject.GetComponent<GameManager>().GetMaxHP() + 3);
                mainObject.GetComponent<GameManager>().HealHP(3);
                break;

            case 1:
                mainObject.GetComponent<GameManager>().SetMaxMP(mainObject.GetComponent<GameManager>().GetMaxMP() + 5);
                mainObject.GetComponent<GameManager>().HealMP(5);
                break;

            case 2:
                mainObject.GetComponent<GameManager>().character_attack++;
                break;

            case 3:
                mainObject.GetComponent<GameManager>().character_magicpower++;
                break;

            case 4:
                mainObject.GetComponent<GameManager>().character_speed++;
                break;

            case 5:
                mainObject.GetComponent<GameManager>().character_defence++;
                break;
        }
    }
}
