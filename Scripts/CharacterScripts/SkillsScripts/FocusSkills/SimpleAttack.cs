using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAttack : FocusAttackSkill {
    public override void Use(int position) {
        base.Use(position);
    }

    public override int GetScaledDamage() => gameManager.character_attack;
}
