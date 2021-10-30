using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///In damage array first 2 elements contain the values of damage attack
/// </summary>
public class FireballSkill : SplashSkill
{
    [System.Serializable] private struct EnemyOffset {
        public float offsetX;
        public EnemyType enemyType;
    }

    [Header("Fireball settings")]
    [SerializeField] private GameObject fireballObject;
    [SerializeField] private Transform startFireballPositionObject;
    [SerializeField] private EnemyOffset[] enemyOffsets = new EnemyOffset[EnemyBase.START_ENEMY_NUMBER];
    
    [Header("Audio settings")]
    [SerializeField] private AudioClip fireballHitSound;
    [SerializeField] private AudioClip fireballExplosionSound;
    private int chosenPosition;
    
    private void DealDamageFireball(int position) {
        int[] damageFireball = new int[EnemyBase.START_ENEMY_NUMBER];

        if (chosenPosition < combatSystem.GetEnemyAmount() - 1) {
            damageFireball[position] = base.GetDamage(0);
            damageFireball[position + 1] = base.GetDamage(0);
        } else {
            damageFireball[position] = base.GetDamage(0);
        }

        combatSystem.TakeMultipleDamage(damageFireball);
    }

    private float GetOffsetByType(EnemyType enemyType) {
        for (int i = 0; i < enemyOffsets.Length; ++i) {
            if(enemyOffsets[i].enemyType == enemyType)
                return enemyOffsets[i].offsetX;
        }

        Debug.LogError($"The enemy with type {enemyType.ToString()} was not found");
        return 0f;
    }

    public override void Use(int position) {
        RaiseOnSkillStartedPlayingEvent();
        UseMana();

        GameObject createdFireball = Instantiate(fireballObject, startFireballPositionObject.position, Quaternion.identity);
        chosenPosition = position;

        GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerAnimations>().SetCastingState(PlayerAnimations.PlayerState.FireCasting, true);
        createdFireball.GetComponent<FireballObject>().SetFireballSettings(0f, combatSystem.GetEnemy(position).enemyObject.transform);
        createdFireball.GetComponent<FireballObject>().StartMoving(GetOffsetByType(combatSystem.GetEnemy(position).enemyType));

        createdFireball.GetComponent<FireballObject>().onExplosion += FireballSkill_onExplosion;
        createdFireball.GetComponent<FireballObject>().onDamage += FireballSkill_onDamage;
        createdFireball.GetComponent<FireballObject>().onEndAnimation += () => RaiseSkillPlayedEvent();
        createdFireball.GetComponent<FireballObject>().onEndAnimation += () => GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerAnimations>().SetCastingState(PlayerAnimations.PlayerState.FireCasting, false);
    }

    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) {
        RaiseOnSkillStartedPlayingEvent();

        GameObject createdFireball = Instantiate(fireballObject, startFireballPositionObject.position, Quaternion.identity);
        chosenPosition = position;

        createdFireball.GetComponent<FireballObject>().SetFireballSettings(0f, combatSystem.GetEnemy(position).enemyObject.transform);
        createdFireball.GetComponent<FireballObject>().StartMoving(GetOffsetByType(combatSystem.GetEnemy(position).enemyType));

        createdFireball.GetComponent<FireballObject>().onExplosion += FireballSkill_onExplosion;
        createdFireball.GetComponent<FireballObject>().onDamage += FireballSkill_onDamage;
        createdFireball.GetComponent<FireballObject>().onEndAnimation += () => MakeActionDelay(actionOnEnd);
    }

    private void FireballSkill_onExplosion() {
        PlaySound(fireballHitSound);
        MakeShaking();
    }

    public override int GetScaledDamage() => (int)(0.9f * (float)gameManager.character_magicpower);

    private void FireballSkill_onDamage() {
        MakeShaking();
        PlaySound(fireballExplosionSound);
        DealDamageFireball(chosenPosition);
    }
}
