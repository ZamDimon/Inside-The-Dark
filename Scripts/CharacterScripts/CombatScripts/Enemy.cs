using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct EffectResist {
    public int resist;
    public EffectType effectType;
}

public class EnemyActions : MonoBehaviour {
    public static void DeleteObject(GameObject objectToDelete) => Destroy(objectToDelete);
    public static void SetTexture(GameObject objectToChange, Texture texture) => objectToChange.GetComponent<RawImage>().texture = texture;
}

[System.Serializable]
public class Enemy {
    private CombatSystem combatSystem;
    public void SetCombatSystem(CombatSystem value) => combatSystem = value;
    private int indexInBattle;
    public void SetIndexInBattle(int value) => indexInBattle = value;

    #region Enemy Properties
    [HideInInspector] public int minDamage;
    [HideInInspector] public int maxDamage;
    [HideInInspector] public GameObject enemyObject;
    [HideInInspector] public GameObject enemyAnimationObject;
    [HideInInspector] public AudioClip beforeAttackSound;
    [HideInInspector] public AudioClip enemyAttackSound;
    [HideInInspector] public int health;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int XPReward;
    [HideInInspector] public string abilityName;
    [HideInInspector] public Texture2D attackingTexture;
    [HideInInspector] public Texture2D defendingTexture;
    [HideInInspector] public float basicMissChance;
    [HideInInspector] public GameObject skullObject;
    [HideInInspector] public GameObject enemyTextBarObject;
    [HideInInspector] public GameObject stunObject;
    [HideInInspector] public string skillText;
    [HideInInspector] public Sprite enemyIcon;
    [HideInInspector] public List<Effect> effects;
    [HideInInspector] public Vector3 offset;
    [HideInInspector] public EnemyType enemyType;
    [HideInInspector] public EffectResist[] effectResists = new EffectResist[4];
    [HideInInspector] public int moneyReward;

    public Enemy() {
        enemyAnimationObject = null;
        minDamage = 0;
        maxDamage = 0;
        enemyObject = null;
        enemyAnimationObject = null;
        enemyAttackSound = null;
        beforeAttackSound = null;
        maxHealth = 0;
        health = 0;
        attackingTexture = null;
        defendingTexture = null;
        basicMissChance = 0;
        skullObject = null;
        enemyTextBarObject = null;
        stunObject = null;
        enemyIcon = null;
        skillText = "";
        effects = new List<Effect>();
        offset = Vector3.zero;
        enemyType = EnemyType.Empty;
    }
    #endregion

    public int GetHealth() => health;
    public int GetMaxHealth() => maxHealth;
    public int SetHealth(int value) => (value > maxHealth) ? maxHealth : value;
    public int SetMaxHealth(int value) => maxHealth = value;
    public Texture2D GetDefendingTexture() => defendingTexture;
    public Texture2D GetAttackingTexture() => attackingTexture;
    public int GetHP() => health;
    public int GetResist(EffectType effectType) {
        for (int i = 0; i < effectResists.Length; ++i) {
            if (effectResists[i].effectType == effectType)
                return effectResists[i].resist;
        }

        Debug.Log("Effect was not found");
        return -1;
    }
    public void SetTexture(Texture2D texture) => EnemyActions.SetTexture(enemyAnimationObject, texture);
    public void SetPosition(Vector3 newPosition) => enemyObject.transform.position = newPosition;
    public void RemoveEnemy() {
        enemyObject.GetComponent<EnemySettings>().UnsubscribeFromEvents();
        if (enemyObject != null)
            EnemyActions.DeleteObject(enemyObject);
        enemyAnimationObject = null;
        minDamage = 0;
        maxDamage = 0;
        enemyObject = null;
        enemyAnimationObject = null;
        enemyAttackSound = null;
        maxHealth = 0;
        health = 0;
        attackingTexture = null;
        defendingTexture = null;
        basicMissChance = 0;
        skullObject = null;
        enemyTextBarObject = null;
        stunObject = null;
        enemyIcon = null;
        skillText = "";
        effects = new List<Effect>();
        offset = Vector3.zero;
        enemyType = EnemyType.Empty;
    }
    
