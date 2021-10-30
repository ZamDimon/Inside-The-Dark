using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MageUltimate : SplashSkill {
    private SkillSystem skillSystem;

    [SerializeField] private FireballSkill fireballSkill;
    [SerializeField] private IceArrowSkill iceArrowSkill;
    [SerializeField] private IceWallSkill iceWallSkill;

    [Header("Sound settings")]
    [SerializeField] private AudioClip sound;

    private void Start() {
        skillSystem = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SkillSystem>();   
    }

    public override void Use(int position) {
        RaiseOnSkillStartedPlayingEvent();
        PlaySound(sound);
        UseMana();
        GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerAnimations>().SetCastingState(PlayerAnimations.PlayerState.UltimateWizard, true);

        /*fireballSkill.UseWithoutTurnSpending(UnityEngine.Random.Range(0, combatSystem.GetEnemyAmount()), () => {
            iceWallSkill.UseWithoutTurnSpending(UnityEngine.Random.Range(0, combatSystem.GetEnemyAmount()), () => {
                iceArrowSkill.UseWithoutTurnSpending(UnityEngine.Random.Range(0, combatSystem.GetEnemyAmount()), () => {
                    GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterMovement>().SetUltimateSkillCasingState(false);
                    RaiseSkillPlayedEvent();
                });
            });
        });*/

        PlaySkill(fireballSkill, () => PlaySkill(iceWallSkill, () => PlaySkill(iceArrowSkill, EndUltimateSkill)));
    }

    private void EndUltimateSkill() {
        GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerAnimations>().SetCastingState(PlayerAnimations.PlayerState.UltimateWizard, false);
        RaiseSkillPlayedEvent();
    }

    private void PlaySkill(SplashSkill splashSkill, Action actionOnEnd) {
        if (combatSystem.GetEnemyAmount() <= 0) {
            EndUltimateSkill();
            return;
        }

        splashSkill.UseWithoutTurnSpending(UnityEngine.Random.Range(0, combatSystem.GetEnemyAmount()), actionOnEnd);
    }

    public override int GetScaledDamage() => 0;
    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) => throw new System.NotImplementedException();
}
