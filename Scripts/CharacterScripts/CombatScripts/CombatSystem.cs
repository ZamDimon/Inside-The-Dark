using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System;

public class CombatSystem : MonoBehaviour
{
    private const float MOVING_ERROR = 0.01f;

    #region Variables
    #region Links to other scripts
    private EnemyBase enemyBase;
    private MoneyManager moneyManager;
    private LevelUpScript levelUpScript;
    #endregion

    #region Events
    public delegate void Attacked(int index, int damage, bool playAnimation);
    public event Attacked OnAttacked;
    public void InvokeAttacked(int index, int damage, bool playAnimation) => OnAttacked?.Invoke(index, damage, playAnimation);
    
    public delegate void Died(int index);
    public event Died OnDied;
    public void InvokeDied(int index) => OnDied?.Invoke(index);

    public delegate void endDyingAnimation(int index);
    public event endDyingAnimation OnDieAnimation;
    public void InvokeEndDyingAnimation(int index) => OnDieAnimation?.Invoke(index);

    public delegate void Stunned(int index);
    public event Stunned onStun;
    public void InvokeStunned(int index) => onStun?.Invoke(index);
    #endregion

    #region Enemy settings
    [System.Serializable] public struct EnemyCell {
        public int index;
        public Enemy enemy;
        public GameObject position;

        public void RemoveEnemy() => enemy.RemoveEnemy();
        public void InitializeEnemy() => enemy.InitializeEnemy();
        public void UpdateIndex() {
            if (enemy.enemyObject != null) {
                enemy.enemyObject.GetComponent<EnemySettings>().SetNumber(index);
                enemy.SetCombatSystem(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CombatSystem>());
                enemy.SetIndexInBattle(index);
            }
        }
    }

    [SerializeField] private EnemyCell[] enemyCells = new EnemyCell[4];
    #endregion

    #region Enemy Position Settings
    [Header("Enemy position settings")]
    private List<Coroutine> movementCoroutines = new List<Coroutine>();
    [SerializeField] private float rotationSpeed;
    #endregion

    #region HPBarColors
    public Color highHPColor;
    public Color midHPColor;
    public Color lowHPColor;
    #endregion

    #endregion

    public Enemy GetEnemy(int index) {
        if (index < 0 || index >= enemyCells.Length) {
            Debug.Log($"Index {index.ToString()} is out of range");
            return null;
        }

        return enemyCells[index].enemy;
    } 
    public int GetEnemyAmount (EnemyCell[] cells) {
        int amount = 0;

        for (int i = 0; i < cells.Length; ++i)
            if (cells[i].enemy.enemyObject != null && cells[i].enemy.health > 0)
                ++amount;

        return amount;
    }
    public void SetEnemyTexture(int index, Texture2D texture) => enemyCells[index].enemy.SetTexture(texture); 
    public int GetCellsLength() => enemyCells.Length;
    public int GetEffectDamage(int index) => enemyCells[index].enemy.GetEffectDamage();
    public void SetEqual_cells (ref EnemyCell enemy1, ref EnemyCell enemy2) {
        enemy1.enemy = enemy2.enemy;
    }