    public void InitializeEnemy() {
        health = maxHealth;
        effects = new List<Effect>();
    }

    public void AddEffect(int chance, Effect effect) {
        if (effect.durancy <= 0 || effect.effectType == EffectType.Empty || enemyObject == null)
            return;

        if (RandomManager.Roll(100 - chance + GetResist(effect.effectType))) {
            Debug.Log("Resist worked!");
            return;
        }

        for (int i = 0; i < effects.Count; ++i) {
            if (effect.effectType == effects[i].effectType) {
                effects[i] = new Effect(effect.effectType, effects[i].durancy + 1, effects[i].power + 1);
                return;
            }
        }

        effects.Add(effect);
    }

    public int GetEffectDamage() {
        int _damage = 0;

        for (int i = 0; i < effects.Count; ++i) {
            switch (effects[i].effectType) {
                case EffectType.Blood:
                    _damage += 2 * effects[i].power;
                    break;
            }
        }

        return _damage;
    }
    
    public bool IsSkipping() {
        for (int i = 0; i < effects.Count; ++i) {
            if (effects[i].effectType == EffectType.Stun) {
                return true;
            }
        }

        return false;
    }

    public int GetEffectDurancy(EffectType effectType) {
        for (int i = 0; i < effects.Count; ++i) {
            if(effects[i].effectType == effectType) {
                return effects[i].durancy;
            }
        }

        return 0;
    }

    public void ApplyEffects() {
        int _damage = 0;

        for (int i = 0; i < effects.Count; ++i) {
            enemyObject.GetComponent<EnemySettings>().ShowEffect(effects[i].effectType);

            switch (effects[i].effectType) {
                case EffectType.Blood:
                    _damage += 2 * effects[i].power;
                    break;
            }

            Effect effect = new Effect(effects[i].effectType, effects[i].durancy - 1, effects[i].power);

            effects[i] = effect;
            if (effects[i].durancy <= 0) {
                enemyObject.GetComponent<EnemySettings>().effectObjects[(int)effects[i].effectType].effectObject.SetActive(false);
                effects.RemoveAt(i);
                --i;
            }
        }

        if (_damage > 0)
            TakeDamage(_damage, true);
    }

    public void UpdateEffects() {
        foreach (Effect effect in effects) {
            if (enemyObject.GetComponent<EnemySettings>().effectObjects[(int)effect.effectType].effectObject != null)
                enemyObject.GetComponent<EnemySettings>().effectObjects[(int)effect.effectType].effectObject.SetActive(true);
        }
    }

    private float GetNormalizedValue(int value1, int value2) => (float)((float)value1 / (float)value2);

    public void TakeDamage(int damage, bool playAnimation) {
        if (enemyObject == null) {
            Debug.LogError($"You can't attack enemy on position {indexInBattle.ToString()}");
            return;
        }

        enemyObject.GetComponent<EnemySettings>().GetBar().SetChanging(GetNormalizedValue(health, maxHealth), GetNormalizedValue(Mathf.Max(0, health - damage), maxHealth));

        health -= damage;
        combatSystem.InvokeAttacked(indexInBattle, damage, playAnimation && health > 0 && damage > 0);
        PassiveSkillsManager passiveSkillsManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PassiveSkillsManager>();
        CharacterTurnHandler characterTurnHandler = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CharacterTurnHandler>();

        if (health <= 0) {
            combatSystem.InvokeDied(indexInBattle);

            if (passiveSkillsManager.IsOpenedSkill(PassiveSkillsManager.PassiveSkillType.BloodBath) &&
                passiveSkillsManager.IsCanBeUsedSkill(PassiveSkillsManager.PassiveSkillType.BloodBath))
                characterTurnHandler.AddTurnAmount();
        }
        else {
            if (passiveSkillsManager.IsOpenedSkill(PassiveSkillsManager.PassiveSkillType.WeakPoints))
                AddEffect(30, new Effect(EffectType.Blood, 2, 1));
        }
    }
}
