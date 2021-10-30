using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FocusAttackSkill : Skill {
    public Texture characterAttackSprite;
    public int minRange;
    public int maxRange;
    public int damage = 8;
    public int missChance;

    protected override void Update() {
        base.Update();
        damage = Random.Range(GetScaledDamage() - 1, GetScaledDamage() + 2);
    }

    protected void DealDamage(int position) {
        int[] damageArray = new int[4];
        for (int i = 0; i < 4; ++i)
            damageArray[i] = 0;

        damageArray[position] = damage;
        combatSystem.TakeMultipleDamage(damageArray);
    }

    public override void Use(int position) {
        //base.Use(position);
        if (position < minRange || position > maxRange) {
            Debug.LogError($"This skill can't be used on position {position.ToString()}");
            return;
        }

        audioSource.PlayOneShot(useSound);

        RaiseOnSkillStartedPlayingEvent();

        int chance = Random.Range(0, 100);
        if (chance < missChance) {
            battleAnimationScript.AttackEnemy_focused(position, damage, this, AnimationStatement.Miss, RaiseSkillPlayedEvent);
        } else {
            DealDamage(position);
            battleAnimationScript.AttackEnemy_focused(position, damage, this, AnimationStatement.Success, RaiseSkillPlayedEvent);
        }
    }

    public override abstract int GetScaledDamage();
}