    private void LoadFromBase(int enemyCellID, int baseID) {
        enemyCells[enemyCellID].enemy.minDamage = enemyBase.enemies[baseID].minDamage;
        enemyCells[enemyCellID].enemy.maxDamage = enemyBase.enemies[baseID].maxDamage;
        enemyCells[enemyCellID].enemy.enemyAnimationObject = enemyBase.enemies[baseID].enemyAnimationObject;
        enemyCells[enemyCellID].enemy.enemyAttackSound = enemyBase.enemies[baseID].enemyAttackSound;
        enemyCells[enemyCellID].enemy.beforeAttackSound = enemyBase.enemies[baseID].beforeAttackSound;
        enemyCells[enemyCellID].enemy.maxHealth = enemyBase.enemies[baseID].maxHealth;
        enemyCells[enemyCellID].enemy.health = enemyBase.enemies[baseID].maxHealth;
        enemyCells[enemyCellID].enemy.XPReward = enemyBase.enemies[baseID].XPReward;
        enemyCells[enemyCellID].enemy.abilityName = enemyBase.enemies[baseID].abilityName;
        enemyCells[enemyCellID].enemy.attackingTexture = enemyBase.enemies[baseID].attackingTexture;
        enemyCells[enemyCellID].enemy.defendingTexture = enemyBase.enemies[baseID].defendingTexture;
        enemyCells[enemyCellID].enemy.basicMissChance = enemyBase.enemies[baseID].basicMissChance;
        enemyCells[enemyCellID].enemy.skullObject = enemyBase.enemies[baseID].skullObject;
        enemyCells[enemyCellID].enemy.enemyTextBarObject = enemyBase.enemies[baseID].enemyTextBarObject;
        enemyCells[enemyCellID].enemy.stunObject = enemyBase.enemies[baseID].stunObject;
        enemyCells[enemyCellID].enemy.skillText = enemyBase.enemies[baseID].skillText;
        enemyCells[enemyCellID].enemy.enemyIcon = enemyBase.enemies[baseID].enemyIcon;
        enemyCells[enemyCellID].enemy.effects = enemyBase.enemies[baseID].effects;
        enemyCells[enemyCellID].enemy.offset = enemyBase.enemies[baseID].offset;
        enemyCells[enemyCellID].enemy.enemyType = enemyBase.enemies[baseID].enemyType;
        enemyCells[enemyCellID].enemy.effectResists = enemyBase.enemies[baseID].effectResists;
        enemyCells[enemyCellID].enemy.moneyReward = enemyBase.enemies[baseID].moneyReward;
    }

    public void InitializeEnemies() {
        for (int i = 0; i < enemyCells.Length; ++i) {
            enemyCells[i].enemy.health = enemyCells[i].enemy.maxHealth;
            enemyCells[i].InitializeEnemy();
        }
    }

    public int GetEnemyAmount () {
        int counter = 0;
        for (int i = 0; i < enemyCells.Length; ++i)
            if (enemyCells[i].enemy.enemyObject != null)
                ++counter;

        return counter;
    }

    public void RemoveEnemy(int index) {
        moneyManager.AddMoney(enemyCells[index].enemy.moneyReward);
        levelUpScript.AddXP(enemyCells[index].enemy.XPReward);

        if (enemyCells[index].enemy.enemyObject != null)
            enemyCells[index].RemoveEnemy();

        OnDieAnimation?.Invoke(index);

        for (int i = index; i < enemyCells.Length - 1; ++i) 
            SwapEnemies(ref enemyCells[i], ref enemyCells[i+1]);
    }

    private void UpdateEnemyHP() {
        foreach (EnemyCell cell in enemyCells) {
            if (cell.enemy.enemyObject == null)
                continue;

            cell.enemy.enemyObject.GetComponent<EnemySettings>().enemyHealthBarObject.GetComponent<Image>().fillAmount = (float)cell.enemy.health / (float)cell.enemy.maxHealth;
            cell.enemy.enemyObject.GetComponent<EnemySettings>().enemyTextBarObject.GetComponent<TextMeshProUGUI>().text = "" + Mathf.Max(0, cell.enemy.health) + "/" + cell.enemy.maxHealth;

            Color colorToHighlight = Color.white; float ratio = (float)cell.enemy.health / (float)cell.enemy.maxHealth;
            if (ratio >= 0.6666f)
                colorToHighlight = highHPColor;
            else if (ratio >= 0.3333f && ratio <= 0.6666f)
                colorToHighlight = midHPColor;
            else colorToHighlight = lowHPColor;

            cell.enemy.enemyObject.GetComponent<EnemySettings>().enemyHealthBarObject.GetComponent<Image>().color = colorToHighlight;
        }
    }

