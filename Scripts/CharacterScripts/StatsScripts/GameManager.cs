using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public int maxAttack, minAttack, manaCost;

    [Header("HP Settings")]
    [SerializeField] private int characterHP;
    [SerializeField] private int maxCharacterHP;
    public GameObject HPText;
    public GameObject HPObject_stats;
    public Bar HPBar;

    [Header("Settings for Campfire (HP)")]
    public GameObject HPText_campfire;
    public GameObject HPBar_campfire;

    [Header("Stats settings")]
    public int character_defence;
    public GameObject defenceObject;
    public GameObject defenceObject_stats;
    public int character_attack;
    public GameObject attackObject;
    public GameObject attackObject_stats;
    public int character_magicpower;
    public GameObject magicObject;
    public GameObject magicObject_stats;
    public int character_speed;
    public GameObject speedObject;
    public GameObject speedObject_stats;
    public GameObject ManaObject_stats;

    [Header("Skill settings")]
    public GameObject skill1_object;
    public GameObject skill2_object;
    public GameObject skill3_object;
    public GameObject skill4_object;

    [Header("Mana settings")]
    private int characterMana;
    [SerializeField] private int maxCharacterMana;
    public GameObject ManaText;
    public Bar ManaBar;

    [Header("Settings for Campfire (MP)")]
    public GameObject ManaText_campfire;
    public GameObject ManaBar_campfire;

    public GameObject treasureObject;

    [Header("Character Settings")]
    public Transform character;
    public GameObject characterSkullObject;

    [Header("Skill number settings")]
    public float startStunChance = 70;
    public float startBlockChance = 50;
    public float startAttackKoefficient = 1f;
    public float startMagicKoefficient = 1.5f;
    public int maxWeight;

    [Header("Another")]
    [HideInInspector]
    public bool IsFighting;
    public GameObject merchantObject;
    [HideInInspector]
    public bool IsClosedStore = false;
    [HideInInspector]
    public int UsedPotions;
    [HideInInspector]
    public int turnsToDefend;
    [HideInInspector]
    public bool IsStunned;
    private bool IsBlocked = false;
    private bool TriggerAttacking;
    private int minusValue;
    private AudioClip _enemyClip;
    public GameObject turnBar;
    public GameObject characterNameObject;

    private bool isUsingGoodAndEvil;
    private bool isShield;
    private int addedDefence;

    private float GetNormalizedValue(int currentValue, int maxValue) => (float)((float)currentValue / (float)maxValue);

    public void SetGoodAndEvil(bool value) => isUsingGoodAndEvil = value;

    public void SetShield(bool value) {
        if (!value) {
            if (isShield) {
                character_defence -= addedDefence;
                isShield = false;
            }
        } else {
            character_defence += 10;
            addedDefence += 10;
            isShield = true;
        }
    }

    public bool TryUseMana(int value) {
        if (isUsingGoodAndEvil) {
            TryDealDamage(value);
            isUsingGoodAndEvil = false;
            return false;
        }

        if (value <= characterMana) {
            if (maxCharacterMana != 0)
                ManaBar.SetChanging(GetNormalizedValue(characterMana, maxCharacterMana), GetNormalizedValue(characterMana - ((value <= characterMana) ? value : 0), maxCharacterMana));
            else Debug.Log("Can't implement change because max character HP is 0");

            characterMana -= (value <= characterMana) ? value : 0;

            return true;
        }

        return false;
    }

    public bool TryDealDamage(int value) {
        bool result = false;

        if (value <= characterHP) {
            if (maxCharacterHP != 0)
                HPBar.SetChanging(GetNormalizedValue(characterHP, maxCharacterHP), GetNormalizedValue(characterHP - ((value <= characterHP) ? value : characterHP), maxCharacterHP));
            else Debug.Log("Can't implement change because max character HP is 0");

            result = true;
        }

        characterHP -= (value <= characterHP) ? value : characterHP;

        return result;
    }

    public void HealHP(int value) {
        if (maxCharacterHP != 0)
            HPBar.SetChanging(GetNormalizedValue(characterHP, maxCharacterHP), GetNormalizedValue(characterHP + Mathf.Min(value, maxCharacterHP - characterHP), maxCharacterHP));
        else Debug.Log("Can't implement change because max character HP is 0");
        characterHP += Mathf.Min(value, maxCharacterHP - characterHP);
    }

    public void HealMP(int value) {
        if (maxCharacterMana != 0)
            ManaBar.SetChanging(GetNormalizedValue(characterMana, maxCharacterMana), GetNormalizedValue(characterMana + Mathf.Min(value, maxCharacterMana - characterMana), maxCharacterMana));
        else Debug.LogError("Can't implement change because max character MP is 0");

        characterMana += Mathf.Min(value, maxCharacterMana - characterMana);
    }

    public int GetHP() => characterHP;

    public int GetMP() => characterMana;

    public void SetHP(int value) {
        if (maxCharacterHP != 0)
            HPBar.SetChanging(GetNormalizedValue(characterHP, maxCharacterHP), GetNormalizedValue(value > maxCharacterHP ? maxCharacterHP : value, maxCharacterHP));
        else Debug.Log("Can't implement change because max character HP is 0");
        characterHP = value > maxCharacterHP ? maxCharacterHP : value;
    }

    public void SetMP(int value) => characterMana = value > maxCharacterMana? maxCharacterMana : value;

    public void SetMaxHP(int value) {
        maxCharacterHP = value;
        TryDealDamage(-1);
        TryDealDamage(1);
    }

    public void SetMaxMP(int value) {
        maxCharacterMana = value;
        TryUseMana(-1);
        TryUseMana(1);
    }

    public int GetMaxHP() => maxCharacterHP;
    
    public int GetMaxMP() => maxCharacterMana;

    public void SaveData() {
        PlayerPrefs.SetInt("Player_HP", characterHP);
        PlayerPrefs.SetInt("Player_maxHP", maxCharacterHP);
        PlayerPrefs.SetInt("Player_defence", character_defence);
        PlayerPrefs.SetInt("Player_magicpower", character_magicpower);
        PlayerPrefs.SetInt("Player_speed", character_speed);
        PlayerPrefs.SetInt("Player_attack", character_attack);
        PlayerPrefs.SetInt("Player_MP", characterMana);
        PlayerPrefs.SetInt("Player_maxMP", maxCharacterMana);
    }

    public void LoadData() {
        characterHP = PlayerPrefs.GetInt("Player_HP");
        maxCharacterHP = PlayerPrefs.GetInt("Player_maxHP");
        character_defence = PlayerPrefs.GetInt("Player_defence");
        character_magicpower = PlayerPrefs.GetInt("Player_magicpower");
        character_speed = PlayerPrefs.GetInt("Player_speed");
        character_attack = PlayerPrefs.GetInt("Player_attack");
        characterMana = PlayerPrefs.GetInt("Player_MP");
        maxCharacterMana = PlayerPrefs.GetInt("Player_maxMP");
    }

    public void LoadData_level() {
        if (SceneManager.GetActiveScene().buildIndex == 1) {
            characterHP = maxCharacterHP;
            characterMana = maxCharacterMana;
        }
        else LoadData();

        characterNameObject.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("CharacterName");
    }

    public void SetCharacterSettings() {
        HPText.GetComponent<TextMeshProUGUI>().text = $"HP: {characterHP.ToString()}/{maxCharacterHP.ToString()}";

        HPText_campfire.GetComponent<Text>().text = characterHP + "/" + maxCharacterHP;
        HPBar_campfire.GetComponent<Image>().fillAmount = (float)characterHP / (float)maxCharacterHP;

        ManaText.GetComponent<TextMeshProUGUI>().text = $"MP: {characterMana.ToString()}/{maxCharacterMana.ToString()}";

        ManaText_campfire.GetComponent<Text>().text = characterMana + "/" + maxCharacterMana;
        ManaBar_campfire.GetComponent<Image>().fillAmount = (float)characterMana / (float)maxCharacterMana;

        attackObject_stats.GetComponent<Text>().text = "Attack: " + character_attack;
        defenceObject_stats.GetComponent<Text>().text = "Defence: " + character_defence;
        magicObject_stats.GetComponent<Text>().text = "Magic power: " + character_magicpower;
        speedObject_stats.GetComponent<Text>().text = "Speed: " + character_speed;
        HPObject_stats.GetComponent<Text>().text = "Max HP: " + maxCharacterHP;
        ManaObject_stats.GetComponent<Text>().text = "Max mana: " + maxCharacterMana;

        if (characterHP <= 0)
            characterHP = 0;
        if (characterMana <= 0)
            characterMana = 0;

        if (characterHP >= maxCharacterHP)
            characterHP = maxCharacterHP;
        if (characterMana >= maxCharacterMana)
            characterMana = maxCharacterMana;


        //earned_money_text.color = new Color(earned_money_text.color.r, earned_money_text.color.g, earned_money_text.color.b, earned_money_text_color);
        //earned_money_image.color = new Color(earned_money_image.color.r, earned_money_image.color.g, earned_money_image.color.b, earned_money_text_color);
    }

    public void Start() {
        LoadData_level();

        merchantObject.SetActive(false);
        treasureObject.SetActive(false);

        IsFighting = false;
    }

    private void Update() {
        SaveData();
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        turnBar.SetActive(IsFighting);
        SetCharacterSettings();
    }
}
