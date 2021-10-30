using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sacrifice : SplashSkill {
    public override void Use(int position) {
        if (gameManager.GetHP() > gameManager.GetMaxHP() / 5) {
            transform.GetComponent<Tentacles>().SetIncreasingCoefficient(1.8f);
            transform.GetComponent<PentagramSkill>().SetIncreasingCoefficient(1.8f);
            gameManager.TryDealDamage(gameManager.GetMaxHP() / 5);

            RaiseOnSkillStartedPlayingEvent();
            RaiseSkillPlayedEvent();
        }
    }
    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) => throw new System.NotImplementedException();
    public override int GetScaledDamage() => 0;
}
