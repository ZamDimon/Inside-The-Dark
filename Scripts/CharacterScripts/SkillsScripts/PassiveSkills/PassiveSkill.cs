using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : Skill {
    public override void Use(int position) {
        //base.Use(position);
        audioSource.PlayOneShot(useSound);
    }

    public override int GetScaledDamage() => 0;
}
