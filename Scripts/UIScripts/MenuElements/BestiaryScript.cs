using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BestiaryScript : MonoBehaviour
{
    private enum Parameter {
        MaxHealth = 0, Damage = 1, MoneyReward = 2, XPReward = 3, 
        BleedResist = 4, StunResist = 5, FreezingResist = 6, BurningResist = 7
    }

    private EnemyBase enemyBase;

    [System.Serializable] private struct EnemyBestiary {
        public string name;
        public EnemyType enemyType;
        public GameObject sprite;
        public string description;
        [HideInInspector] public string[] stats;
        public ITEM[] items;
    }

    public GameObject[] buttons;
    public GameObject[] cellObjects;
    private int chosenID;
    public GameObject[] statsTextObjects;
    public string[] statsTexts;
    [SerializeField] private EnemyBestiary[] enemies;

    public GameObject descriptionLabel, nameLabel, darkeningObject;
    public AudioClip clipToPlay;

    private Color startColor;

    public void PressButton (int id) {
        chosenID = id;
        AudioSource.PlayClipAtPoint(clipToPlay, GameObject.FindGameObjectWithTag("MainCamera").transform.position);
    }

    private void Start() {
        chosenID = 0;
        startColor = buttons[0].GetComponent<Image>().color;
        enemyBase = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<EnemyBase>();
        
        for (int i = 0; i < enemies.Length; ++i) 
            enemies[i].stats = new string[statsTexts.Length];

        SetStatsTexts();
    }

    private void SetStatsTexts() {
        for (int i = 0; i < enemies.Length; ++i) {
            Enemy enemy = enemyBase.GetEnemyByType(enemies[i].enemyType);

            enemies[i].stats[(int)Parameter.MaxHealth] = enemy.maxHealth.ToString();
            enemies[i].stats[(int)Parameter.Damage] = enemy.minDamage.ToString() + "-" + enemy.maxDamage.ToString();
            enemies[i].stats[(int)Parameter.MoneyReward] = enemy.moneyReward.ToString();
            enemies[i].stats[(int)Parameter.XPReward] = enemy.XPReward.ToString();
            enemies[i].stats[(int)Parameter.BleedResist] = enemy.GetResist(EffectType.Blood).ToString() + '%';
            enemies[i].stats[(int)Parameter.StunResist] = enemy.GetResist(EffectType.Stun).ToString() + '%';
            enemies[i].stats[(int)Parameter.FreezingResist] = enemy.GetResist(EffectType.Freezing).ToString() + '%';
            enemies[i].stats[(int)Parameter.BurningResist] = enemy.GetResist(EffectType.Burning).ToString() + '%';
        }
    }

    private void UpdateEnemyInfo() {
        for (int i = 0; i < enemies.Length; ++i) {
            enemies[i].sprite.SetActive(i == chosenID);
        }

        nameLabel.GetComponent<TextMeshProUGUI>().text = enemies[chosenID].name;
        descriptionLabel.GetComponent<TextMeshProUGUI>().text = enemies[chosenID].description;

        for (int i = 0; i < enemies[chosenID].items.Length; ++i) {
            cellObjects[i].SetActive(true);
            cellObjects[i].GetComponent<CellScript>().SetInfoSettingsByItem(enemies[chosenID].items[i]);
        }

        for (int i = enemies[chosenID].items.Length; i < cellObjects.Length; ++i) {
            cellObjects[i].SetActive(false);
        }

        for (int i = 0; i < statsTexts.Length; ++i) 
            statsTextObjects[i].GetComponent<TextMeshProUGUI>().text = statsTexts[i] + enemies[chosenID].stats[i];
    }

    private void Update() {
        UpdateEnemyInfo();
    }
}
