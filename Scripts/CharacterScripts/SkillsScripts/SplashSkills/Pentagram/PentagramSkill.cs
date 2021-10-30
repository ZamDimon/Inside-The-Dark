using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PentagramSkill : SplashSkill {
    [SerializeField] private List<GameObject> pentagramObjects = new List<GameObject>();
    [SerializeField] private List<Sprite> pentagramAnimation = new List<Sprite>();
    [SerializeField] private float delayBetweenFrames = 0.2f;
    [SerializeField] private float fadingSpeed_appearing = 2f;
    [SerializeField] private float fadingSpeed_disappearing = 2f;
    [SerializeField] private AudioClip sound;

    private float increasingCoefficient = 1.0f;
    public void SetIncreasingCoefficient(float value) => increasingCoefficient = value;

    private IEnumerator ShowObject(GameObject pentagramObject, float speed, Action actionOnEnd) {
        while (pentagramObject.GetComponent<SpriteRenderer>().color.a < 1) {
            pentagramObject.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, speed * Time.deltaTime);
            yield return null;
        }

        actionOnEnd?.Invoke();
    }

    private IEnumerator IEPlayPentagramAnimation (GameObject pentagramObject, List<Sprite> frames, Action actionOnEnd) {
        int currentFrame = 0;

        while (currentFrame < frames.Count) {
            pentagramObject.GetComponent<SpriteRenderer>().sprite = frames[currentFrame++];
            yield return new WaitForSeconds(delayBetweenFrames);
        }

        actionOnEnd?.Invoke();
    }

    private void PlayPentagramAnimation(GameObject pentagramObject, List<Sprite> frames, Action actionOnEnd) =>
        StartCoroutine(IEPlayPentagramAnimation(pentagramObject, frames, actionOnEnd));

    protected int DealSingleDamage(int position) {
        int[] damageArray = new int[4];
        for (int i = 0; i < 4; ++i)
            damageArray[i] = 0;

        damageArray[position] = UnityEngine.Random.Range(GetScaledDamage() - 1, GetScaledDamage() + 1);
        combatSystem.TakeMultipleDamage(damageArray);
        gameManager.HealHP((int)(0.2f * (float)damageArray[position]));
        return damageArray[position];
    }

    private IEnumerator HidePentagramObject(GameObject pentagramObject, float speed) {
        while (pentagramObject.GetComponent<SpriteRenderer>().color.a >= 0) {
            pentagramObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, speed * Time.deltaTime);
            yield return null;
        }
    }

    public override void Use(int position) {
        RaiseOnSkillStartedPlayingEvent();
        PlaySound(sound);
        UseMana();
        pentagramObjects[position].GetComponent<SpriteRenderer>().sprite = pentagramAnimation[0];

        GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerAnimations>().SetCastingState(PlayerAnimations.PlayerState.Warlock, true);
        StartCoroutine(ShowObject(pentagramObjects[position], fadingSpeed_appearing, () => {
            PlayPentagramAnimation(pentagramObjects[position], pentagramAnimation, () => {
                gameManager.HealHP(DealSingleDamage(position));

                MakeShaking();
                RaiseSkillPlayedEvent();
                GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerAnimations>().SetCastingState(PlayerAnimations.PlayerState.Warlock, false);

                StartCoroutine(HidePentagramObject(pentagramObjects[position], fadingSpeed_disappearing));
            });
        }));
    }

    public override int GetScaledDamage() => (int)(0.9f * (float)gameManager.character_magicpower * increasingCoefficient);
    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) => throw new System.NotImplementedException();

}
