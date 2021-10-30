using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyInfoPanel : MonoBehaviour {
    private GameObject nullRefObject;

    [System.Serializable] private struct UIDebuffElement {
        public GameObject debuffImage;
        public GameObject durancyText;
        public EffectType effectType;
    }

    [System.Serializable] private struct ResistTextPanel {
        public GameObject textObject;
        public EffectType effectType;
    }

    #region Links to other scripts
    private EnemySettings enemySettings;
    private CombatSystem combatSystem;
    private int enemyIndex;
    public void SetEnemyIndex(int value) => enemyIndex = value; 
    #endregion

    #region Links to the interface elements
    [Header("Info texts settings")]
    [SerializeField] private GameObject HPStats;
    [SerializeField] private GameObject totalEffectDamageText;
    [Header("Debuffs settings")]
    [SerializeField] private List<UIDebuffElement> UIDebuffsElements = new List<UIDebuffElement>();
    [SerializeField] private Color unactiveDebuffColor;
    [SerializeField] private Color activeDebuffColor;

    [Header("Resistances settings")]
    [SerializeField] private List<ResistTextPanel> resistanceTexts = new List<ResistTextPanel>();
    #endregion

    private void Awake() {
        combatSystem = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CombatSystem>();
        enemySettings = transform.GetComponent<EnemySettings>();
    } 

    private void UpdateHPStats() {
        int currentHP = combatSystem.GetEnemy(enemyIndex).GetHP();
        int maxHP = combatSystem.GetEnemy(enemyIndex).GetMaxHealth();
        string labelColor = "";

        float currentRatio = (float)(Mathf.Max(0, (float)currentHP) / (float)maxHP);
        if (currentRatio <= 0.33f) 
            labelColor = "red";
        else if (currentRatio > 0.33f && currentRatio <= 0.67f)
            labelColor = "yellow";
        else labelColor = "green";

        HPStats.GetComponent<TextMeshProUGUI>().text = $"HP: <color={labelColor}> {currentHP}/{maxHP} </color>";
    }

    private void UpdateTotalEffectDamageText() {
        int totalEffectDamage = combatSystem.GetEnemy(enemyIndex).GetEffectDamage();
        totalEffectDamageText.GetComponent<TextMeshProUGUI>().text = $"Damage from debuffs:<color=red> {totalEffectDamage.ToString()} </color>"; 
    }

    private void UpdateEffectsInterface() {
        for (int i = 0; i < UIDebuffsElements.Count; ++i) {
            int currentDurancy = combatSystem.GetEnemy(enemyIndex).GetEffectDurancy(UIDebuffsElements[i].effectType);
            if (currentDurancy <= 0) {
                UIDebuffsElements[i].durancyText.SetActive(false);
                UIDebuffsElements[i].debuffImage.GetComponent<Graphic>().color = new Color(unactiveDebuffColor.r, unactiveDebuffColor.g, unactiveDebuffColor.b, UIDebuffsElements[i].debuffImage.GetComponent<Graphic>().color.a);
            } else {
                UIDebuffsElements[i].durancyText.SetActive(true);
                UIDebuffsElements[i].durancyText.GetComponent<TextMeshProUGUI>().text = currentDurancy.ToString(); 
                UIDebuffsElements[i].debuffImage.GetComponent<Graphic>().color = new Color(activeDebuffColor.r, activeDebuffColor.g, activeDebuffColor.b, UIDebuffsElements[i].debuffImage.GetComponent<Graphic>().color.a);
            }
        }
    }

    private void UpdateResistPanels() {
        int effectAmout = combatSystem.GetEnemy(enemyIndex).effectResists.Length; 

        for (int i = 0; i < effectAmout; ++i) {
            for (int j = 0; j < effectAmout; ++j) {
                if (resistanceTexts[i].effectType == combatSystem.GetEnemy(enemyIndex).effectResists[j].effectType) {
                    resistanceTexts[i].textObject.GetComponent<TextMeshProUGUI>().text = 
                    $"{combatSystem.GetEnemy(enemyIndex).GetResist(resistanceTexts[i].effectType).ToString()}%";
                }
            }
        }
    }

    private void UpdateInfoPanel() {
        UpdateHPStats();
        UpdateTotalEffectDamageText();
        UpdateEffectsInterface();
        UpdateResistPanels();
    }

    private void Update() {
        enemyIndex = enemySettings.GetNumber();
        UpdateInfoPanel();
    }
}
