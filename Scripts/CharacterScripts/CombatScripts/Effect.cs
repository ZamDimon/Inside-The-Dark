using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect {
    public int durancy;
    public EffectType effectType;
    public int power;

    public Effect(EffectType _effectType, int _durancy, int _power) {
        durancy = _durancy;
        effectType = _effectType;
        power = _power;
    }
}
