using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle : MonoBehaviour {
    //Turn order array. The links to the enemies' positions are put here
    private List<int> turnOrder = new List<int>();

    #region Events
    public event Action OnTurnChanged;
    public event Action OnStartBattle;
    public event Action OnBeginCharacterTurn;
    #endregion

    #region Labels & Boxes settings
    [Header("Labels&Boxes settings")]
    public GameObject healObject;
    public GameObject skillObject;
    public GameObject skillBox;
    public GameObject turnBox;
    public GameObject turnText;
    public float timeBeforeTurn = 2f;
    public GameObject DefendStatusObject;
    public GameObject DefendStatusTextObject;
    public GameObject passiveSkillsList;
    #endregion

    #region Links on another scripts
    private BattleAnimationScript battleAnimationScript;
    private CombatSystem combatSystem;
    private GameManager gameManager;
    private TurnNumberAnimation turnNumberAnimation;
    private CharacterTurnHandler characterTurnHandler;
    private PassiveSkillsManager passiveSkillManager;
    #endregion

    #region Some other stuff
    public GameObject soundObject;
    public float durationBetween = 0.5f;
    #endregion

    private bool IsEqualToArrayElement (int a, List<int> arr) {
        for (int i = 0; i < arr.Count; ++i) {
            if (arr[i] == a)
                return true;
        }

        return false;
    }

    public void RemoveEnemy(int id) {
        for (int i = 0; i < turnOrder.Count; ++i) {
            if (turnOrder[i] == id) {
                turnOrder.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < turnOrder.Count; ++i) {
            if (turnOrder[i] > id)
                turnOrder[i]--;
        }

        OnTurnChanged?.Invoke();
    }

    public void RemoveEnemyByID(int id) { 
        turnOrder.RemoveAt(id); 
        OnTurnChanged?.Invoke();
    }

    private void DrawArr (List<int> arr) {
        string output = "";
        foreach (int number in arr)
            output += number + " ";
        Debug.Log(output);
    }

    public void GenerateTurnOrder() {
        turnOrder.Clear();
        passiveSkillsList.SetActive(true);

        for (int i = 0; i < combatSystem.GetEnemyAmount() + 1; ++i) {
            int currentIndex = UnityEngine.Random.Range(0, combatSystem.GetEnemyAmount() + 1);
            while (IsEqualToArrayElement(currentIndex, turnOrder)) 
                currentIndex = UnityEngine.Random.Range(0, combatSystem.GetEnemyAmount() + 1);
            
            turnOrder.Add(currentIndex);
        }

        OnStartBattle?.Invoke();
    }

    public static void Swap <T> (ref List<T> list, int index1, int index2) {
        T tmp = list[index1];
        list[index1] = list[index2];
        list[index2] = tmp;
    }

    public void SwapTurnOrder(int index1, int index2) { 
        Swap(ref turnOrder, index1, index2);
    }

    public void SetNewTurn() {
        for (int i = 0; i < turnOrder.Count - 1; ++i)
            Swap(ref turnOrder, i, i + 1);
        OnTurnChanged?.Invoke();
    }
    
    public int GetCurrentTurn() => turnOrder[0];

    public int GetTurnByID(int ID) => turnOrder[ID];

    public Sprite GetEnemySprite(int ID) => combatSystem.GetEnemy(turnOrder[ID] - 1).enemyIcon;

    public void RemoveIntFromTurnOrder(int index, int value){ 
        turnOrder[index] -= value;
        OnTurnChanged?.Invoke();
    }

    public bool CompareToTurnOrders(int index1, int index2) => turnOrder[index1] > turnOrder[index2];

    public int GetEnemyAmount() => turnOrder.Count;
    
    private bool IsDeathFromEffects(int enemyID) => combatSystem.GetEnemy(enemyID).GetEffectDamage() >= combatSystem.GetEnemy(enemyID).GetHP();

    private bool isPlayingTurn = false;
    public void SetPlayingTurn(bool value) => isPlayingTurn = value;

    public void EndFighting() {
        if (!combatSystem.IsSomeoneAlive()) {
            gameManager.IsFighting = false;
            soundObject.GetComponent<AudioSource>().clip = battleAnimationScript.passiveSound;
            soundObject.GetComponent<AudioSource>().Play();
            GameObject.FindGameObjectWithTag("Character").GetComponent<Animator>().SetBool("IsAttacking", false);
            characterTurnHandler.RemoveAllTurns();
            turnNumberAnimation.SetCurrentTurnToZero();
            passiveSkillsList.SetActive(false);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().SetShield(false);
            isPlayingTurn = true;
        }
    }

    private int DealDamageEnemy(EnemyType enemyType) {
        EnemyBase enemyBase = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<EnemyBase>();
        GameManager gameManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();

        int damage = (int)((1f - (float)gameManager.character_defence / 100f) * (float)UnityEngine.Random.Range(enemyBase.GetEnemyByType(enemyType).minDamage, enemyBase.GetEnemyByType(enemyType).maxDamage + 1));

        if (passiveSkillManager.IsCanBeUsedSkill(PassiveSkillsManager.PassiveSkillType.DivineShield) &&
            passiveSkillManager.IsOpenedSkill(PassiveSkillsManager.PassiveSkillType.DivineShield)) {
            passiveSkillManager.SetCannotBeUsed(PassiveSkillsManager.PassiveSkillType.DivineShield);
            GameObject.FindGameObjectWithTag("SkillManager").GetComponent<HolyShieldSkill>().PlayBreakingShieldAnimation();
            passiveSkillManager.UpdateSkills();
            damage = 0;
        }
        else gameManager.TryDealDamage(damage);

        return damage;
    }
    private void AttackEnemy (EnemyType enemyType) {
        int dealtDamage = DealDamageEnemy(enemyType);
        battleAnimationScript.AttackCharacter(GetCurrentTurn() - 1, dealtDamage);
        
        /*switch (enemyType) {
            case EnemyType.Dog:
                battleAnimationScript.AttackCharacter(GetCurrentTurn() - 1, dealtDamage);
                break;
            case EnemyType.Skeleton:
                battleAnimationScript.AttackCharacter(GetCurrentTurn() - 1, dealtDamage);
                break;
            case EnemyType.Slave:
                battleAnimationScript.AttackCharacter(GetCurrentTurn() - 1, dealtDamage);
                break;
            case EnemyType.Ghost:
                battleAnimationScript.AttackCharacter(GetCurrentTurn() - 1, dealtDamage);
                break;
        }*/
    }
    public IEnumerator DoTurnEnemy () {
        if (turnOrder[0] == 0) {
            if (combatSystem.IsSomeoneAlive_HP())
                turnNumberAnimation.PlayAnimation(characterTurnHandler.GenerateTurnAmount(), () => OnBeginCharacterTurn?.Invoke());
            yield break;
        }
        else {
            GameObject.FindGameObjectWithTag("SkillManager").GetComponent<Tentacles>().SetIncreasingCoefficient(1f);
            GameObject.FindGameObjectWithTag("SkillManager").GetComponent<PentagramSkill>().SetIncreasingCoefficient(1f);
        }

        while (isPlayingTurn) {
            yield return null;
        }

        isPlayingTurn = true;

        int currentID = turnOrder[0] - 1;
        bool willBeDead = IsDeathFromEffects(currentID), isStunned = combatSystem.GetEnemy(currentID).IsSkipping();

        combatSystem.GetEnemy(currentID).ApplyEffects();

        if (willBeDead) {
            isPlayingTurn = false;
            yield break;
        } else if (isStunned) {
            isPlayingTurn = false;
            SetNewTurn();
            MakeTurnDelay();
            yield break;
        }
        else AudioManager.PlaySound(combatSystem.GetEnemy(currentID).beforeAttackSound);

        yield return new WaitForSeconds(timeBeforeTurn);

        AttackEnemy(combatSystem.GetEnemy(currentID).enemyType);
        SetNewTurn();

        isPlayingTurn = false;  
    }

    private void InitializeLinksToScripts() {
        battleAnimationScript = transform.GetComponent<BattleAnimationScript>();
        combatSystem = transform.GetComponent<CombatSystem>();
        gameManager = transform.GetComponent<GameManager>();
        turnNumberAnimation = transform.GetComponent<TurnNumberAnimation>();
        characterTurnHandler = transform.GetComponent<CharacterTurnHandler>();
        passiveSkillManager = transform.GetComponent<PassiveSkillsManager>();
    }

    private void SubscribeToEvents() {
        battleAnimationScript.onEndAnimationEnemy += () => MakeTurnDelay();
        //battleAnimationScript.onEndAnimationCharacter += () => { SetNewTurn(); MakeTurnDelay(); };
        combatSystem.OnDieAnimation += CombatSystem_OnDieAnimation;
        Skill[] skills = GameObject.FindGameObjectWithTag("SkillManager").GetComponents<Skill>();
        foreach (Skill skill in skills) {
            skill.OnSkillPlayed += () => { 
                characterTurnHandler.DecreaseTurnAmount();
                if (characterTurnHandler.GetTurnAmount() <= 0) {
                    SetNewTurn(); 
                    MakeTurnDelay();
                } else StartCoroutine(MakeGiveTurnDelay(0.5f));
            };
        }
    }

    private void Awake() {
        InitializeLinksToScripts();
        GenerateTurnOrder();
        SubscribeToEvents();
    }

    #region Event Receiver Methods
    private void CombatSystem_OnDieAnimation(int index) {
        if (index == turnOrder[0] - 1) {
            isPlayingTurn = false;
            StartCoroutine(makeTurnWithDelay(durationBetween));
        }

        RemoveEnemy(index + 1);
        EndFighting();
    }
    #endregion

    private IEnumerator makeTurnWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        MakeTurn();
    }
    
    public void MakeTurnDelay() => StartCoroutine(makeTurnWithDelay(durationBetween));
    private IEnumerator MakeGiveTurnDelay(float delay) {
        yield return new WaitForSeconds(delay);
        OnBeginCharacterTurn?.Invoke();
    } 
    public void MakeTurn() => StartCoroutine(DoTurnEnemy());
}
