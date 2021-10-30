using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunAttack : FocusAttackSkill {
    [SerializeField] protected Effect effect;
    [SerializeField] private int chance;

    public override void Use(int position) {
        base.Use(position);

        combatSystem.GetEnemy(position).AddEffect(chance, effect);
    }

    public override int GetScaledDamage() => (int)(0.3f * (float)gameManager.character_attack);
}
