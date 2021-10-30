using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HammerSkill : SplashSkill {
    [SerializeField] private GameObject hammerObject;
    [SerializeField] private List<Sprite> hammerFrames;
    [SerializeField] private float speed;
    [SerializeField] private float delayBetweenFrames;
    [SerializeField] private AudioClip hammerAppearingSound;
    [SerializeField] private AudioClip hammerHitSound;

    private IEnumerator ShowObject(GameObject hammerObject, float speed, Action actionOnEnd) {
        while (hammerObject.GetComponent<SpriteRenderer>().color.a < 1) {
            hammerObject.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, speed * Time.deltaTime);
            yield return null;
        }

        actionOnEnd?.Invoke();
    }

    private IEnumerator HideObject(GameObject hammerObject, float speed, Action actionOnEnd) {
        while (hammerObject.GetComponent<SpriteRenderer>().color.a >= 0) {
            hammerObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, speed * Time.deltaTime);
            yield return null;
        }

        actionOnEnd?.Invoke();
    }

    private IEnumerator PlayFrameAnimation(GameObject hammerObject, List<Sprite> frames, float delayBetweenFrames, Action actionOnEnd) {
        int currentFrame = 0;
        
        while (currentFrame < frames.Count) {
            hammerObject.GetComponent<SpriteRenderer>().sprite = frames[currentFrame++];
            yield return new WaitForSeconds(delayBetweenFrames);
        }

        actionOnEnd?.Invoke();
    }

    public override void Use(int position) {
        RaiseOnSkillStartedPlayingEvent();
        PlaySound(hammerAppearingSound);
        UseMana();

        StartCoroutine(ShowObject(hammerObject, speed, () => {
            StartCoroutine(PlayFrameAnimation(hammerObject, hammerFrames, delayBetweenFrames, () => {
                DealDamage();
                StartCoroutine(HideObject(hammerObject, speed, () => {
                    hammerObject.GetComponent<SpriteRenderer>().sprite = hammerFrames[0];
                    RaiseSkillPlayedEvent();
                }));

                MakeShaking();
                MakeShaking();

                PlaySound(hammerHitSound);
            }));
        }));
    }

    public override int GetScaledDamage() => (int)(0.6f * gameManager.character_attack);
    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) => throw new System.NotImplementedException();
}