    public void AddEnemyFront(int baseID) {
        if (GetEnemyAmount(enemyCells) >= enemyCells.Length)
            return;

        for (int i = enemyCells.Length - 1; i > 0; --i) 
            SwapEnemies(ref enemyCells[i], ref enemyCells[i-1]);
        
        LoadFromBase(0, baseID);
        enemyCells[0].enemy.enemyObject = (GameObject)Instantiate(enemyBase.enemies[baseID].enemyObject, enemyCells[0].position.transform.position, Quaternion.identity);
    }

    public bool IsSomeoneAlive() {
        for (int i = 0; i < enemyCells.Length; ++i) {
            if (enemyCells[i].enemy.enemyObject != null)
                return true;
        }

        return false;
    }

    public bool IsSomeoneAlive_HP() {
        for (int i = 0; i < enemyCells.Length; ++i) {
            if (enemyCells[i].enemy.GetHP() > 0)
                return true;
        }

        return false;
    }

    public void UpdateEnemyNumbers () {
        int enemiesAlive = 0;
        for (int i = 0; i < enemyCells.Length; ++i) {
            enemyCells[i].UpdateIndex();
            if (enemyCells[i].enemy.enemyObject != null) {
                ++enemiesAlive;
            }
        }
    }
 
    private void SwapEnemies(ref EnemyCell enemyCell1, ref EnemyCell enemyCell2) {
        EnemyCell _enemyCell = new EnemyCell();

        SetEqual_cells(ref _enemyCell, ref enemyCell2);
        SetEqual_cells(ref enemyCell2, ref enemyCell1);
        SetEqual_cells(ref enemyCell1, ref _enemyCell);

        DisableCoroutines();
        UpdateEnemyPositions(enemyCells);
    }

    private IEnumerator moveToPosition(EnemyCell cell, float speed) {
        if (cell.enemy.enemyObject == null)
            yield break;

        while (Vector3.Distance(cell.enemy.enemyObject.transform.position, cell.position.transform.position + cell.enemy.offset) > MOVING_ERROR) {
            cell.enemy.SetPosition(Vector3.Lerp(cell.enemy.enemyObject.transform.position, cell.position.transform.position + cell.enemy.offset, speed * Time.deltaTime));
            yield return null;
        }
    }

    public void UpdateEnemyPositions (EnemyCell[] cells) {
        DisableCoroutines();
        for (int i = 0; i < cells.Length; ++i) {
            Coroutine action = StartCoroutine(moveToPosition(cells[i], rotationSpeed));
            movementCoroutines.Add(action);
        }
    }

    public void DisableCoroutines() {
        for (int i = 0; i < movementCoroutines.Count; ++i) {
            if (movementCoroutines[i] != null)
                StopCoroutine(movementCoroutines[i]);
        }

        movementCoroutines.Clear();
    }

    public void TakeMultipleDamage (int[] damage) {
        if (damage.Length != enemyCells.Length) {
            Debug.LogError("The size of damage array is not equal to the size of the enemy array");
            return;
        }

        for (int i = enemyCells.Length - 1; i >= 0; --i) {
            if (enemyCells[i].enemy.enemyObject == null)
                continue;

            enemyCells[i].enemy.TakeDamage(damage[i], enemyCells[i].enemy.health > 0 && damage[i] > 0);
        }
    }

    private void UpdateEffects() {
        foreach (EnemyCell enemyCell in enemyCells) {
            enemyCell.enemy.UpdateEffects();
        }
    }

    private void Update() {
        UpdateEnemyHP();
        UpdateEnemyNumbers();
        UpdateEffects();
    }

    private void SetStartSettings() {
        enemyBase = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<EnemyBase>();
        moneyManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MoneyManager>();
        levelUpScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<LevelUpScript>();

        InitializeEnemies();

        for (int i = 0; i < enemyCells.Length; ++i)
            enemyCells[i].index = i;

        for (int i = 0; i < 4; ++i)
            RemoveEnemy(0);
    }
    private void Start() {
        SetStartSettings();
    }
}
