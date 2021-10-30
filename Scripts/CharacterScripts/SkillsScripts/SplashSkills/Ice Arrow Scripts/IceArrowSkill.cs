using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// First element in damage array contains value of damage that will be dealt to an enemy
/// </summary>
public class IceArrowSkill : SplashSkill {
    [System.Serializable] private struct EnemyOffset {
        public float offsetX;
        public EnemyType enemyType;
    }

    [Header("Ice arrow settings settings")]
    [SerializeField] private GameObject iceArrowObject;
    [SerializeField] private Transform startIceArrowPositionObject;

    [SerializeField] private EnemyOffset[] enemyOffsets = new EnemyOffset[EnemyBase.START_ENEMY_NUMBER];

    [Header("Sound settings")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip castingSound;

    private int chosenPosition;

    protected void DealSingleDamage(int position) {
        int[] damageArray = new int[4];
        for (int i = 0; i < 4; ++i)
            damageArray[i] = 0;

        damageArray[position] = base.GetDamage(0);
        combatSystem.TakeMultipleDamage(damageArray);
    }

    private float GetOffsetByType(EnemyType enemyType) {
        for (int i = 0; i < enemyOffsets.Length; ++i) {
            if (enemyOffsets[i].enemyType == enemyType)
                return enemyOffsets[i].offsetX;
        }

        Debug.LogError($"The enemy with type {enemyType.ToString()} was not found");
        return 0f;
    }

    public override void Use(int position) {
        RaiseOnSkillStartedPlayingEvent();
        UseMana();

        GameObject createdIceArrow = Instantiate(iceArrowObject, startIceArrowPositionObject.position, Quaternion.identity);
        chosenPosition = position;

        GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerAnimations>().SetCastingState(PlayerAnimations.PlayerState.IceCasting, true);
        createdIceArrow.GetComponent<IceArrowObject>().SetIceArrowSettings(GetOffsetByType(combatSystem.GetEnemy(position).enemyType), combatSystem.GetEnemy(position).enemyObject.transform);

        createdIceArrow.GetComponent<IceArrowObject>().onAnimationEnd += IceArrowSkill_onAnimationEnd;
        createdIceArrow.GetComponent<IceArrowObject>().onExplosion += IceArrowSkill_onExplosion;

        createdIceArrow.GetComponent<IceArrowObject>().StartMoving();
        AudioManager.PlaySound(castingSound);
    }

    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) {
        RaiseOnSkillStartedPlayingEvent();

        GameObject createdIceArrow = Instantiate(iceArrowObject, startIceArrowPositionObject.position, Quaternion.identity);
        chosenPosition = position;

        createdIceArrow.GetComponent<IceArrowObject>().SetIceArrowSettings(GetOffsetByType(combatSystem.GetEnemy(position).enemyType), combatSystem.GetEnemy(position).enemyObject.transform);

        createdIceArrow.GetComponent<IceArrowObject>().onAnimationEnd += () => MakeActionDelay(actionOnEnd);
        createdIceArrow.GetComponent<IceArrowObject>().onExplosion += IceArrowSkill_onExplosion;

        createdIceArrow.GetComponent<IceArrowObject>().StartMoving();
        AudioManager.PlaySound(castingSound);
    }

    public override int GetScaledDamage() => (int)(1.5f * (float)gameManager.character_magicpower);

    private void IceArrowSkill_onAnimationEnd() {
        GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerAnimations>().SetCastingState(PlayerAnimations.PlayerState.IceCasting, false);
        RaiseSkillPlayedEvent();
    }

    private void IceArrowSkill_onExplosion() {
        MakeShaking();
        AudioManager.PlaySound(hitSound);
        DealSingleDamage(chosenPosition);
    }
}
