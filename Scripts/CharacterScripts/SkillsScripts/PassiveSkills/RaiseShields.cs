using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaiseShields : SplashSkill {

    public override void Use(int position) {
        UseMana();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().SetShield(true);
        RaiseOnSkillStartedPlayingEvent();
        RaiseSkillPlayedEvent();
    }

    public override int GetScaledDamage() => 0;
    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) => throw new System.NotImplementedException();
}
