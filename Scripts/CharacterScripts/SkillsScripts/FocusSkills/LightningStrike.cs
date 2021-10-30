using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : FocusAttackSkill {
    public override void Use(int position) {
        UseMana();
        base.Use(position);
    }

    public override int GetScaledDamage() => gameManager.character_magicpower;
}
