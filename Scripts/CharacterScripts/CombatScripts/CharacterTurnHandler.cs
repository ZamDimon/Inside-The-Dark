using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTurnHandler : MonoBehaviour {
    public enum CharacterTurnAmount {
        OneTwo = 0,
        Two = 1,
        TwoThree = 2,
        ThreeFour = 3
    }

    private CharacterTurnAmount currentCharacterTurnAmount;

    private const int MIN_TURN_AMOUNT = 1;
    private const int MAX_TURN_AMOUNT = 4;
    private int currentTurnAmount = 0;
    
    private bool canAttack = false;

    private Battle battle;
    private Skill[] skills;
    private TurnNumberAnimation turnNumberAnimation;

    public bool GetAbilityToAttack() => canAttack;
    private bool SetAttack(bool value) => canAttack = value;
    public int GetTurnAmount() => currentTurnAmount;
    public void AddTurnAmount() => currentTurnAmount++;
    public void DecreaseTurnAmount() => currentTurnAmount -= Mathf.Min(currentTurnAmount, 1);
    public void RemoveAllTurns() => currentTurnAmount = 0;

    public void SetCharacterTurnAmount(CharacterTurnAmount characterTurnAmount) => currentCharacterTurnAmount = characterTurnAmount;

    private void SubscribeToEvents() {
        battle = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Battle>();
        skills = GameObject.FindGameObjectWithTag("SkillManager").GetComponents<Skill>();
        turnNumberAnimation = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TurnNumberAnimation>();

        battle.OnBeginCharacterTurn += () => SetAttack(true);
        for (int i = 0; i < skills.Length; ++i) skills[i].OnSkillStartedPlaying += () => SetAttack(false);
    }

    private void Awake() => SubscribeToEvents();

    public int GenerateTurnAmount() {
        //TODO: Here I will put some really difficult and complex calculations of how many turns the player will have. 
        //But for now I am just gonna leave this random generator:
        int generatedValue = 0;

        switch (currentCharacterTurnAmount) {
            case CharacterTurnAmount.OneTwo:
                int chance_OneTwo = Random.Range(0, 101);
                generatedValue = ((chance_OneTwo <= 25) ? 2 : 1);
                break;
            case CharacterTurnAmount.Two: generatedValue = 2; break;
            case CharacterTurnAmount.TwoThree: generatedValue = Random.Range(2, 4); break;
            case CharacterTurnAmount.ThreeFour:
                int chance_ThreeFour = Random.Range(0, 101);
                generatedValue = ((chance_ThreeFour <= 20)? 4 : 3);
                break;
            default: generatedValue = 1; break;
        }

        currentTurnAmount = generatedValue;

        if (generatedValue == 0) {
            Debug.LogError("Generator value can't be 0!");
            return 1;
        }

        return generatedValue;
    }
}
