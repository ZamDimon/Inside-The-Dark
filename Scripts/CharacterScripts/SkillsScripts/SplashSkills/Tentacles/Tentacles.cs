using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tentacles : SplashSkill {
    [System.Serializable] public struct CharacterFrame {
        public Sprite characterFrame;
        public float delay;
    }

    [Header("Portal animations")]
    [SerializeField] private List<CharacterFrame> beginningFrames = new List<CharacterFrame>();
    [SerializeField] private List<CharacterFrame> portalAnimation = new List<CharacterFrame>();
    [SerializeField] private List<CharacterFrame> endingFrames = new List<CharacterFrame>();
    [SerializeField] private GameObject portalLightObject;

    [Header("Offset settings")]
    [SerializeField] private float offsetY;
    [SerializeField] private float offsetX;

    [Header("Length settings")]
    [SerializeField]private List<int> tentaclesLength = new List<int>(4);

    private bool isPlayingAnimation = false;
    private string currentMode;
    private Coroutine portalAnimationCoroutine;
    private int chosenPosition;

    [Header("Sound settings")]
    [SerializeField] private AudioClip startPortalSound;
    [SerializeField] private AudioClip endPortalSound;
    [SerializeField] private AudioClip movingSound;
    [SerializeField] private AudioClip hitSound;

    private float increasingCoefficient = 1.0f;
    public void SetIncreasingCoefficient(float value) => increasingCoefficient = value;

    private void ActivateAnimation(bool state) {
        isPlayingAnimation = state;

        int signOffset = (state? (-1) : (1));

        portalLightObject.SetActive(state);
        GameObject.FindGameObjectWithTag("Character").transform.position += new Vector3(signOffset * offsetX, signOffset * offsetY, 0);
        GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterMovement>().SetActiveAnimation(!state);
    }

    private IEnumerator PlayAnimation(List<CharacterFrame> animation, Action actionOnEnd) {
        int currentFrame = 0;
        while (currentFrame < animation.Count) {
            GameObject.FindGameObjectWithTag("Character").GetComponent<SpriteRenderer>().sprite = animation[currentFrame].characterFrame;
            
            yield return new WaitForSeconds(animation[currentFrame].delay);
            ++currentFrame;
        }

        actionOnEnd?.Invoke();
    }

    private IEnumerator PlayConstantAnimation(List<CharacterFrame> animation) {
        int currentFrame = 0;
        while (true) {
            GameObject.FindGameObjectWithTag("Character").GetComponent<SpriteRenderer>().sprite = animation[currentFrame].characterFrame;

            yield return new WaitForSeconds(animation[currentFrame].delay);
            ++currentFrame;
            if (currentFrame == animation.Count)
                currentFrame = 0;

            yield return null;
        }
    }

    private void EndAnimation() {
        StopCoroutine(portalAnimationCoroutine);
        AudioManager.PlaySound(endPortalSound);
        StartCoroutine(PlayAnimation(endingFrames, () => {
            ActivateAnimation(false);
            RaiseSkillPlayedEvent();
        }));
    }

    private void DealDamageTentacles(int position) {
        int[] damageArray = new int[EnemyBase.START_ENEMY_NUMBER];

        if (chosenPosition < combatSystem.GetEnemyAmount() - 1) {
            damageArray[position] = base.GetDamage(0);
            damageArray[position + 1] = base.GetDamage(1);
        } else {
            damageArray[position] = base.GetDamage(0);
        }

        combatSystem.TakeMultipleDamage(damageArray);
    }

    public override void Use(int position) {
        RaiseOnSkillStartedPlayingEvent();
        chosenPosition = position;
        AudioManager.PlaySound(startPortalSound);
        ActivateAnimation(true);
        UseMana();

        StartCoroutine(PlayAnimation(beginningFrames, () => {
            portalAnimationCoroutine = StartCoroutine(PlayConstantAnimation(portalAnimation));
            GameObject.FindGameObjectWithTag("SkillManager").GetComponent<TentaclesObjectsAnimation>().PlayAnimation(tentaclesLength[chosenPosition], EndAnimation, () => {
                MakeShaking();
                DealDamageTentacles(chosenPosition);
                AudioManager.PlaySound(hitSound);
            });
        }));
    }

    public override int GetScaledDamage() => (int)(1.3f * (float)gameManager.character_magicpower * increasingCoefficient);
    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) => throw new System.NotImplementedException();
}
