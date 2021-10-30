using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WideAttack : SplashSkill {
    [System.Serializable] public struct Frame {
        public GameObject frameObject;
        [HideInInspector] public Color startColor;
        public float hidingSpeed;
    }

    [Header("Animation settings")]
    [SerializeField] private Frame frame1;
    [SerializeField] private Frame frame2;
    [SerializeField] private float delay;
    private bool isPlayingAnimation;

    private void Start() {
        isPlayingAnimation = false;

        frame1.startColor = frame1.frameObject.GetComponent<SpriteRenderer>().color;
        frame2.startColor = frame2.frameObject.GetComponent<SpriteRenderer>().color;
    }

    private IEnumerator hideFrame (Frame frame) {
        if (isPlayingAnimation)
            yield return null;

        isPlayingAnimation = true;

        while (frame.frameObject.GetComponent<SpriteRenderer>().color.a > 0) {
            frame.frameObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, frame.hidingSpeed * Time.deltaTime);
            yield return null;
        }

        isPlayingAnimation = false;
    }

    private void SetStandartSettings() {
        frame1.frameObject.SetActive(false);
        frame2.frameObject.SetActive(false);
        frame1.frameObject.GetComponent<SpriteRenderer>().color = frame1.startColor;
        frame2.frameObject.GetComponent<SpriteRenderer>().color = frame2.startColor;
        isPlayingAnimation = false;
    }

    private IEnumerator playAnimation () {
        frame1.frameObject.SetActive(true);
        frame2.frameObject.SetActive(true);
        HideObject(frame1.frameObject, frame1.hidingSpeed);
        //StartCoroutine(hideFrame(frame1));
        DealDamage();
        MakeShaking();
        yield return new WaitForSeconds(delay);
        HideObject(frame2.frameObject, frame2.hidingSpeed);
        //StartCoroutine(hideFrame(frame2));
        RaiseSkillPlayedEvent();
    }

    public override void Use(int position) {
        //base.Use(position);
        RaiseOnSkillStartedPlayingEvent();
        UseMana();
        audioSource.PlayOneShot(useSound);
        SetStandartSettings();
        StartCoroutine(playAnimation());
    }

    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) => throw new System.NotImplementedException();

    public override int GetScaledDamage() => (int)((float)gameManager.character_attack / 2f);


}
