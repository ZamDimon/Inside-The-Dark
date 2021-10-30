using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SplashSkill : Skill {
    [System.Serializable] public struct damageRange {
        public int minDamage, maxDamage;
        public damageRange(int value) {
            minDamage = value - 1;
            maxDamage = value + 1;
        }
    }

    [Header("Damage & effects settings")]
    [SerializeField] protected damageRange[] damageRanges = new damageRange[4];
    [SerializeField] protected Effect[] effects = new Effect[4];
    [SerializeField] protected int[] rollChances = new int[4];

    protected override void Update() {
        base.Update();

        for (int i = 0; i < damageRanges.Length; ++i)
            if (damageRanges[i].minDamage > 0 && damageRanges[i].maxDamage > 0)
                damageRanges[i] = new damageRange(GetScaledDamage());
    }

    public int GetDamage(int index) => UnityEngine.Random.Range(damageRanges[index].minDamage, damageRanges[index].maxDamage + 1);

    private IEnumerator showObject(GameObject animationObject, float fadingSpeed, float criticalAlpha) {
        while (animationObject.GetComponent<SpriteRenderer>().color.a < criticalAlpha) {
            float additionValue = Mathf.Min(criticalAlpha - animationObject.GetComponent<SpriteRenderer>().color.a, fadingSpeed * Time.deltaTime);
            animationObject.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, additionValue);

            yield return null;
        }
    }
    private IEnumerator hideObject(GameObject animationObject, float fadingSpeed) {
        while (animationObject.GetComponent<SpriteRenderer>().color.a > 0) {
            float additionValue = Mathf.Min(animationObject.GetComponent<SpriteRenderer>().color.a, fadingSpeed * Time.deltaTime);
            animationObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, additionValue);

            yield return null;
        }
    }
    protected void ShowObject (GameObject animationObject, float fadingSpeed, float criticalAlpha) => StartCoroutine(showObject(animationObject, fadingSpeed, criticalAlpha));
    protected void HideObject(GameObject animationObject, float fadingSpeed) => StartCoroutine(hideObject(animationObject, fadingSpeed));
    protected void DealDamage() {
        int[] damage = new int[4];
        for (int i = 0; i < 4; ++i)
            damage[i] = GetDamage(i);

        combatSystem.TakeMultipleDamage(damage);
        for (int i = 0; i < combatSystem.GetCellsLength(); ++i) {
            combatSystem.GetEnemy(i).AddEffect(rollChances[i], effects[i]);
        }
    }

    public override abstract void Use(int position);
    public abstract void UseWithoutTurnSpending(int position, Action actionOnEnd);
    public override abstract int GetScaledDamage();
}
